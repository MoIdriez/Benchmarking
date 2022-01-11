﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public static class MapExt
    {
        public static int MaxValue = 100;
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
                map[x, 0] = MaxValue;
                map[x, height - 1] = MaxValue;
            }
            for (var y = 0; y < height; y++)
            {
                map[0, y] = MaxValue;
                map[width - 1, y] = MaxValue;
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

            if (map[p.X, p.Y] == MaxValue)
                return false;

            for (var x = p.X - 1; x <= p.X + 1; x++)
            {
                for (var y = p.Y - 1; y <= p.Y + 1; y++)
                {
                    if (map[p.X, p.Y] == MaxValue)
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

            return points.Any(p => map[p.X, p.Y] == MaxValue);
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
                map[ex, ey] < MaxValue;
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
    }
}