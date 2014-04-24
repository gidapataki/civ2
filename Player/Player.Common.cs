using System;
using System.Drawing;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;


namespace CivPlayer
{

	public abstract class Player
	{
		public int Turn { get; private set; }
		private Phase phase;
		public WorldInfo world { get; private set; }
		private PlayerInfo myPlayer;
		private PlayerInfo enemyPlayer;
		private UnitInfo[] myUnits;
		private UnitInfo[] enemyUnits;
		
		private CityInfo[] myCities;
		private CityInfo[] enemyCities;

		protected Strategy strategy;

		public abstract string PlayerName { get; }


		public string PlayerRace // API
		{
			get { return "civ2"; }
		}

		public void RefreshWorldInfo(int turn, WorldInfo world) // API
		{
			this.Turn = turn;
			this.world = world;
			phase = Phase.Turn;
			UpdateStats();
			strategy.NewRound();
		}

		public MovementData OnMovement() // API
		{
			if (phase != Phase.Movement)
			{
				phase = Phase.Movement;
				strategy.BeforeMovement();
			}
			return strategy.OnMovement();
		}


		public ResearchData OnResearch() // API
		{
			if (phase != Phase.Research)
			{
				phase = Phase.Research;
				strategy.BeforeResearch();
			}
			return strategy.OnResearch();
		}

		public BuildingData OnBuilding() // API
		{
			if (phase != Phase.Building)
			{
				phase = Phase.Building;
				strategy.BeforeBuilding();
			}
			return strategy.OnBuilding();
		}

		public TrainingData OnTraining() // API
		{
			if (phase != Phase.Training)
			{
				phase = Phase.Training;
				strategy.BeforeTraining();
			}
			return strategy.OnTraining();
		}

		public void ActionResult(WorldInfo world) // API
		{
			this.world = world;
			UpdateStats();
			strategy.ActionResult(world);
		}
		
		public void CityLost(int positionX, int positionY) // API
		{}

		public void EnemyDestroyed(string playerName) // API
		{}

		public void GameOver(bool winner, string message) // API
		{}

		public void UnitLost(string unitID) // API
		{}

		private void UpdateStats()
		{
			myPlayer = world.Players.Single(p => p.Name == PlayerName);
			enemyPlayer = world.Players.Single(p => p.Name != PlayerName);
			myUnits = world.Units.Where(p => p.Owner == PlayerName && p.HitPoints > 0).ToArray();
			enemyUnits = world.Units.Where(p => p.Owner != PlayerName && p.HitPoints > 0).ToArray();

			myCities = world.Cities.Where(p => p.Owner == PlayerName).ToArray();
			enemyCities = world.Cities.Where(p => p.Owner != PlayerName).ToArray();
		}


		public UnitInfo[] MyUnits
		{
			get { return myUnits; }
		}

		public UnitInfo[] EnemyUnits
		{
			get { return enemyUnits; }
		}

		public CityInfo[] AllCities
		{
			get { return world.Cities; }
		}

		public CityInfo[] MyCities
		{
			get { return myCities; }
		}

		public CityInfo[] EnemyCities
		{
			get { return enemyCities; }
		}

		public int Money 
		{
			get { return myPlayer.Money; }
		}

		public int EnemyMoney
		{
			get { return enemyPlayer.Money; }
		}

	
		public string[] Researched
		{
			get { return myPlayer.Researched; }
		}

	}
}
