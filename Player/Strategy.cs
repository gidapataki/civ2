﻿using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CivSharp.Common;


namespace CivPlayer
{

	public delegate int UnitFitness(UnitInfo unit);
	public delegate int PositionFitness(Position pos);


	public class Strategy
	{
		protected Player player;
		protected List<TrackedUnit> trackedUnits;
		protected Random random;
		protected Plan plan;
		protected bool useFallback = true;

		public Strategy(Player player)
		{
			this.player = player;
			plan = new Plan(this);
			random = new Random();
			trackedUnits = new List<TrackedUnit>();
		}

		public static MovementData Move(UnitInfo unit, Position pos)
		{
			return new MovementData
			{
				UnitID = unit.UnitID,
				FromX = unit.PositionX,
				FromY = unit.PositionY,
				ToX = pos.x,
				ToY = pos.y,
			};
		}

		public static TrainingData TrainingDataFor(CityInfo city, UnitType unitType)
		{
			return TrainingDataFor(Position.Of(city), unitType);
		}

		public static TrainingData TrainingDataFor(Position pos, UnitType unitType)
		{
			return new TrainingData
			{
				PositionX = pos.x,
				PositionY = pos.y,
				UnitTypeName = unitType.GetDescription(),
			};
		}

		public static ResearchData ResearchDataFor(ResearchType res)
		{
			return new ResearchData
			{
				WhatToResearch = res.GetDescription()
			};
		}

		public static BuildingData BuildingDataFor(UnitInfo unit)
		{
			return BuildingDataFor(Position.Of(unit));
		}

		public static BuildingData BuildingDataFor(Position pos)
		{
			return new BuildingData
			{
				PositionX = pos.x,
				PositionY = pos.y
			};
		}

		// 

		public bool HasMoney(int cost)
		{
			return player.Money >= cost;
		}

		public bool HasBudget(int cost)
		{
			return player.Money - plan.PlanCost >= cost;
		}

		public bool IsPlanned(ResearchType type)
		{
			return plan.ResearchList.Contains(type);
		}

		public bool HasResearch(ResearchType type)
		{
			return type == ResearchType.None || player.Researched.Contains(type.GetDescription());
		}

		public bool CanMove(UnitInfo unit, Position pos)
		{
			var unitPos = Position.Of(unit);
			return pos.IsValid() && unitPos.IsNeighbour(pos) && unit.MovementPoints > 0;
		}

		public bool CanTrain(UnitType type)
		{
			return player.MyUnits.Count() < Math.Min(20, player.MyCities.Count() * 5) &&
				HasResearch(Unit.Requirement(type)) && HasMoney(Unit.Cost(type));
		}

		public bool CanResearch(ResearchType type)
		{
			return !HasResearch(type) && HasResearch(Research.Requirement(type)) && HasMoney(Research.Cost(type));
		}

		public bool IsMyUnitAt(Position pos)
		{
			return player.MyUnits.Any(unit => Position.Of(unit).Equals(pos));
		}

		public bool IsEnemyUnitAt(Position pos)
		{
			return player.EnemyUnits.Any(unit => Position.Of(unit).Equals(pos));
		}

		public bool IsDefendedBy(CityInfo city, UnitType type)
		{
			var pos = Position.Of(city);
			return MyUnitsAt(pos).Concat(EnemyUnitsAt(pos)).Any(p => p.GetUnitType().Equals(type));
		}

		public bool DefendCity(CityInfo city, UnitType type)
		{
			var pos = Position.Of(city);
			var res = Unit.Requirement(type);
			var rcost = Research.Cost(res);
			var ucost = Unit.Cost(type);

			if (!HasResearch(res) && !IsPlanned(res) && HasBudget(rcost + ucost))
			{
				plan.Want(res);
				plan.Want(type, pos);
				return true;
			}
			else if ((HasResearch(res) || IsPlanned(res)) && HasBudget(ucost))
			{
				plan.Want(type, pos);
				return true;
			}
			return false;
		}

		public UnitInfo[] MyUnitsAt(Position pos)
		{
			return player.MyUnits.Where(p => Position.Of(p).Equals(pos)).ToArray();
		}

