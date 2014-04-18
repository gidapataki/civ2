using System;
using System.Drawing;
using CivSharp.Common;
using System.Linq;
using DummyPlayer.Enums;
using DummyPlayer.Helpers;

namespace DummyPlayer
{
	public partial class Player : IPlayer
	{
		private UnitInfo unitToMove;
		private string unitId = String.Empty;
		private bool moved;



		public MovementData OnMovement()
		{
			foreach (var unitToMove in myUnits)
			{
				if (unitToMove.MovementPoints > 0)
				{
					unitId = unitToMove.UnitID;
					Func<int, int> sgn = (int y) => y < 0 ? -1 : (y > 0 ? 1 : 0);
					Func<int, int> converge = (int y) => sgn(10 - y) + y;

					if (converge(unitToMove.PositionY) != unitToMove.PositionY)
					{

						var whereToGo = GeometryHelper.GetDirectionByLocationAndDestination(
										new Point { X = unitToMove.PositionX, Y = unitToMove.PositionY },
										new Point { X = unitToMove.PositionX, Y = converge(unitToMove.PositionY) });

						var md = new MovementData
						{
							UnitID = unitToMove.UnitID,
							FromX = unitToMove.PositionX,
							FromY = unitToMove.PositionY,
							ToX = unitToMove.PositionX + GeometryHelper.TransformX(whereToGo, capitalPosition),
							ToY = unitToMove.PositionY + GeometryHelper.TransformY(whereToGo, capitalPosition)
						};

						unitToMove.PositionX = md.ToX;
						unitToMove.PositionY = md.ToY;
						unitToMove.MovementPoints -= 1;
						return md;
					}
				}
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
			//if (myPlayer.Money < 300 || myUnits.Count == 0)
			//    return null;

			//var unit = myUnits.FirstOrDefault();

			//if (unit != null)
			//{
			//    var bd = new BuildingData
			//    {
			//        PositionX = unit.PositionX,
			//        PositionY = unit.PositionY
			//    };

			//    return bd;
			//}

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
