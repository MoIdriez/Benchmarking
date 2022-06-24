using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Thesis.ChapterThree.Data;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class BasicPotentialFieldAnalysis
    {
        private readonly ITestOutputHelper _output;
        public BasicPotentialFieldAnalysis(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void WallParameterExplorationAnalysis()
        {
            _output.WriteLine("WallOne Parameter Exploration Analysis");

            var wallOne = File.ReadAllLines(FileRef.WallOneParameterExploration).Select(l => { var items = l.Split(","); return new { HasSeenGoal = int.Parse(items[0]), IsStuck = int.Parse(items[1]), Time = int.Parse(items[2]), Steps = int.Parse(items[3]), ObstacleRange = int.Parse(items[4]), ObstacleConstant = int.Parse(items[5]), AttractiveConstant = int.Parse(items[6]),}; }).ToList();
            var wallTwo = File.ReadAllLines(FileRef.WallTwoParameterExploration).Select(l => { var items = l.Split(","); return new { HasSeenGoal = int.Parse(items[0]), IsStuck = int.Parse(items[1]), Time = int.Parse(items[2]), Steps = int.Parse(items[3]), ObstacleRange = int.Parse(items[4]), ObstacleConstant = int.Parse(items[5]), AttractiveConstant = int.Parse(items[6]),}; }).ToList();
            var wallThree = File.ReadAllLines(FileRef.WallThreeParameterExploration).Select(l => { var items = l.Split(","); return new { HasSeenGoal = int.Parse(items[0]), IsStuck = int.Parse(items[1]), Time = int.Parse(items[2]), Steps = int.Parse(items[3]), ObstacleRange = int.Parse(items[4]), ObstacleConstant = int.Parse(items[5]), AttractiveConstant = int.Parse(items[6]),}; }).ToList();

            _output.WriteLine($"Success WallOne rate: {wallOne.Count(r => r.HasSeenGoal == 1)} / {wallOne.Count} ({wallOne.Count(r => r.HasSeenGoal == 1).Percentage(wallOne.Count)}%)");
            _output.WriteLine($"Success WallTwo rate: {wallTwo.Count(r => r.HasSeenGoal == 1)} / {wallTwo.Count} ({wallTwo.Count(r => r.HasSeenGoal == 1).Percentage(wallTwo.Count)}%)");
            _output.WriteLine($"Success WallThree rate: {wallThree.Count(r => r.HasSeenGoal == 1)} / {wallThree.Count} ({wallThree.Count(r => r.HasSeenGoal == 1).Percentage(wallThree.Count)}%)");

            _output.WriteLine($"==========================================================");

            var scatters = new List<Scatter>();

            foreach (var result in wallOne.Where(w => w.HasSeenGoal == 1))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(10))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant}");
            }
            _output.WriteLine($"==========================================================");

            File.WriteAllLines(@"WallOneAnalysis.txt", scatters.Select(l => l.Line));

            scatters = new List<Scatter>();

            foreach (var result in wallTwo.Where(w => w.HasSeenGoal == 1))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(10))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant}");
            }

            File.WriteAllLines(@"WallTwoAnalysis.txt", scatters.Select(l => l.Line));

            _output.WriteLine($"==========================================================");

            scatters = new List<Scatter>();

            foreach (var result in wallThree.Where(w => w.HasSeenGoal == 1))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(10))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant}");
            }

            File.WriteAllLines(@"WallThreeAnalysis.txt", scatters.Select(l => l.Line));
        }

        public class Scatter
        {
            public Scatter(int obstacleRange, int obstacleConstant, int attractiveConstant)
            {
                ObstacleRange = obstacleRange;
                ObstacleConstant = obstacleConstant;
                AttractiveConstant = attractiveConstant;
            }
            public int ObstacleRange { get; set; }
            public int ObstacleConstant { get; set; }
            public int AttractiveConstant { get; set; }
            public int Counter { get; set; } = 1;

            public virtual string Line => $"{ObstacleRange},{ObstacleConstant},{AttractiveConstant},{Counter}";
        }

    }
}
