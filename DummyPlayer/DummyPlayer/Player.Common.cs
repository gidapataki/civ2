using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CivSharp.Common;

namespace DummyPlayer
{
    public partial class Player
    {
        private int turn;
        private WorldInfo world;
        private List<UnitInfo> myUnits;
        private PlayerInfo myPlayer;
        private List<CityInfo> myCities; 

        public string PlayerName
        {
            get { return "omelette du fromage 2"; }
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
        }
    }
}
