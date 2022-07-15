using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public static class MapExt
    {
        public static int WallPoint = 100;
        public static int DefaultValue = 0;
        public static int ExploredPoint = 1;
        public static int GoalSpawn = 1;
        public static int RobotSpawn = 2;
        public static int Width(this int[,] map) { return map.GetLength(0); }
        public static int Height(this int[,] map) { return map.GetLength(1); }
        public static IEnumerable<LineSegment> GetFov(this Robot robot)
        {
            for (var i = 0; i < 360; i++)
                yield return new LineSegment(robot.Location, i, robot.FovLength);
        }

        public static int[,] SetupMapWithBoundary(int width, int height)
        {
            var map = new int[width, height];

            for (var x = 0; x < width; x++)
            {
                map[x, 0] = WallPoint;
                map[x, height - 1] = WallPoint;
            }
            for (var y = 0; y < height; y++)
            {
                map[0, y] = WallPoint;
                map[width - 1, y] = WallPoint;
            }
            return map;
        }

        public static Point GetRandomLocation(this int[,] map, Random r)
        {
            return map.GetRandomLocation(new Rectangle(10, map.Width(), 10, map.Height()), r);
        }

        public static Point GetRandomLocation(this int[,] map, Rectangle rect, Random r)
        {
            Point p;
            do
            {
                p = new Point(r.Next(rect.MinX, rect.MaxX), r.Next(rect.MinY, rect.MaxY));
            } while (!map.CanMove(p));

            return p;
        }

        public static bool CanMove(this int[,] map, Point p)
        {
            if (!map.WithinBound(p.X, p.Y))
                return false;

            if (map[p.X, p.Y] == WallPoint)
                return false;

            for (var x = p.X - 1; x <= p.X + 1; x++)
            {
                for (var y = p.Y - 1; y <= p.Y + 1; y++)
                {
                    if (map[p.X, p.Y] == WallPoint)
                        return false;
                }
            }

            return true;
        }
        public static List<Point> GetPointsOnLine(this LineSegment l)
        {
            var x1 = l.Start.X;
            var y1 = l.Start.Y;
            var x2 = l.End.X;
            var y2 = l.End.Y;

            var points = new List<Point>();

            // vertical
            if (x2 - x1 == 0)
            {
                var inc = y2 > y1 ? 1 : -1;
                for (var y = y1; y2 > y1 ? y <= y2 : y >= y2; y += inc)
                    if (x1 >= 0 && y >= 0)
                        points.Add(new Point(x1, y));
            }
            else
            {
                // sloop 
                var m = (double)(y2 - y1) / (x2 - x1);
                if (Math.Abs(y2 - y1) > Math.Abs(x2 - x1))
                {
                    var inc = y2 > y1 ? 1 : -1;
                    for (var y = y1; y2 > y1 ? y <= y2 : y >= y2; y += inc)
                    {
                        var nx = ((y - y1) / m) + x1;
                        if (nx >= 0 && y >= 0)
                            points.Add(new Point((int)Math.Round(nx), y));
                    }
                }
                else
                {
                    var inc = x2 > x1 ? 1 : -1;
                    for (var x = x1; x2 > x1 ? x <= x2 : x >= x2; x += inc)
                    {
                        var ny = (x - x1) * m + y1;
                        if (x >= 0 && ny >= 0)
                            points.Add(new Point(x, (int)Math.Round(ny)));
                    }
                }
            }
            return points;

        }
        public static bool IsObstructed(this int[,] map, Point p1, Point p2)
        {
            var line = new LineSegment(p1, p2);
            var points = line.GetPointsOnLine();

            return points.Any(p => map[p.X, p.Y] == WallPoint);
        }

        private static bool IsObstructed(this int[,] map, Point p)
        {
            for (var x = p.X-1; x <= p.X + 1; x++)
            {
                for (var y = p.Y - 1; y <= p.Y + 1; y++)
                {
                    if (map[x, y] == WallPoint)
                        return true;
                }
            }

            return false;
        }

        public static Point GetLastUnObstructed(this int[,] map, Point location, Point randomLocation, int maxDistance)
        {
            var line = new LineSegment(location, randomLocation);
            var points = line.GetPointsOnLine();

            for (var i = 1; i < points.Count; i++)
            {
                if (map.IsObstructed(points[i]) || location.DistanceTo(randomLocation) > maxDistance)
                    return points[i - 1];
            }

            return randomLocation;
        }

        public static List<Point> GetAllTillObstruction(this int[,] map, LineSegment line)
        {
            var points = new List<Point>();

            foreach (var point in line.GetPointsOnLine())
            {
                points.Add(point);
                if (map[point.X, point.Y] == WallPoint)
                    break;
            }

            return points;
        }

        public static bool CanStepTo(this int[,] map, Point start, Point end)
        {
            return map.CanStepTo(start, end.X, end.Y);
        }

        public static bool CanStepTo(this int[,] map, Point start, int ex, int ey)
        {
            return
                WithinBound(map, start) &&
                WithinBound(map, ex, ey) &&
                map[ex, ey] != WallPoint;
        }

        public static bool WithinBound(this int[,] map, Point p)
        {
            return WithinBound(map, p.X, p.Y);
        }

        public static bool WithinBound(this int[,] map, int x, int y)
        {
            return
                0 <= x && x < map.Width() &&
                0 <= y && y < map.Height();
        }
        public static bool InsideObstacle(this Point p, Rectangle[] getFiveObstacles)
        {
            foreach (var obstacle in getFiveObstacles)
            {
                if (obstacle.MinX <= p.X && p.X < obstacle.MaxX && obstacle.MinY <= p.Y && p.Y < obstacle.MaxY)
                    return true;
            }

            return false;
        }

        public static void InverseMap(this int[,] map)
        {
            for (var x = 0; x < map.Width(); x++)
            {
                for (var y = 0; y < map.Height(); y++)
                {
                    map[x, y] = map[x, y] == WallPoint ? 0 : WallPoint;

                }
            }
        }

        public static List<Point> GetSurround(this Point p, int width, int height, bool includePoint = false, int range = 1)
        {
            var minX = Math.Max(0, p.X - range);
            var maxX = Math.Min(width - 1, p.X + range);
            var minY = Math.Max(0, p.Y - range);
            var maxY = Math.Min(height - 1, p.Y + range);


            var points = new List<Point>();

            for (var x = minX; x <= maxX; x++)
            {
                for (var y = minY; y <= maxY; y++)
                {
                    var nb = new Point(x, y);
                    if (includePoint || !p.Equals(nb))
                        points.Add(nb);
                }
            }

            return points;
        }

        public static double Occupation(this int[,] map)
        {
            return map.Cast<int>().Count(i => DefaultValue == i).Percentage(map.Length);
        }
    }
}
