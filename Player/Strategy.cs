﻿using System;
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

		public Strategy(Player player)
		{
			this.player = player;
			random = new Random();
			trackedUnits = new List<TrackedUnit>();
		}

		public static int UnitCost(UnitType type)
		{
			switch (type)
			{
				case UnitType.Felderito: return 50;
				case UnitType.Orzo: return 75;
				case UnitType.Lovag: return 150;
				case UnitType.Tanonc: return 100;
				case UnitType.Mester: return 200;
				default: return 0;
			}
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

		public static int ResearchCost(ResearchType type)
		{
			switch (type)
			{
				case ResearchType.Falu: return 200;
				case ResearchType.Varos: return 300;
				case ResearchType.OrzokTornya: return 100;
				case ResearchType.Kovacsmuhely: return 150;
				case ResearchType.Barakk: return 200;
				case ResearchType.HarciAkademia: return 500;
				case ResearchType.Varoshaza: return 150;
				case ResearchType.Bank: return 300;
				case ResearchType.Barikad: return 100;
				case ResearchType.Fal: return 200;
				default: return 0;
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
			return new TrainingData
			{
				PositionX = city.PositionX,
				PositionY = city.PositionY,
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
			return new BuildingData
			{
				PositionX = unit.PositionX,
				PositionY = unit.PositionY
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
				HasResearch(UnitRequirement(type)) && HasMoney(UnitCost(type));
		}

		public bool CanResearch(ResearchType type)
		{
			return !HasResearch(type) && HasResearch(ResearchRequirement(type)) && HasMoney(ResearchCost(type));
		}

		public bool IsCity(Position pos)
		{
			return IsMyCity(pos) || IsEnemyCity(pos);
		}

		public int[] CityDistances(Position pos)
		{
			return player.AllCities.Select(city => Position.Of(city).Distance(pos)).ToArray();
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
			return HasMoney(300) && player.MyCities.Count() < 4;
		}

		public bool CanBuild(UnitInfo unit)
		{
			return !IsMyCity(Position.Of(unit)) && CanColonize();
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

		public virtual void BeforeMovement() { }
		public virtual void BeforeResearch() { }
		public virtual void BeforeBuilding() { }
		public virtual void BeforeTraining() { }

		public virtual MovementData OnMovement() { return null; }
		public virtual ResearchData OnResearch() { return null; }
		public virtual BuildingData OnBuilding() { return null; }
		public virtual TrainingData OnTraining() { return null; }
	}





}
