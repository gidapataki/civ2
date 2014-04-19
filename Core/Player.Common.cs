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
		private UnitInfo[] myUnits;
		private CityInfo[] myCities;

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
			myUnits = world.Units.Where(p => p.Owner == PlayerName && p.HitPoints > 0).ToArray();
			myCities = world.Cities.Where(p => p.Owner == PlayerName).ToArray();
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
					Func<int, int> converge = (int y) => sgn(5 - y) + y;
					var targetY = converge(unit.PositionY);

					if (targetY != unit.PositionY)
					{
						return Movement(unit, unit.PositionX, targetY);
					}
				}
			}
			return null;
		}


		private ResearchData Research(ResearchType research)
		{
			return new ResearchData { WhatToResearch = research.GetDescription() };
		}

		private bool HasResearch(ResearchType research)
		{
			return myPlayer.Researched.Contains(research.GetDescription());
		}

		private bool CanResearch(ResearchType research)
		{
			switch (research) {
				case ResearchType.Falu:
				case ResearchType.OrzokTornya:
				case ResearchType.Kovacsmuhely:
				case ResearchType.Barakk:
					return true;
				case ResearchType.Varos:
				case ResearchType.Varoshaza:
				case ResearchType.Barikad:
					return HasResearch(ResearchType.Falu);
				case ResearchType.HarciAkademia:
					return HasResearch(ResearchType.Barakk);
				case ResearchType.Bank:
					return HasResearch(ResearchType.Varos);
				case ResearchType.Fal:
					return HasResearch(ResearchType.Barikad);
				default:
					return false;
			}
		}

		private static int ResearchCost(ResearchType research)
		{
			switch (research)
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
				default: return -1;
			}
		}

		private bool HasMoney(int x)
		{
			return x >= 0 && myPlayer.Money >= x;
		}


		private ResearchData TryResearch(ResearchType research)
		{
			return !HasResearch(research) && CanResearch(research) && HasMoney(ResearchCost(research))
				? Research(research)
				: null;
		}

		public ResearchData OnResearch() // API-2
		{
			return TryResearch(ResearchType.OrzokTornya);
		}


		public BuildingData OnBuilding() // API-3
		{
			return null;
		}


		private TrainingData Training(CityInfo city, UnitType unitType)
		{
			return new TrainingData
			{
				PositionX = city.PositionX,
				PositionY = city.PositionY,
				UnitTypeName = unitType.GetDescription(),
			};
		}


		private static int TrainingCost(UnitType unitType)
		{
			switch (unitType)
			{
				case UnitType.Felderito: return 50;
				case UnitType.Orzo: return 75;
				case UnitType.Lovag: return 150;
				case UnitType.Tanonc: return 100;
				case UnitType.Mester: return 200;
				default: return -1;
			}
		}


		private bool CanTrain(UnitType unitType)
		{
			switch (unitType)
			{
				case UnitType.Felderito: return true;
				case UnitType.Orzo: return HasResearch(ResearchType.OrzokTornya);
				case UnitType.Lovag: return HasResearch(ResearchType.Kovacsmuhely);
				case UnitType.Tanonc: return HasResearch(ResearchType.Barakk);
				case UnitType.Mester: return HasResearch(ResearchType.HarciAkademia);
				default: return false;
			}
		}


		private TrainingData TryTraining(CityInfo city, UnitType unitType)
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
				return TryTraining(city, UnitType.Felderito);
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
