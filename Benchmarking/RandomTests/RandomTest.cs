using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Thesis.ChapterFour.Data;
using Benchmarking.Thesis.ChapterThree;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.RandomTests
{
    public class RandomTest
    {
        private readonly ITestOutputHelper _output;

        public RandomTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void PathSmoothnessCheck()
        {
            var data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();
            var ps = data.Where(d => !double.IsNaN(d.PathSmoothness));
            _output.WriteLine($"Min: {ps.Min(d => d.PathSmoothness):F}");
            _output.WriteLine($"25: {ps.Select(d => d.PathSmoothness).Percentile(0.25):F}");
            _output.WriteLine($"50: {ps.Select(d => d.PathSmoothness).Percentile(0.50):F}");
            _output.WriteLine($"75: {ps.Select(d => d.PathSmoothness).Percentile(0.75):F}");
            _output.WriteLine($"100: {ps.Select(d => d.PathSmoothness).Percentile(1):F}");
            _output.WriteLine($"Max: {ps.Max(d => d.PathSmoothness):F}");


            _output.WriteLine($"{data.Count(d => d.Success && d.Approach == Evaluate.ApproachType.PotentialField && !Evaluate.IsStaticMap(d.Map))} / {data.Count(d => d.Approach == Evaluate.ApproachType.PotentialField && !Evaluate.IsStaticMap(d.Map))}");
            _output.WriteLine($"{data.Count(d => d.Success && d.Approach == Evaluate.ApproachType.PotentialField && !Evaluate.IsStaticMap(d.Map)).Percentage(data.Count(d => d.Approach == Evaluate.ApproachType.PotentialField && !Evaluate.IsStaticMap(d.Map)))}");
        }

        [Fact]
        public void RangeChecks()
        {
            var data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();

            var path = new List<(double bs, double mn, double mx)>();
            var duration = new List<(double bs, double mn, double mx)>();
            var tc = 0;
            var pc = 0;
            foreach (var bs in data.Where(d => d.Approach == Evaluate.ApproachType.BaseLine))
            {
                var relevantRuns = data.Where(d => bs.Id == d.Id && bs.Map == d.Map && d.Success && d.Approach != Evaluate.ApproachType.BaseLine).ToList();

                var times = new { BaselineTime = bs.Time, Min = relevantRuns.Any() ? relevantRuns.Min(r => r.Time) : bs.Time, Max = relevantRuns.Any() ? relevantRuns.Max(r => r.Time) : bs.Time };
                var steps = new { BaseLineSteps = bs.Steps, Min = relevantRuns.Any() ? relevantRuns.Min(r => r.Steps) : bs.Steps, Max = relevantRuns.Any() ? relevantRuns.Max(r => r.Steps) : bs.Steps };
                path.Add((steps.BaseLineSteps, steps.Min, steps.Max));
                duration.Add((times.BaselineTime, times.Min, times.Max));

                if (times.Min < times.BaselineTime)
                {
                    _output.WriteLine($"Faster Time: \t{bs.Id}-{bs.Map} \t= {relevantRuns.First(r => Math.Abs(r.Time - times.Min) < 0.1).Approach}\t{times.Min} < {bs.Time}");
                    tc++;
                }


                if (steps.Min < steps.BaseLineSteps)
                {
                    pc++;
                    _output.WriteLine($"Shorter Step: \t{bs.Id}-{bs.Map} \t= {relevantRuns.First(r => Math.Abs(r.Steps - steps.Min) < 0.1).Approach}\t{steps.Min} < {bs.Steps}");
                }
            }

            _output.WriteLine("==========================================================");
            _output.WriteLine($"Better than baseline: {tc+pc} / {data.Count*2} || PC:{pc} - DC:{tc}");
            _output.WriteLine($"Largest path multiplier: {path.Max(p => p.mx / p.bs)}");
            _output.WriteLine($"Largest duration multiplier: {duration.Max(p => p.mx / p.bs)}");

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
            _output.WriteLine($"Full: {v / (double)fov.Count}");

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
                foreach (var point in points)
                {
                    _output.WriteLine($"{point}");

                }
                
                v += points.Count;
            }
            _output.WriteLine($"None: {v / (double)fov.Count}");
        }

        [Fact]
        public void PfTests()
        {
            var p1 = new Point(1, 1);
            var p2 = new Point(1, 2);
            _output.WriteLine(p1.DirectionTo(p2).ToString());
            _output.WriteLine(p1.DistanceTo(p2).ToString());
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
            _output.WriteLine($"Smoothness: {MathExt.GetPathSmoothness(lineSegments)}");
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
            _output.WriteLine($"Smoothness: {MathExt.GetPathSmoothness(lineSegments)}");
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
