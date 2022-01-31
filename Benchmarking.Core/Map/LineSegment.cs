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

        public override string ToString()
        {
            return $"{Start}-{End}";
        }
    }
}
