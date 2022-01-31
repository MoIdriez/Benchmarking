using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public class Point
    {
        public int X { get; }
        public int Y { get; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public static Point operator +(Point p, Point p2)
        {
            return new Point(p.X + p2.X, p.Y + p2.Y);
        }

        public static Vector operator -(Point p, Point p2)
        {
            return new Vector(p.X - p2.X, p.Y - p2.Y);
        }

        public static Point operator +(Point p, Vector v)
        {
            return new Point(p.X + v.X.Round(), p.Y + v.Y.Round());
        }

        public override bool Equals(object? obj)
        {
            var p = obj as Point;
            if (p == null) return false;
            return (X == p.X && Y == p.Y);
        }

        public override int GetHashCode()
        {
            return (X << 16) ^ Y;
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