		public UnitInfo[] EnemyUnitsAt(Position pos)
		{
			return player.EnemyUnits.Where(p => Position.Of(p).Equals(pos)).ToArray();
		}

		public int TravelRounds(UnitInfo unit, Position pos)
		{
			var dist = Position.Of(unit).Distance(pos);
			var div = Unit.Stats(unit.GetUnitType()).Movement;
			return (dist + div - 1) / div;
		}

		public bool IsCity(Position pos)
		{
			return IsMyCity(pos) || IsEnemyCity(pos);
		}

		public CityInfo[] SortMyCities(PositionFitness fitnessFunc)
		{
			return player.MyCities.Select(city => Tuple.Create(fitnessFunc(Position.Of((city))), city))
				.OrderBy(p => -p.Item1)
				.Select(p => p.Item2)
				.ToArray();
		}


		public int MyClosestCity(Position pos)
		{
			return player.MyCities.Select(city => Position.Of(city).Distance(pos)).Min();
		}
		
		public int[] CityDistances(Position pos)
		{
			return player.AllCities.Select(city => Position.Of(city).Distance(pos)).ToArray();
		}

		public int EnemyDistance(Position pos)
		{
			return player.EnemyCities.Select(city => Position.Of(city).Distance(pos)).Min();
		}

		public bool IsMyCity(Position pos)
		{
			return player.MyCities.Any(city => Position.Of(city).Equals(pos));
		}

		public bool IsEnemyCity(Position pos)
		{
			return player.EnemyCities.Any(city => Position.Of(city).Equals(pos));
		}

		public bool CanColonize(int deposit = 0)
		{
			return HasBudgetFor(Colony: 1, Deposit: deposit) && player.MyCities.Count() < 4;
		}

		public bool BeforeCanColonize()
		{
			return HasBudget(Constant.ColonyCost + (player.MyUnits.Any() ? 0 : Unit.Cost(UnitType.Felderito)) - Income);
		}

		public bool BeforeCanColonizeLovag()
		{
			return HasBudget(Constant.ColonyCost + (player.MyUnits.Any() ? 0 : Unit.Cost(UnitType.Lovag)) - Income);
		}

		public bool CanBuild(UnitInfo unit)
		{
			return !IsMyCity(Position.Of(unit)) && CanColonize();
		}

		public bool CanBuild(Position pos)
		{
			return IsMyUnitAt(pos) && !IsMyCity(pos) && CanColonize();
		}

		public double ChanceToLose(Position pos)
		{
			var plannedUnits = plan.TrainingList.Where(t => t.Item2.Equals(pos)).Select(t => t.Item1).ToArray();
			var threat = player.EnemyUnits.Where(u => TravelRounds(u, pos) <= 1).ToArray();
			var survive = Battle.WillDefendersSurvive(player.world, threat, pos, plannedUnits);

			return survive ? 0 : 1;
		}

		public UnitInfo FindUnit(UnitFitness fitnessFunc, UnitInfo[] units = null)
		{
			int fitness = Int32.MinValue;
			UnitInfo candidate = null;
			if (units == null) { units = player.MyUnits; }
			foreach (var unit in units)
			{
				int f = fitnessFunc(unit);
				if (f > fitness) { fitness = f; candidate = unit; }
			}
			return candidate;
		}

		public UnitInfo FindEnemyUnit(UnitFitness fitnessFunc)
		{
			return FindUnit(fitnessFunc, player.EnemyUnits);
		}

		public Position FindUnitTarget(UnitInfo unit, PositionFitness fitnessFunc)
		{
			Position pos = Position.Of(unit);
			Position target = pos;
			int fitness = fitnessFunc(pos);
			int delta = unit.MovementPoints;

			for (int x = -delta; x <= delta; ++x)
			{
				for (int y = -delta; y <= delta; ++y)
				{
					var p = pos.Offset(x, y);
					if (p.IsValid() && !p.Equals(pos))
					{
						var f = fitnessFunc(p);
						if (f > fitness) { fitness = f; target = p; }
					}
				}
			}
			return target;
		}

