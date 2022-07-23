using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Helper;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;
using Point = Benchmarking.Core.Map.Point;
using Rectangle = Benchmarking.Core.Map.Rectangle;

namespace Benchmarking.Thesis.FigureGen
{
    public class ChapterOne
    {
        private readonly ITestOutputHelper _output;

        public ChapterOne(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void BrezierTest()
        {
            var psMin = 99.0;
            var iB = 0;
            var jB = 0;
            for (int i = 0; i < 30; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    var bc = CubicBezierToPoints(new Point(12, 57), new Point(12 + i, j), new Point(87 - i, j), new Point(87, 57));
                    var ps = bc.ToArray().ToSegments().ToArray().GetPathSmoothness();
                    if (ps < psMin)
                    {
                        psMin = ps;
                        iB = i;
                        jB = j;
                    }
                }
            }
            _output.WriteLine($"{iB}-{jB}: {psMin}");

        }

        [Fact]
        public void IntroFigure()
        {
            var map = MapExt.SetupMapWithBoundary(100, 70);

            var obstacles = new[]
            {
                new Rectangle(49, 51, 0, 10),
                new Rectangle(49, 51, 30, 69),
            };

            foreach (var obstacle in obstacles)
                map.FillMap(obstacle, MapExt.WallPoint);

            var robot = new Robot(new Point(12, 57), 30);
            var goal = new Point(87, 57);

            var astar = AStar.Plan(map, robot.Location, goal);
            var redLine = new LineSegment(robot.Location, goal).GetPointsOnLine();

            var wallFollower = WallFollower(robot.Location, goal);
            var pathSmooth = PathSmooth(robot.Location, goal);
            var brezierCurve = CubicBezierToPoints(robot.Location, new Point(robot.Location.X + 10, 20), new Point(goal.X - 10, 20), goal);

            ////////////////////////////////////////////////////
            var image = new Bitmap(map.Width(), map.Height());
            var grf = Graphics.FromImage(image);
            for (var x = 0; x < map.Width(); x++)
            {
                for (var y = 0; y < map.Height(); y++)
                {
                    if (map[x, y] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    if (map[x, y] == MapExt.DefaultValue)
                        image.SetPixel(x, y, Color.White);
                }
            }

            foreach (var point in astar.Skip(3))
            {
                image.SetPixel(point.X, point.Y, Color.Aqua);
                //grf.FillEllipse(new SolidBrush(Color.Aqua), point.X, point.Y, 2, 2);
            }

            foreach (var point in redLine.Skip(3))
            {
                if (map[point.X, point.Y] != MapExt.WallPoint)
                    image.SetPixel(point.X, point.Y, Color.Crimson);
                //grf.FillEllipse(new SolidBrush(Color.Aqua), point.X, point.Y, 2, 2);
            }

            foreach (var point in wallFollower)//.Skip(3))
            {
                image.SetPixel(point.X, point.Y, Color.DarkViolet);
            }

            foreach (var point in pathSmooth)//.Skip(3))
            {
                image.SetPixel(point.X, point.Y, Color.Chocolate);
            }

            //DrawBezierCurve(new Pen(Color.Chocolate),)

            //grf.FillEllipse(new SolidBrush(Color.LightGreen), goal.X - 2, goal.Y - 2, 7, 7);
            grf.FillEllipse(new SolidBrush(Color.LightBlue), robot.Location.X - 2, robot.Location.Y - 2, 7, 7);
            var goalPoints = MarkGoals(goal);
            foreach (var point in goalPoints)//.Skip(3))
            {
                image.SetPixel(point.X, point.Y, Color.LightGreen);
            }
            image.Save($"Images/ChapterOne-{Guid.NewGuid()}.png", ImageFormat.Png);

            _output.WriteLine($"Astar: {astar.ToArray().ToSegments().ToArray().GetPathSmoothness()}");
            _output.WriteLine($"Brezier: {brezierCurve.ToArray().ToSegments().ToArray().GetPathSmoothness()}");
            _output.WriteLine($"PathSmooth: {pathSmooth.ToArray().ToSegments().ToArray().GetPathSmoothness()}");

        }

        internal void DrawBezierCurve(Pen pen, PaintEventArgs e, Point robot, Point goal)
        {
            var LinePath = new GraphicsPath();
            var p1 = new System.Drawing.Point(robot.X, robot.Y);
            var p2 = new System.Drawing.Point(robot.X, 10); //first control point
            var p3 = new System.Drawing.Point(goal.X, 10); //second control point
            var p4 = new System.Drawing.Point(goal.X, goal.Y); //second control point
            LinePath.AddBezier(p1, p2, p3, p4);
            e.Graphics.DrawPath(pen, LinePath);
        }

        public List<Point> CubicBezierToPoints(Point P0, Point P1, Point P2, Point P3, double step = 0.01)
        {
            var pointList = new List<Point>();
            for (var t = 0.00; t <= 1; t += step)
            {
                var x = Math.Pow(1 - t, 3) * P0.X + 3 * Math.Pow(1 - t, 2) * t * P1.X +
                        3 * (1 - t) * Math.Pow(t, 2) * P2.X + Math.Pow(t, 3) * P3.X;
                var y = Math.Pow(1 - t, 3) * P0.Y + 3 * Math.Pow(1 - t, 2) * t * P1.Y +
                        3 * (1 - t) * Math.Pow(t, 2) * P2.Y + Math.Pow(t, 3) * P3.Y;
                pointList.Add(new Point((int)x, (int)y));
            }
            return pointList;
        }

        private List<Point> PathSmooth(Point robot, Point goal)
        {
            var line1 = new LineSegment(robot, new Point(50, 20));
            var line2 = new LineSegment(new Point(50, 20), goal);
            var lines = new List<LineSegment> { line1, line2 };
            return lines.SelectMany(l => l.GetPointsOnLine()).ToList();
        }

        private List<Point> WallFollower(Point robot, Point goal)
        {
            var line1 = new LineSegment(robot, new Point(4, robot.Y));
            var line2 = new LineSegment(line1.End, new Point(4, 4));
            var line3 = new LineSegment(line2.End, new Point(46, 4));
            var line4 = new LineSegment(line3.End, new Point(46, 14));
            var line5 = new LineSegment(line4.End, new Point(54, 14));
            var line6 = new LineSegment(line5.End, new Point(54, 4));
            var line7 = new LineSegment(line6.End, new Point(96, 4));
            var line8 = new LineSegment(line7.End, new Point(96, 57));
            var line9 = new LineSegment(line8.End, goal);

            var lines = new List<LineSegment> { line1, line2, line3, line4, line5, line6, line7, line8, line9 };
            return lines.SelectMany(l => l.GetPointsOnLine()).ToList();
        }


        private IEnumerable<Point> MarkGoals(Point goal)
        {
            for (var x = goal.X - 3; x < goal.X + 3; x++)
            {
                for (var y = goal.Y - 3; y < goal.Y + 3; y++)
                {
                    if ((x + y) % 2 == 0)
                        yield return new Point(x,y);
                }
            }
        }
    }
}
