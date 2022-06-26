using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public class LineSegment
    {
        public Point Start { get; }
        public Point End { get; }

        public LineSegment(Point start, Point end)
        {
            Start = start;
            End = end;
        }
        public LineSegment(Point start, double angle, double length)
        {
            Start = start;
            End = start.GetEndPoint(angle, length);
        }

        public double GetAngleBetween(LineSegment segment)
        {
            var theta1 = Math.Atan2(Start.Y - End.Y, Start.X - End.X);
            var theta2 = Math.Atan2(segment.Start.Y - segment.End.Y, segment.Start.X - segment.End.X);
            var diff = Math.Abs(theta1 - theta2);

            return Math.Min(diff, Math.Abs(180 - diff));
        }

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }
}
