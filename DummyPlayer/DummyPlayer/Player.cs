using CivSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DummyPlayer
{
    public partial class Player : IPlayer
    {
        public MovementData OnMovement()
        {
            var unit = myUnits.FirstOrDefault();

            if (unit != null && unit.MovementPoints > 0)
            {
                var md = new MovementData
                {
                    UnitID = unit.UnitID,
                    FromX = unit.PositionX,
                    FromY = unit.PositionY,
                    ToX = turn % 2 == 0 ? unit.PositionX + 1 : unit.PositionX - 1,
                    ToY = unit.PositionY
                };

                return md;
            }

            return null;
        }

        public ResearchData OnResearch()
        {
            if (myPlayer.Researched.Contains(ResearchTypes.OrzokTornya.GetDescription()))
                return null;
            return new ResearchData { WhatToResearch = ResearchTypes.OrzokTornya.GetDescription() };
        }

        public BuildingData OnBuilding()
        {
            if (myPlayer.Money < 300 || myUnits.Count == 0)
                return null;

            var unit = myUnits.FirstOrDefault();

            if (unit != null)
            {
                var bd = new BuildingData
                {
                    PositionX = unit.PositionX,
                    PositionY = unit.PositionY
                };

                return bd;
            }

            return null;
        }

        public TrainingData OnTraining()
        {
            if (myPlayer.Money < 50)
                return null;

            var city = myCities.First();

            var td = new TrainingData
            {
                PositionX = city.PositionX,
                PositionY = city.PositionY,
                UnitTypeName = UnitTypes.Felderito.GetDescription()
            };

            return td;
        }





        public void ActionResult(WorldInfo world)
        {

        }

        public void CityLost(int positionX, int positionY)
        {

        }

        public void EnemyDestroyed(string playerName)
        {

        }

        public void GameOver(bool winner, string message)
        {

        }

        public void UnitLost(string unitID)
        {

        }
    }
}
