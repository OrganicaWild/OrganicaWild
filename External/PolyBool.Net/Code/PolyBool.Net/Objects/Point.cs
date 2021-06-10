using Tektosyne.Geometry;
using UnityEngine;

namespace Polybool.Net.Objects
{
    public class Point
    {
        public Point(decimal x, decimal y)
        {
            X = x;
            Y = y;
        }

        public decimal X { get; set; }
        public decimal Y { get; set; }

        public bool Equals(Point other)
        {
            return other.X == X && other.Y == Y;
        }

        //vector2 to point implicit conversion
        public static implicit operator Point(Vector2 value)
        {
            return new Point((decimal) value.x, (decimal) value.y);
        }

        //point to vector2 implicit conversion
        public static implicit operator Vector2(Point value)
        {
            return new Vector2((float) value.X, (float) value.Y);
        }

        public static implicit operator Point(PointD pointD)
        {
            return new Point((decimal) pointD.X, (decimal) pointD.Y);
        }

        public static implicit operator PointD(Point point)
        {
            return new PointD((double) point.X, (double) point.Y);
        }
    }
}