		public bool HasBudgetFor(
			int Felderito = 0, int Lovag = 0, int Orzo = 0, int Tanonc = 0, int Mester = 0,
			int Falu = 0, int Varos = 0, int OrzokTornya = 0, int Kovacsmuhely = 0, int Barakk = 0, int HarciAkademia = 0, int Varoshaza = 0, int Bank = 0, int Barikad = 0, int Fal = 0,
			int Colony = 0, int Deposit = 0,
			bool NextRound = false)
		{
			var budget = player.Money + (NextRound ? Income : 0) - plan.PlanCost;
			var cost = Felderito * Unit.Cost(UnitType.Felderito) +
					   Lovag * Unit.Cost(UnitType.Lovag) +
					   Orzo * Unit.Cost(UnitType.Orzo) +
					   Tanonc * Unit.Cost(UnitType.Tanonc) +
					   Mester * Unit.Cost(UnitType.Mester) +
			           Falu*Research.Cost(ResearchType.Falu) +
			           Varos*Research.Cost(ResearchType.Varos) +
			           OrzokTornya*Research.Cost(ResearchType.OrzokTornya) +
			           Kovacsmuhely*Research.Cost(ResearchType.Kovacsmuhely) +
			           Barakk*Research.Cost(ResearchType.Barakk) +
			           HarciAkademia*Research.Cost(ResearchType.HarciAkademia) +
			           Varoshaza*Research.Cost(ResearchType.Varoshaza) +
			           Bank*Research.Cost(ResearchType.Bank) +
			           Barikad*Research.Cost(ResearchType.Barikad) +
			           Fal*Research.Cost(ResearchType.Fal) +
			           Colony*Constant.ColonyCost +
			           Deposit;

			return budget >= cost;
		}


		public int Money
		{
			get { return player.Money; }
		}

		public int Income
		{
			get { return (25 + (HasResearch(ResearchType.Varoshaza) ? 10 : 0) + (HasResearch(ResearchType.Bank) ? 40 : 0)) * Math.Min(4, player.MyCities.Count()); }
		}

		public int NumberOfUnits
		{
			get { return player.MyUnits.Count(); }
		}

		public int NumberOfCities
		{
			get { return player.MyCities.Count(); }
		}

		public void ActionResult(WorldInfo world)
		{
			RefreshUnits(world);
		}

		public void RefreshUnits(WorldInfo world)
		{
			foreach (var u in trackedUnits)
			{
				u.Refresh(world);
			}
		}

		/// <summary>
		/// Chance and severity of a riposte
		/// </summary>
		/// <param name="type"></param>
		/// <param name="pos"></param>
		/// <returns>damage, chance of riposte, expected damage</returns>
		public Tuple<int, double, double> Riposte(UnitType type, Position pos)
		{
			var att = Unit.Stats(type).Attack;
			var riposte = EnemyUnitsAt(pos)
				.Select(p => Unit.Stats(p.GetUnitType()))
				.Select(p => Tuple.Create(p.Defense, p.Damage, 1 - Unit.ChanceToHit(att, p.Defense)))
				.OrderBy(t => -t.Item1)
				.FirstOrDefault();

			if (riposte == null)
				return Tuple.Create(0, 0.0, 0.0);
			else
				return Tuple.Create(riposte.Item2, riposte.Item3, riposte.Item2 * riposte.Item3);
		}

		public ResearchData NextResearch()
		{
			if (plan.ResearchList.Any())
			{
				var next = plan.ResearchList.First();
				plan.ResearchList.RemoveAt(0);
				if (CanResearch(next))
					return ResearchDataFor(next);
			}
			return null;
		}

		public TrainingData NextTraining()
		{
			if (plan.TrainingList.Any())
			{
				var next = plan.TrainingList.First();
				plan.TrainingList.RemoveAt(0);
				if (CanTrain(next.Item1) && IsMyCity(next.Item2))
					return TrainingDataFor(next.Item2, next.Item1);
			}
			return null;
		}

		public BuildingData NextBuilding()
		{
			if (plan.BuildList.Any())
			{
				var next = plan.BuildList.First();
				plan.BuildList.RemoveAt(0);
				if (CanBuild(next))
					return BuildingDataFor(next);
			}
			return null;
		}

