using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public static class MathExt
    {
        public static int ToInt(this double v)
        {
            return Convert.ToInt32(v);
        }
        public static int Round(this double v)
        {
            return (int)Math.Round(v);
        }
        public static double ToRad(this double degree)
        {
            return degree * Math.PI / 180;
        }

        public static double ToDeg(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static Vector DirectionTo(this Point start, Point end)
        {
            return new Vector(end.X - start.X, end.Y - start.Y);
        }

        public static double DistanceTo(this Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static double GetAngleBetween(this Point p1, Point p2)
        {
            return Math.Atan2(p1.Y - p2.Y, p1.X - p2.X).ToDeg();
        }

        public static Point GetEndPoint(this Point p, double angle, double length)
        {
            return new Point(
                (p.X + length * Math.Cos(angle.ToRad())).ToInt(),
                (p.Y + length * Math.Sin(angle.ToRad())).ToInt()
            );
        }

        public static Point StepTowards(this Point p, Point t)
        {
            var x = Math.Abs(p.X - t.X) < 1 ? p.X : p.X > t.X ? p.X - 1 : p.X + 1;
            var y = Math.Abs(p.Y - t.Y) < 1 ? p.Y : p.Y > t.Y ? p.Y - 1 : p.Y + 1;
            return new Point(x, y);
        }
    }
}
