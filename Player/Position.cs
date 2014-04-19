using System;
using CivSharp.Common;
using CivPlayer.Enums;
using CivPlayer.Helpers;


namespace CivPlayer
{

	public class Position
	{
		public readonly int x;
		public readonly int y;

		public Position(int x, int y) 
		{ 
			this.x = x;
			this.y = y;
		}

		public Position(UnitInfo unit)
		{
			this.x = unit.PositionX;
			this.y = unit.PositionY;
		}

		public Position(CityInfo city)
		{
			this.x = city.PositionX;
			this.y = city.PositionY;
		}

		public Position Offset(Position p)
		{
			return new Position(x + p.x, y + p.y);
		}

		public bool IsNeighbour(Position p)
		{
			return (x != p.x || y != p.y) && Math.Abs(p.x - x) <= 1 && Math.Abs(p.y - y) <= 1;
		}

		public bool IsValid()
		{
			return x >= 0 && y >= 0 && x < 20 && y <= 20;
		}

		public bool Equals(Position p)
		{
			return (object) p != null && x == p.x && y == p.y;
		}
	}

}
