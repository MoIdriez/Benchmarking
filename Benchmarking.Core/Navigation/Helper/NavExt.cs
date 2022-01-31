using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Models;

namespace Benchmarking.Core.Navigation.Helper
{
    public static class NavExt
    {
        public static Vector CalculatePotential(int[,] map, PotentialFieldSettings settings, Point robot, Point goal)
        {
            var attractive = robot.Attractive(goal, settings.AttractiveConstant);
            var obstacleRepulsive = new Vector(0, 0);

            for (var x = Math.Max(0, robot.X - settings.ObstacleRange); x < Math.Min(map.Width() - 1, robot.X + settings.ObstacleRange); x++)
            {
                for (var y = Math.Max(0, robot.Y - settings.ObstacleRange); y <= Math.Min(map.Height() - 1, robot.Y + settings.ObstacleRange); y++)
                {
                    if (map[x, y] == MapExt.WallPoint)
                        obstacleRepulsive += robot.Repulsive(new Point(x, y), settings.ObstacleConstant,
                            settings.ObstacleRange);
                }
            }
            return attractive + obstacleRepulsive;
        }

        public static Vector Attractive(this Point robot, Point target, double c)
        {
            var direction = robot.DirectionTo(target);
            var distance = robot.DistanceTo(target);
            return direction * c * (1 / distance);
        }

        public static Vector Repulsive(this Point robot, Point obstacle, double c, double maxRange)
        {
            var direction = obstacle.DirectionTo(robot);
            var distance = obstacle.DistanceTo(robot);

            distance = Math.Abs(distance) < 0.001 ? 1 : distance;

            return direction * c * (maxRange / distance);
        }
        
        public static Point GridPoint(this Vector v)
        {
            var x = v.X > 0.5 ? 1 : v.X < -0.5 ? -1 : 0;
            var y = v.Y > 0.5 ? 1 : v.Y < -0.5 ? -1 : 0;
            return new Point(x, y);
        }

        public static (Point, Node) GetRandomLocation(this int[,] map, List<Node> nodes, int growthSize, Random r)
        {
            var randomLocation = new Point(r.Next(1, map.Width() - 1), r.Next(1, map.Height() - 1));
            var node = nodes.OrderBy(d => randomLocation.DistanceTo(d.Location)).First();

            var bestLocation = map.GetLastUnObstructed(node.Location, randomLocation, growthSize);
            return (bestLocation, node);
        }

        public static void RewireNodes(this int[,] map, List<Node> nodes, Node nt, Point point, int growthSize)
        {
            // get all neighbors
            var neighbors = nodes.Where(n => point.DistanceTo(n.Location) <= growthSize && !map.IsObstructed(point, n.Location));

            // wire point to the lowest cost node in range
            var current = new Node(point, neighbors.OrderBy(n => n.CostFromStart).First());
            nodes.Add(current);

            foreach (var n in neighbors)
            {
                if (current.CostFromStart < (n.Parent?.CostFromStart ?? -1))
                    n.SetNewParent(current);
            }
        }

        public static List<Point> GenerateResults(this Node goalNode)
        {
            var points = goalNode.GetAncestors();
            var result = new List<Point>();
            for (var j = 0; j < points.Count - 1; j++)
            {
                var line = new LineSegment(points[j], points[j + 1]);
                result.AddRange(line.GetPointsOnLine());
            }
            return result;
        }

        public static Point StepTowards(this Point s, Point e)
        {
            return new LineSegment(s, e).GetPointsOnLine()[1];
        }
    }
}
