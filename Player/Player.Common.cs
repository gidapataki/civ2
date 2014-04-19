using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;
using CivPlayer.Enums;
using CivPlayer.Helpers;


namespace CivPlayer
{
	public abstract class Player
	{
		private int turn;
		private WorldInfo world;
		private PlayerInfo myPlayer;
		private List<UnitInfo> myUnits;
		private List<CityInfo> myCities;

		public abstract string PlayerName { get; }

		public string PlayerRace // API
		{
			get { return "civ2"; }
		}

		public void RefreshWorldInfo(int turn, WorldInfo world) // API
		{
			this.turn = turn;
			this.world = world;
			UpdateStats();
		}


		private void UpdateStats()
		{
			myPlayer = world.Players.Single(p => p.Name == PlayerName);
			myUnits = world.Units.Where(p => p.Owner == PlayerName && p.HitPoints > 0).ToList();
			myCities = world.Cities.Where(p => p.Owner == PlayerName).ToList();
		}


		private MovementData Movement(UnitInfo unit, int x, int y)
		{
			return new MovementData {
				UnitID = unit.UnitID,
				FromX = unit.PositionX,
				FromY = unit.PositionY,
				ToX = x,
				ToY = y,
			};
		}


		public MovementData OnMovement() // API-1
		{
			foreach (var unit in myUnits)
			{
				if (unit.MovementPoints > 0)
				{
					Func<int, int> sgn = (int y) => y < 0 ? -1 : (y > 0 ? 1 : 0);
					Func<int, int> converge = (int y) => sgn(10 - y) + y;
					var targetY = converge(unit.PositionY);

					if (targetY != unit.PositionY)
					{
						return Movement(unit, unit.PositionX, targetY);
					}
				}
			}
			return null;
		}


		private ResearchData Research(ResearchTypes research)
		{
			return new ResearchData { WhatToResearch = research.GetDescription() };
		}

		private bool HasResearch(ResearchTypes research)
		{
			return myPlayer.Researched.Contains(research.GetDescription());
		}

		private bool CanResearch(ResearchTypes research)
		{
			switch (research) {
				case ResearchTypes.Falu:
				case ResearchTypes.OrzokTornya:
				case ResearchTypes.Kovacsmuhely:
				case ResearchTypes.Barakk:
					return true;
				case ResearchTypes.Varos:
				case ResearchTypes.Varoshaza:
				case ResearchTypes.Barikad:
					return HasResearch(ResearchTypes.Falu);
				case ResearchTypes.HarciAkademia:
					return HasResearch(ResearchTypes.Barakk);
				case ResearchTypes.Bank:
					return HasResearch(ResearchTypes.Varos);
				case ResearchTypes.Fal:
					return HasResearch(ResearchTypes.Barikad);
				default:
					return false;
			}
		}

		private static int ResearchCost(ResearchTypes research)
		{
			switch (research)
			{
				case ResearchTypes.Falu: return 200;
				case ResearchTypes.Varos: return 300;
				case ResearchTypes.OrzokTornya: return 100;
				case ResearchTypes.Kovacsmuhely: return 150;
				case ResearchTypes.Barakk: return 200;
				case ResearchTypes.HarciAkademia: return 500;
				case ResearchTypes.Varoshaza: return 150;
				case ResearchTypes.Bank: return 300;
				case ResearchTypes.Barikad: return 100;
				case ResearchTypes.Fal: return 200;
				default: return -1;
			}
		}

		private bool HasMoney(int x)
		{
			return x >= 0 && myPlayer.Money >= x;
		}


		private ResearchData TryResearch(ResearchTypes research)
		{
			return !HasResearch(research) && CanResearch(research) && HasMoney(ResearchCost(research))
				? Research(research)
				: null;
		}

		public ResearchData OnResearch() // API-2
		{
			return TryResearch(ResearchTypes.OrzokTornya);
		}


		public BuildingData OnBuilding() // API-3
		{
			return null;
		}


		private TrainingData Training(CityInfo city, UnitTypes unitType)
		{
			return new TrainingData
			{
				PositionX = city.PositionX,
				PositionY = city.PositionY,
				UnitTypeName = unitType.GetDescription(),
			};
		}


		private static int TrainingCost(UnitTypes unitType)
		{
			switch (unitType)
			{
				case UnitTypes.Felderito: return 50;
				case UnitTypes.Orzo: return 75;
				case UnitTypes.Lovag: return 150;
				case UnitTypes.Tanonc: return 100;
				case UnitTypes.Mester: return 200;
				default: return -1;
			}
		}


		private bool CanTrain(UnitTypes unitType)
		{
			switch (unitType)
			{
				case UnitTypes.Felderito: return true;
				case UnitTypes.Orzo: return HasResearch(ResearchTypes.OrzokTornya);
				case UnitTypes.Lovag: return HasResearch(ResearchTypes.Kovacsmuhely);
				case UnitTypes.Tanonc: return HasResearch(ResearchTypes.Barakk);
				case UnitTypes.Mester: return HasResearch(ResearchTypes.HarciAkademia);
				default: return false;
			}
		}


		private TrainingData TryTraining(CityInfo city, UnitTypes unitType)
		{
			return CanTrain(unitType) && HasMoney(TrainingCost(unitType))
				? Training(city, unitType)
				: null;
		}


		public TrainingData OnTraining() // API-4
		{
			if (Math.Min(20, myCities.Count() * 5) <= myUnits.Count()) { return null; }
			else
			{
				var city = myCities.First();
				return TryTraining(city, UnitTypes.Felderito);
			}
		}


		public void ActionResult(WorldInfo world) // API
		{
			this.world = world;
			UpdateStats();
		}

		
		public void CityLost(int positionX, int positionY) // API
		{

		}

		public void EnemyDestroyed(string playerName) // API
		{

		}

		public void GameOver(bool winner, string message) // API
		{

		}

		public void UnitLost(string unitID) // API
		{

		}
	}
}
