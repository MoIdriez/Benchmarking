using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public class Robot
    {
        public List<Point> Steps;
        public int FovLength;

        public Point Location => Steps.Last();
        public Robot(Point location, int fovLength)
        {
            Steps = new List<Point> { location };
            FovLength = fovLength;
        }

        public void Step(Point p)
        {
            if (Location.DistanceTo(p) > 1.5)
                throw new ArgumentOutOfRangeException(nameof(p), p, $"Location: {Location}, Point: {p}, Distance: {Location.DistanceTo(p)}");

            Steps.Add(p);
        }
    }
}
