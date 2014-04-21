using System;
using System.Linq;
using CivSharp.Common;


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

		public static Position Of(UnitInfo unit)
		{
			return new Position(unit.PositionX, unit.PositionY);
		}

		public static Position Of(CityInfo city)
		{
			return new Position(city.PositionX, city.PositionY);
		}

		public Position Offset(int dx, int dy)
		{
			return new Position(x + dx, y + dy);
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
			return x >= 0 && y >= 0 && x < Constant.BoardSize && y < Constant.BoardSize;
		}
		
		public bool Equals(Position p)
		{
			return p != null && x == p.x && y == p.y;
		}

		public int Distance(Position p)
		{
			return Math.Max(Math.Abs(p.x - x), Math.Abs(p.y - y));
		}

		public int Distance(int px, int py)
		{
			return Math.Max(Math.Abs(px - x), Math.Abs(py - y));
		}

		public int HDistance(Position p)
		{
			return Math.Abs(p.x - x) + Math.Abs(p.y - y);
		}

		public Position Closer(Position p)
		{
			return new Position(x + Math.Sign(p.x - x), y + Math.Sign(p.y - y));
		}

		public int EdgeDistance()
		{
			var c = Constant.BoardSize - 1;
			return (new[] { x, y, c - x, c - y }).Min();
		}

		public int CornerDistance()
		{
			var c = Constant.BoardSize - 1;
			return (new[] { Distance(0, 0), Distance(c, 0), Distance(0, c), Distance(c, c) }).Min();
		}


	}

}
