using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.RandomTests
{
    public class RandomTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RandomTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void VisibilityCheck()
        {
            // full visibility
            var map = new int[200, 200];
            var robot = new Robot(new Point(100, 100), 30);

            var fov = robot.GetFov().ToList();
            var v = 0;

            foreach (var line in fov)
            {
                var points = map.GetAllTillObstruction(line);
                v += points.Count;
            }
            _testOutputHelper.WriteLine($"Full: {v / (double)fov.Count}");

            var objects = new List<Point>
            {
                new Point(99, 99), new Point(100, 99), new Point(101, 99),
                new Point(99, 100), new Point(101, 100),
                new Point(99, 101), new Point(100, 101), new Point(101, 101)
            };

            foreach (var point in objects) map[point.X, point.Y] = MapExt.WallPoint;

            v = 0;
            foreach (var line in fov)
            {
                var points = map.GetAllTillObstruction(line);
                v += points.Count;
            }
            _testOutputHelper.WriteLine($"None: {v / (double)fov.Count}");
        }

        [Fact]
        public void PfTests()
        {
            var p1 = new Point(1, 1);
            var p2 = new Point(1, 2);
            _testOutputHelper.WriteLine(p1.DirectionTo(p2).ToString());
            _testOutputHelper.WriteLine(p1.DistanceTo(p2).ToString());
        }

        [Fact]
        public void PathDivs()
        {
            var pts = new[]
            {
                new Point(0, 0),
                new Point(1, 1),
                new Point(2, 2),
                new Point(3, 3),
            };

            var lineSegments = MathExt.ToSegments(pts).ToArray();
            _testOutputHelper.WriteLine($"Smoothness: {MathExt.GetPathSmoothness(lineSegments)}");
        }

        [Fact]
        public void PathDivs2()
        {
            var pts = new[]
            {
                new Point(0, 0),
                new Point(1, 1),
                new Point(1, 2),
                new Point(0, 3),
                new Point(1, 4),
            };

            var lineSegments = MathExt.ToSegments(pts).ToArray();
            _testOutputHelper.WriteLine($"Smoothness: {MathExt.GetPathSmoothness(lineSegments)}");
        }

        [Fact]
        public void IsStuckTester()
        {
            var steps = new List<Point>
            {
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
                new(111, 76),
                new(110, 76),
            };
            var groupBy = steps.TakeLast(20).GroupBy(g => g).ToList();
            var isStuck = groupBy.Any(g => g.Count() >= 8);
            Assert.True(isStuck);
        }
    }
}
