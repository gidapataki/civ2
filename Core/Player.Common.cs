using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;
using Core.Enums;

namespace Core
{
	public partial class Player
	{
		private int turn;
		private WorldInfo world;
		private List<UnitInfo> myUnits;
		private PlayerInfo myPlayer;
		private List<CityInfo> myCities;
		private CapitalPlaces capitalPosition;

		public string PlayerName
		{
			get { return "Alice"; }
		}

		public string PlayerRace
		{
			get { return "civ1"; }
		}

		public void RefreshWorldInfo(int turn, WorldInfo world)
		{
			this.turn = turn;
			this.world = world;

			myPlayer = world.Players.Single(p => p.Name == PlayerName);
			myUnits = world.Units.Where(p => p.Owner == PlayerName).ToList();
			myCities = world.Cities.Where(p => p.Owner == PlayerName).ToList();

			if (turn == 1)
			{
				if (myCities[0].PositionX == 0 && myCities[0].PositionY == 0)
					capitalPosition = CapitalPlaces.TopLeft;
				else if (myCities[0].PositionX == 0 && myCities[0].PositionY == 19)
					capitalPosition = CapitalPlaces.BottomLeft;
				else if (myCities[0].PositionX == 19 && myCities[0].PositionY == 0)
					capitalPosition = CapitalPlaces.TopRight;
				else if (myCities[0].PositionX == 19 && myCities[0].PositionY == 19)
					capitalPosition = CapitalPlaces.BottomRight;

			}
		}
	}
}
