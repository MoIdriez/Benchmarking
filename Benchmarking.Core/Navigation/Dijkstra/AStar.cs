using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Models;

namespace Benchmarking.Core.Navigation.Dijkstra
{
    public class AStar : NavigationalMethod
    {
        private int _step;
        private List<Point> _path = new();
        public int NewPlanCount { get; set; }
        public AStar(int[,] fullMap, Robot robot, Point goal, int maxIterations) : base(fullMap, robot, goal, maxIterations) { }

        protected override void Loop()
        {
            if (!_path.Any())
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
            return $",{NewPlanCount}";
        }

        private void GetNewPlan()
        {
            NewPlanCount++;
            _path = Plan(ExploredMap, Robot.Location, Goal);
            _step = 0;
        }

        public static List<Point> Plan(int[,] map, Point start, Point goal)
        {
            var open = new Dictionary<int, Node>();

            var visited = new double[map.Width(), map.Height()];

            open.Add(start.GetHashCode(), new Node(start, null, 0, 0));

            while (open.Count > 0)
            {
                var current = GetFirst(open);

                open.Remove(current.GetHashCode());

                var successors = GenerateSuccessors(map, current, goal);

                foreach (var successor in successors)
                {
                    if (successor.Location.Equals(goal))
                        return GetPlan(successor);

                    var visitedValue = visited[successor.Location.X, successor.Location.Y];

                    // if it has been visited and the cost of it is lower
                    if (visitedValue > 0.5 && visitedValue <= successor.Cost)
                        continue;

                    if (open.ContainsKey(successor.Location.GetHashCode()))
                    {
                        if (open[successor.Location.GetHashCode()].Cost > successor.Cost)
                        {
                            open[successor.Location.GetHashCode()] = successor;
                        }
                    }
                    else
                    {
                        open.Add(successor.Location.GetHashCode(), successor);
                    }
                }

                visited[current.Location.X, current.Location.Y] = current.Cost;
            }

            throw new Exception("Couldn't find a path");
        }

        private static Node GetFirst(Dictionary<int, Node> nodes)
        {
            return nodes.OrderBy(n => n.Value.Cost).First().Value;
        }

        private static List<Point> GetPlan(Node successor)
        {
            var points = Node.GetNextSteps(successor).ToList();
            points.Reverse();
            return points;
        }

        private static List<Node> GenerateSuccessors(int[,] map, Node current, Point goal)
        {
            var results = new List<Node>();
            for (var x = current.Location.X - 1; x <= current.Location.X + 1; x++)
            {
                for (var y = current.Location.Y - 1; y <= current.Location.Y + 1; y++)
                {
                    if (current.Location.X == x && current.Location.Y == y)
                        continue;

                    if (!map.CanStepTo(current.Location, x, y))
                        continue;

                    var successor = new Point(x, y);
                    var costFromStart = CalculateStartCost(current, successor);
                    var costTillEnd = CalculateEndCost(successor, goal);
                    results.Add(new Node(successor, current, costFromStart, costTillEnd));
                }
            }

            return results;
        }


        private static double CalculateStartCost(Node current, Point successor)
        {
            return current.CostFromStart + current.Location.DistanceTo(successor);
        }

        private static double CalculateEndCost(Point successor, Point goal)
        {
            return successor.DistanceTo(goal);
        }
    }
}
