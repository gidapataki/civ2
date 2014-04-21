using System;
using System.ComponentModel;
using System.Linq;
using System.Collections.Generic;
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

		public Strategy(Player player)
		{
			this.player = player;
			plan = new Plan();
			random = new Random();
			trackedUnits = new List<TrackedUnit>();
		}

		public static ResearchType UnitRequirement(UnitType type)
		{
			switch (type)
			{
				case UnitType.Felderito: return ResearchType.None;
				case UnitType.Orzo: return ResearchType.OrzokTornya;
				case UnitType.Lovag: return ResearchType.Kovacsmuhely;
				case UnitType.Tanonc: return ResearchType.Barakk;
				case UnitType.Mester: return ResearchType.HarciAkademia;
				default: return ResearchType.None;
			}
		}

		public static ResearchType ResearchRequirement(ResearchType type)
		{
			switch (type)
			{
				case ResearchType.Falu:
				case ResearchType.OrzokTornya:
				case ResearchType.Kovacsmuhely:
				case ResearchType.Barakk:
					return ResearchType.None;
				case ResearchType.Varos:
				case ResearchType.Varoshaza:
				case ResearchType.Barikad:
					return ResearchType.Falu;
				case ResearchType.HarciAkademia:
					return ResearchType.Barakk;
				case ResearchType.Bank:
					return ResearchType.Varos;
				case ResearchType.Fal:
					return ResearchType.Barikad;
				default:
					return ResearchType.None;
			}
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


		public static TrainingData Train(CityInfo city, UnitType unitType)
		{
			return Train(Position.Of(city), unitType);
		}


		public static TrainingData Train(Position pos, UnitType unitType)
		{
			return new TrainingData
			{
				PositionX = pos.x,
				PositionY = pos.y,
				UnitTypeName = unitType.GetDescription(),
			};
		}

		public static ResearchData Research(ResearchType res)
		{
			return new ResearchData
			{
				WhatToResearch = res.GetDescription()
			};
		}

		public static BuildingData Build(UnitInfo unit)
		{
			return Build(Position.Of(unit));
		}

		public static BuildingData Build(Position pos)
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
				HasResearch(UnitRequirement(type)) && HasMoney(Constant.UnitCost(type));
		}

		public bool CanResearch(ResearchType type)
		{
			return !HasResearch(type) && HasResearch(ResearchRequirement(type)) && HasMoney(Constant.ResearchCost(type));
		}

		public bool IsMyUnitAt(Position pos)
		{
			return player.MyUnits.Any(unit => Position.Of(unit).Equals(pos));
		}

		public bool IsEnemyUnitAt(Position pos)
		{
			return player.EnemyUnits.Any(unit => Position.Of(unit).Equals(pos));
		}

		public bool IsDefended(CityInfo city)
		{
			var pos = Position.Of(city);
			return IsMyUnitAt(pos) || IsEnemyUnitAt(pos);
		}

		public int TravelRounds(UnitInfo unit, Position pos)
		{
			var dist = Position.Of(unit).Distance(pos);
			var div = Constant.MovementPoints(unit.GetUnitType());
			return (dist + div - 1)/div;
		}

		public bool IsCity(Position pos)
		{
			return IsMyCity(pos) || IsEnemyCity(pos);
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

		public bool CanColonize()
		{
			return HasMoney(Constant.ColonyCost) && player.MyCities.Count() < 4;
		}

		public bool BeforeCanColonize()
		{
			return Money + Income >= Constant.ColonyCost + (player.MyUnits.Any() ? 0 : Constant.UnitCost(UnitType.Felderito));
		}

		public bool CanBuild(UnitInfo unit)
		{
			return !IsMyCity(Position.Of(unit)) && CanColonize();
		}

		public bool CanBuild(Position pos)
		{
			return IsMyUnitAt(pos) && !IsMyCity(pos) && CanColonize();
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

		public bool HasMoneyFor(
			int Felderito = 0, int Lovag = 0, int Orzo = 0, int Tanonc = 0, int Mester = 0,
			int Colony = 0, int Falu = 0, int Varos = 0, int OrzokTornya = 0, int Kovacsmuhely = 0, int Barakk = 0, int HarciAkademia = 0, int Varoshaza = 0, int Bank = 0, int Barikad = 0, int Fal = 0,
			bool NextRound = false)
		{
			var money = player.Money + (NextRound ? Income : 0);
			var cost = Felderito * Constant.UnitCost(UnitType.Felderito) +
				Lovag * Constant.UnitCost(UnitType.Lovag) +
				Orzo * Constant.UnitCost(UnitType.Orzo) +
				Tanonc * Constant.UnitCost(UnitType.Tanonc) +
				Mester * Constant.UnitCost(UnitType.Mester) +
				Colony * Constant.ColonyCost +
				Falu * Constant.ResearchCost(ResearchType.Falu) +
				Varos * Constant.ResearchCost(ResearchType.Varos) +
				OrzokTornya * Constant.ResearchCost(ResearchType.OrzokTornya) +
				Kovacsmuhely * Constant.ResearchCost(ResearchType.Kovacsmuhely) +
				Barakk * Constant.ResearchCost(ResearchType.Barakk) +
				HarciAkademia * Constant.ResearchCost(ResearchType.HarciAkademia) +
				Varoshaza * Constant.ResearchCost(ResearchType.Varoshaza) +
				Bank * Constant.ResearchCost(ResearchType.Bank) +
				Barikad * Constant.ResearchCost(ResearchType.Barikad) +
				Fal * Constant.ResearchCost(ResearchType.Fal);

			return money >= cost;
		}


		public int Money
		{
			get { return player.Money; }
		}

		public int Income
		{
			get { return (25 + (HasResearch(ResearchType.Varoshaza) ? 10 : 0) + (HasResearch(ResearchType.Bank) ? 20 : 0)) * player.MyCities.Count(); }
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

		public ResearchData NextResearch()
		{
			if (plan.ResearchList.Any())
			{
				var next = plan.ResearchList.First();
				plan.ResearchList.RemoveAt(0);
				if (CanResearch(next))
					return Research(next);
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
					return Train(next.Item2, next.Item1);
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
					return Build(next);
			}
			return null;
		}

		public MovementData NextMovement()
		{
			while (plan.MovementList.Any())
			{
				var next = plan.MovementList.First();
				var unit = player.MyUnits.SingleOrDefault(u => u.UnitID == next.Item1);
				plan.MovementList.RemoveAt(0);
				if (unit != null && CanMove(unit, next.Item2))
					return Move(unit, next.Item2);
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
