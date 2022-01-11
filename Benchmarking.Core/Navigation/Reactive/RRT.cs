using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Helper;
using Benchmarking.Core.Navigation.Models;

namespace Benchmarking.Core.Navigation.Reactive
{
    public class RRT : NavigationalMethod
    {
        private int _step;
        private List<Point> _path = new();
        private readonly RRTSettings _settings;
        private static readonly Random R = new();
        

        public int NewPlanCount { get; set; }

        public RRT(int[,] map, Robot robot, Point goal, int maxIterations, RRTSettings settings) : base(map, robot, goal, maxIterations)
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
                Robot.Step(nextStep);
                _step++;
            }
            else
            {
                GetNewPlan();
                Robot.Step(Robot.Location);
            }
        }

        protected override string AdditionalMetrics()
        {
            return $",{NewPlanCount}";
        }

        private void GetNewPlan()
        {
            NewPlanCount++;
            _path = Plan(ExploredMap, Robot.Location, Goal);
            _step = 0;
        }

        public List<Point> Plan(int[,] exploredMap, Point robotLocation, Point goalCenter)
        {
            var currentNode = new Node(robotLocation, null);
            var nodes = new List<Node> { currentNode };

            while (InValidSolution(currentNode))
            {
                var (randomLocation, node) = GetRandomLocation(nodes);

                currentNode = new Node(randomLocation, node);
                nodes.Add(currentNode);

                if (!(currentNode.Location.DistanceTo(goalCenter) < _settings.GoalDistance) || exploredMap.IsObstructed(currentNode.Location, goalCenter)) continue;
                break;
            }

            var goalNode = new Node(goalCenter, currentNode);
            nodes.Add(goalNode);

            var bestNode= goalNode;

            var points = bestNode.GetAncestors();
            var result = new List<Point>();
            for (var j = 0; j < points.Count - 1; j++)
            {
                var line = new LineSegment(points[j], points[j + 1]);
                result.AddRange(line.GetPointsOnLine());
            }
            return result;
        }

        // get random unobstructed location
        private (Point, Node) GetRandomLocation(List<Node> nodes)
        {
            for (var i = 0; i < 1000; i++)
            {
                var randomLocation = new Point(R.Next(1, ExploredMap.Width() - 1), R.Next(1, ExploredMap.Height() - 1));
                var node = nodes.OrderBy(d => randomLocation.DistanceTo(d.Location)).First();

                if (node.Location.DistanceTo(randomLocation) > _settings.GrowthSize)
                {
                    var angle = node.Location.GetAngleBetween(randomLocation);
                    randomLocation = node.Location.GetEndPoint(angle, _settings.GrowthSize);
                }

                if (!ExploredMap.IsObstructed(node.Location, randomLocation))
                    return (randomLocation, node);
            }

            throw new Exception("Can't generate a random location for some reason");
        }
        
        private bool InValidSolution(Node node)
        {
            return node.Location.DistanceTo(Goal) > _settings.GoalDistance || ExploredMap.IsObstructed(node.Location, Goal);
        }
    }
}
