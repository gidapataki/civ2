using System;
using System.Linq;
using CivSharp.Common;
using CivPlayer.Enums;
using CivPlayer.Helpers;


namespace CivPlayer
{


	class Strategy
	{
		private Player player;
		
		public Strategy(Player player) 
		{
			this.player = player; 
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
			switch (type) {
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
			return new MovementData {
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
			var unitPos = new Position(unit);
			return pos.IsValid() && unitPos.IsNeighbour(pos) &&	unit.MovementPoints > 0;
		}

		public bool CanTrain(UnitType type)
		{
			return player.Units.Count() < Math.Min(20, player.Cities.Count() * 5) &&
				HasResearch(UnitRequirement(type)) && HasMoney(UnitCost(type));
		}

		public bool CanResearch(ResearchType type)
		{
			return !HasResearch(type) && HasResearch(ResearchRequirement(type)) && HasMoney(ResearchCost(type));
		}

		public bool IsMyCity(Position pos)
		{
			foreach (var city in player.Cities) {
				if (city.PositionX == pos.x && city.PositionY == pos.y) { return true; }
			}
			return false;
		}

		public bool IsEnemCity(Position pos)
		{
			foreach (var city in player.EnemyCities) {
				if (city.PositionX == pos.x && city.PositionY == pos.y) { return true; }
			}
			return false;
		}

		public bool CanBuild(UnitInfo unit)
		{
			return !IsMyCity(new Position(unit)) && HasMoney(300);
		}
	}



	class ColonizeStrategy
	{
		// train.scout if no units
		// move.[somewhere uncolonized] if CanColonize()
		// build if money >= 300
		
		// move, res, build, train
	}
}
