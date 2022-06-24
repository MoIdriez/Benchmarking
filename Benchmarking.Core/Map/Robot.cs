using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public class Robot
    {
        //public List<Point> Steps;
        //public List<int> Visibilities = new();
        public List<RobotStep> Steps;
        public int FovLength;

        public Point Location => Steps.Last().Location;
        public Robot(Point location, int fovLength)
        {
            Steps = new List<RobotStep> { new(location, 0) };
            FovLength = fovLength;
        }

        public void Step(Point p, int v)
        {
            Steps.Add(new RobotStep(p, v));
        }
    }
}