		public MovementData NextMovement()
		{

			while (plan.AttackList.Any())
			{
				var pos = plan.AttackList.First();
				if (!IsEnemyUnitAt(pos))
				{
					plan.AttackList.RemoveAt(0);
				}
				else
				{
					UnitInfo mester = player.MyUnits.Where(unit =>
						Position.Of(unit).Distance(pos) <= unit.MovementPoints-1  &&
						MyClosestCity(pos) <= 2 && unit.GetUnitType() == UnitType.Mester)
						.FirstOrDefault();

					UnitInfo defender = player.MyUnits.Where(unit => 
						Position.Of(unit).Distance(pos) <= unit.MovementPoints && 
						unit.GetUnitType() != UnitType.Felderito &&
						unit.GetUnitType() != UnitType.Orzo)
							.Select(unit => Tuple.Create(unit.HitPoints, Riposte(unit.GetUnitType(), pos), unit))
							.Select(t => Tuple.Create(t.Item1 - t.Item2.Item3, t.Item3))
							.OrderBy(n => -n.Item1)
							.Select(n => n.Item2)
							.FirstOrDefault();

					//if (mester != null)
					//{
					//	defender = mester; 
					//}

					if (defender != null)
					{
						if (CanMove(defender, pos))
							return Move(defender, pos);
					}
					plan.AttackList.RemoveAt(0);
				}
			}

			while (plan.MovementList.Any())
			{
				var next = plan.MovementList.First();
				var unit = player.MyUnits.SingleOrDefault(u => u.UnitID == next.Item1);
				plan.MovementList.RemoveAt(0);
				if (unit != null && CanMove(unit, next.Item2))
					return Move(unit, next.Item2);
			}

			return (useFallback ? Fallback() : null);
		}

		public bool WontBuild(UnitInfo u)
		{
			var pos = Position.Of(u);
			return !plan.BuildList.Any(p => p.Equals(pos));
		}

		public MovementData Fallback()
		{
			//	- not in town, back to town
			//	- in town, find in range: should raise population
			var lostUnit = player.MyUnits.FirstOrDefault(u => !IsMyCity(Position.Of(u)) && WontBuild(u) && u.MovementPoints > 0);
			var cityUnits = player.MyUnits.Where(u => IsMyCity(Position.Of(u)) && u.GetUnitType() != UnitType.Felderito && u.MovementPoints > 0);

			if (lostUnit != null)
			{
				var pos = Position.Of(lostUnit);
				var target = FindUnitTarget(lostUnit, p =>
					+ (IsMyCity(p) ? 100 : 0)
					- (MyClosestCity(p)*1)
					- (MyUnitsAt(p).Count()*5)
					);

				target = pos.Closer(target);
				if (CanMove(lostUnit, target))
				{
					return Move(lostUnit, target);
				}
			}
			else if (cityUnits.Any())
			{
				foreach (var u in cityUnits.Where(u => u.GetUnitType() != UnitType.Orzo))
				{
					var pos = Position.Of(u);
					var target = FindUnitTarget(u, p =>
						+(IsMyCity(p) ? 100 : 0)
						- (MyUnitsAt(p).Count() * 5)
						);

					target = pos.Closer(target);

					var there = MyUnitsAt(target).Count();
					var here = MyUnitsAt(pos).Count();

					if (here - 1 >= there + 1  && CanMove(u, target))
					{
						return Move(u, target);
					}
				}
			}
			return null;
		}


		public void Track(TrackedUnit t, UnitInfo unit)
		{
			if (t != null)
			{
				trackedUnits.Remove(t);
				trackedUnits.Add(t);
				t.unit = unit;
			}
		}

		public void Untrack(TrackedUnit unit)
		{
			trackedUnits.Remove(unit);
		}

		public void NewRound()
		{
			plan.Bump();
			PlotMasterPlan();
		}

		public virtual void UnitLost(string unitID) {}

		public virtual void PlotMasterPlan() { }
		public virtual void BeforeMovement() { }
		public virtual void BeforeResearch() { }
		public virtual void BeforeBuilding() { }
		public virtual void BeforeTraining() { }

		public virtual MovementData OnMovement() { return NextMovement(); }
		public virtual ResearchData OnResearch() { return NextResearch(); }
		public virtual BuildingData OnBuilding() { return NextBuilding(); }
		public virtual TrainingData OnTraining() { return NextTraining(); }
	}


}
