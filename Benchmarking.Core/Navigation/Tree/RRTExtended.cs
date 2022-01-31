using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Helper;
using Benchmarking.Core.Navigation.Models;

namespace Benchmarking.Core.Navigation.Tree
{
    public class RRTExtended : NavigationalMethod
    {
        private int _step;
        private List<Point> _path = new();
        private readonly RRTSettings _settings;
        private static readonly Random R = new();

        public int NewPlanCount { get; set; }

        public RRTExtended(int[,] map, Robot robot, Point goal, int maxIterations, RRTSettings settings) : base(map, robot, goal, maxIterations)
        {
            _settings = settings;
        }

        protected override void Loop()
        {
            if (!_path.Any() && _path.Count == _step)
                GetNewPlan();

            var nextStep = _path[_step];

            if (ExploredMap.CanStepTo(Robot.Location, nextStep))
            {
                RobotStep(nextStep);
                _step++;
            }
            else
            {
                GetNewPlan();
                RobotStep(Robot.Location);
            }
        }

        protected override string AdditionalMetrics()
        {
            return $",{NewPlanCount},{_settings}";
        }

        private void GetNewPlan()
        {
            NewPlanCount++;
            _path = Plan();
            _step = 0;
        }

        public List<Point> Plan()
        {
            var currentLocation = Robot.Location;
            var nodes = new List<Node> { new(currentLocation, null) };

            while (currentLocation.DistanceTo(Goal) > _settings.GoalDistance || ExploredMap.IsObstructed(currentLocation, Goal))
            {
                (currentLocation, var nt) = ExploredMap.GetRandomLocation(nodes, _settings.GrowthSize, R);

                ExploredMap.RewireNodes(nodes, nt, currentLocation, _settings.GrowthSize);
            }

            var goalNode = new Node(Goal, nodes.Last());
            return goalNode.GenerateResults();
        }
    }
}
