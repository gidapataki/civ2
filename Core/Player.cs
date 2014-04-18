using System;
using System.Drawing;
using CivSharp.Common;
using System.Linq;
using Core.Enums;
using Core.Helpers;

namespace Core
{
	public partial class Player : IPlayer
	{
		private UnitInfo unitToMove;
		private string unitId = String.Empty;
		private bool moved;

		public MovementData OnMovement()
		{
			if (moved)
			{
				moved = false;
				return null;
			}

			if (unitId == String.Empty)
				unitToMove = myUnits.FirstOrDefault();
			else
				unitToMove = myUnits.SingleOrDefault(p => p.UnitID == unitId);

			if (unitToMove != null && unitToMove.MovementPoints > 0)
			{
				unitId = unitToMove.UnitID;

				var whereToGo = turn % 2 == 0 ?
							GeometryHelper.GetDirectionByLocationAndDestination(
								new Point { X = unitToMove.PositionX, Y = unitToMove.PositionY },
								new Point { X = unitToMove.PositionX + 1, Y = unitToMove.PositionY })
							: GeometryHelper.GetDirectionByLocationAndDestination(
								new Point { X = unitToMove.PositionX, Y = unitToMove.PositionY },
								new Point { X = unitToMove.PositionX - 1, Y = unitToMove.PositionY });

				var md = new MovementData
				{
					UnitID = unitToMove.UnitID,
					FromX = unitToMove.PositionX,
					FromY = unitToMove.PositionY,
					ToX = unitToMove.PositionX + GeometryHelper.TransformX(whereToGo, capitalPosition),
					ToY = unitToMove.PositionY + GeometryHelper.TransformY(whereToGo, capitalPosition)
				};

				moved = true;
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
