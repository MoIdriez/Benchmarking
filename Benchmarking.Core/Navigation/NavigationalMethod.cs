using System.Diagnostics;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Helper;

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
                //UpdateExploredMap();
                if (HasSeenGoal)
                {
                    RobotStep(_stepsTillGoal[0]);
                    _stepsTillGoal.RemoveAt(0);
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

        protected void RobotStep(Point location)
        {
            var fov = Robot.GetFov().ToList();
            var v = 0;
            var av = 0;

            foreach (var line in fov)
            {
                var points = Map.GetAllTillObstruction(line);
                v += points.Count;

                foreach (var point in points)
                {
                    if (ExploredMap[point.X, point.Y] == MapExt.DefaultValue) av++;

                    ExploredMap[point.X, point.Y] = Map[point.X, point.Y] == MapExt.WallPoint ? MapExt.WallPoint : MapExt.ExploredPoint;
                    if (point.Equals(Goal))
                    {
                        _stepsTillGoal ??= points.GetRange(0, points.IndexOf(point) + 1);
                        HasSeenGoal = true;
                    }
                }
            }
            Robot.Step(location, v);
        }

        private List<Point>? _stepsTillGoal;

        protected abstract void Loop();
        protected abstract string AdditionalMetrics();

        public string Result()
        {
            return $"{HasSeenGoal},{IsStuck},{Time},{Robot.Steps.Count},{AverageVisibility},{AdditionalMetrics()}";
        }

        public bool HasSeenGoal;
        public int[,] Map { get; }
        public Point Goal { get; }
        public Robot Robot { get; }
        public long Time { get; set; }

        public int MaxIterations { get; }
        public int[,] ExploredMap { get; }
        
        public int IsStuckCount { get; set; } = 40;
        public int IsStuckVerifier { get; set; } = 10;

        public double AverageVisibility => Robot.Steps.Average(s => s.Visibility);

        public bool IsStuck => Robot.Steps.Count > IsStuckCount && Robot.Steps.TakeLast(IsStuckCount).GroupBy(g => g).Any(g => g.Count() >= IsStuckVerifier);
        //public bool IsStuck => Robot.Steps.Count > 10 && Robot.Steps.TakeLast(10).All(s => s.Equals(Robot.Steps.Last()));
    }
}
