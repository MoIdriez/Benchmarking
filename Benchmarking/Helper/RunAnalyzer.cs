using System.Diagnostics;
using System.Linq;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation;
using Benchmarking.Core.Navigation.Dijkstra;

namespace Benchmarking.Helper
{
    public class RunAnalyzer
    {
        private readonly NavigationalMethod _nvm;
        
        public RunAnalyzer(NavigationalMethod nvm)
        {
            _nvm = nvm;
        }

        /// <summary>
        /// Gives a value between 0 - 1 where 1 is that it has seen the goal and if not the range from the goal
        /// calculated as shortest path to goal / diameter of the map
        /// </summary>
        public double Success()
        {
            if (_nvm.HasSeenGoal)
                return 1;

            var goalLocation = _nvm.Goal;
            var robotLocation = _nvm.Robot.Location;
            double steps = AStar.Plan(_nvm.Map, robotLocation, goalLocation).Count;

            return steps / MathExt.DiagonalOfRectangle(_nvm.Map.Width(),_nvm.Map.Height());
        }

        /// <summary>
        /// Gives a value between 0 - 1 where 1 is that it took the shortest path and 0 is 5 times that distance
        /// </summary>
        public double StepEfficiency()
        {
            var goalLocation = _nvm.Goal;
            var robotLocation = _nvm.Robot.Steps[0].Location;
            double steps = AStar.Plan(_nvm.Map, robotLocation, goalLocation).Count;

            var v = MathExt.PercentageFromRange(_nvm.Robot.Steps.Count, steps, steps * 5);
            return 1.0 - v;
        }

        /// <summary>
        /// Gives a value between 0 - 1 where 1 is the time it took a single astar on explored map and 0 is ten times that
        /// </summary>
        public double TimeEfficiency()
        {
            var goalLocation = _nvm.Goal;
            var robotLocation = _nvm.Robot.Steps[0].Location;

            var sw = new Stopwatch();
            sw.Start();
            AStar.Plan(_nvm.Map, robotLocation, goalLocation);
            sw.Stop();

            var v = MathExt.PercentageFromRange(_nvm.Time, sw.ElapsedMilliseconds, sw.ElapsedMilliseconds * 40);
            return 1.0 - v;
        }

        /// <summary>
        /// Gives a value between 0 - 1 where 1 is all points explored and 0 is none
        /// </summary>
        public double ExplorationEfficiency()
        {
            double availablePoints = CheckMapFor(MapExt.DefaultValue);
            double exploredPoints = CheckMapFor(MapExt.ExploredPoint);
            var v = MathExt.PercentageFromRange(exploredPoints, 0, availablePoints);
            return v;
        }

        /// <summary>
        /// Gives a value between 0 - 1 where 1 is all steps are redundant
        /// </summary>
        public double StepRedundancy()
        {
            double redundantSteps = _nvm.Robot.Steps.Count(s => _nvm.Robot.Steps.Count(r => Equals(r.Location, s.Location)) > 1);
            return redundantSteps / _nvm.Robot.Steps.Count;
        }

        /// <summary>
        /// Gives a value between 0 - 1 for average visibility where 1 is that the field of vision is always clear (dependent on length of fov)
        /// </summary>
        /// <returns></returns>
        public double StepVisibility()
        {
            var visibility = _nvm.Robot.Steps.Average(s => s.Visibility) / 360;
            var v = MathExt.PercentageFromRange(visibility, 0, _nvm.Robot.FovLength);
            return v;
        }


        private int CheckMapFor(int value)
        {
            var c = 0;
            for (var x = 0; x < _nvm.Map.Width()-1; x++)
            for (var y = 0; y < _nvm.Map.Height() - 1; y++)
                if (_nvm.Map[x, y] == value)
                    c++;
            return c;
        }
    }
}
