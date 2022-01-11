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
        public static Vector CalculatePotential(int[,] map, PfSettings settings, Point robot, Point goal)
        {
            var attractive = robot.Attractive(goal, settings.AttractiveConstant);
            var obstacleRepulsive = new Vector(0, 0);

            for (var x = Math.Max(0, robot.X - settings.ObstacleRange); x < Math.Min(map.Width() - 1, robot.X + settings.ObstacleRange); x++)
            {
                for (var y = Math.Max(0, robot.Y - settings.ObstacleRange); y <= Math.Min(map.Height() - 1, robot.Y + settings.ObstacleRange); y++)
                {
                    if (map[x, y] == MapExt.MaxValue)
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

        public static void RewireNodes(this int[,] map, List<Node> nodes, Node current, int range)
        {
            var inRange = nodes.Where(n =>
                n.Location.DistanceTo(current.Location) < range
                && !map.IsObstructed(n.Location, current.Location)
            );
            foreach (var n in inRange)
            {
                if (current.CostFromStart + current.Location.DistanceTo(n.Location) < n.CostFromStart)
                    n.SetNewParent(current);
            }
        }

        public static Point GridPoint(this Vector v)
        {
            var x = v.X > 0.5 ? 1 : v.X < -0.5 ? -1 : 0;
            var y = v.Y > 0.5 ? 1 : v.Y < -0.5 ? -1 : 0;
            return new Point(x, y);
        }
    }
}
