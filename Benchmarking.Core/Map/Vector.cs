using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public class Vector
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Vector(Point p)
        {
            X = p.X;
            Y = p.Y;
        }

        public Vector(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vector operator -(Vector p1, double number)
        {
            return new Vector(p1.X - number, p1.Y - number);
        }

        public static Vector operator -(Vector p1, Vector p2)
        {
            return new Vector(p1.X - p2.X, p1.Y - p2.Y);
        }

        public static Vector operator +(Vector p1, double number)
        {
            return new Vector(p1.X + number, p1.Y + number);
        }

        public static Vector operator +(Vector p1, Vector p2)
        {
            return new Vector(p1.X + p2.X, p1.Y + p2.Y);
        }

        public static double operator *(Vector p1, Vector p2)
        {
            return p1.X * p2.X + p1.Y * p2.Y;
        }

        public static Vector operator *(Vector p, double number)
        {
            return new Vector(p.X * number, p.Y * number);
        }

        public static Vector operator /(Vector p, double number)
        {
            return new Vector(p.X / number, p.Y / number);
        }

        public override string ToString()
        {
            return $"{X},{Y}";
        }
    }
}
