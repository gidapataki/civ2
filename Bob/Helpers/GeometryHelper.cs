using System;
using System.Collections.Generic;
using System.Drawing;
using DummyPlayer.Enums;

namespace DummyPlayer.Helpers
{
    public static class GeometryHelper
    {
        private static readonly Dictionary<DirectionTypes, Point> directionVectors =
            new Dictionary<DirectionTypes, Point>
                {
                    { DirectionTypes.Up, new Point { X = 0, Y = -1 }},
                    { DirectionTypes.UpRight, new Point { X = 1, Y = -1 }},
                    { DirectionTypes.Right, new Point { X = 1, Y = 0 }},
                    { DirectionTypes.DownRight, new Point { X = 1, Y = 1 }},
                    { DirectionTypes.Down, new Point { X = 0, Y = 1 }},
                    { DirectionTypes.DownLeft, new Point { X = -1, Y = 1 }},
                    { DirectionTypes.Left, new Point { X = -1, Y = 0 }},
                    { DirectionTypes.UpLeft, new Point { X = -1, Y = -1 }},
                };

        public static int TransformX(DirectionTypes direction, CapitalPlaces capitalPosition)
        {
            var vector = directionVectors[direction];
            return vector.X;
        }

        public static int TransformY(DirectionTypes direction, CapitalPlaces capitalPosition)
        {
            var vector = directionVectors[direction];
            return vector.Y;
        }

        public static double CountCityDistance(Point unit, Point destination)
        {
            var xDist = Math.Abs(unit.X - destination.X);
            var yDist = Math.Abs(unit.Y - destination.Y);
            return Math.Max(xDist, yDist);
        }

        public static DirectionTypes GetDirectionByLocationAndDestination(Point location, Point destination)
        {
            int xDistance = location.X - destination.X;
            int yDistance = location.Y - destination.Y;

            if (Math.Abs(xDistance) > 0 && Math.Abs(yDistance) > 0)
            {
                if (xDistance < 0)
                    return yDistance < 0 ? DirectionTypes.DownRight : DirectionTypes.UpRight;
                return yDistance < 0 ? DirectionTypes.DownLeft : DirectionTypes.UpLeft;
            }
            else
            {
                if (xDistance == 0)
                    return yDistance < 0 ? DirectionTypes.Down : DirectionTypes.Up;
                return xDistance < 0 ? DirectionTypes.Right : DirectionTypes.Left;
            }
        }
    }
}

