using System.Diagnostics;
using Benchmarking.Core.Map;

namespace Benchmarking.Core.Navigation
{
    public abstract class NavigationalMethod
    {
        protected NavigationalMethod(int[,] map, Robot robot, Point goal, int maxIterations)
        {
            Map = map;
            Robot = robot;
            Goal = goal;
            MaxIterations = maxIterations;
            ExploredMap = new int[map.Width(), map.Height()];
        }

        public string Run()
        {

            var sw = new Stopwatch();
            sw.Start();

            var i = 0;
            while (!Robot.Location.Equals(Goal) && i < MaxIterations && !IsStuck)
            {
                UpdateExploredMap();
                if (HasSeenGoal)
                {
                    Robot.Step(Robot.Location.StepTowards(Goal));
                }
                else
                {
                    Loop();
                }

                i++;
            }

            Time = sw.ElapsedMilliseconds;
            return Result();
        }

        protected void UpdateExploredMap()
        {
            var fov = Robot.GetFov();

            foreach (var line in fov)
            {
                var points = line.GetPointsOnLine();

                for (var i = 1; i < points.Count; i++)
                {
                    var point = points[i];

                    //within fullMap
                    if (ExploredMap.WithinBound(point))
                    {
                        ExploredMap[point.X, point.Y] = Map[point.X, point.Y];

                        if (ExploredMap[point.X, point.Y] == MapExt.MaxValue)
                            break;

                        if (point.Equals(Goal))
                            HasSeenGoal = true;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        protected abstract void Loop();
        protected abstract string AdditionalMetrics();

        public string Result()
        {
            return $"{HasSeenGoal},{IsStuck},{Time},{Robot.Steps.Count},{MaxIterations}{AdditionalMetrics()}";
        }

        public bool HasSeenGoal;
        public int[,] Map { get; }
        public Point Goal { get; }
        public Robot Robot { get; }
        public long Time { get; set; }

        public int MaxIterations { get; }
        public int[,] ExploredMap { get; }
        public double DistanceFromGoal => Robot.Steps.Last().DistanceTo(Goal);

        public bool IsStuck => Robot.Steps.Count > 40 &&
                               Robot.Steps.TakeLast(40).GroupBy(g => g).Any(g => g.Count() >= 10);
        //public bool IsStuck => Robot.Steps.Count > 10 && Robot.Steps.TakeLast(10).All(s => s.Equals(Robot.Steps.Last()));
    }
}
