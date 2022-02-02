using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.ResultsAnalysis
{
    public class RRTAnalysis
    {
        private readonly ITestOutputHelper _output;
        public RRTAnalysis(ITestOutputHelper testOutputHelper)
        {
            _output = testOutputHelper;
        }

        [Fact]
        public void BasicStats()
        {
            var results = TestResult.GetRRT();
            _output.WriteLine("RRT test");

            //results = results.Where(r => r.ObstacleRange <= 10).ToList();
            var success = results.Where(r => r.Success).ToList();
            _output.WriteLine($"Success rate: {success.Count} / {results.Count} ({success.Count.Percentage(results.Count)}%)");

            _output.WriteLine(" ");
            _output.WriteLine("Maps success rates:");
            _output.WriteLine($"Five blocks: \t{success.Count(r => r.Map == TestResult.MapType.Five)} ({success.Count(r => r.Map == TestResult.MapType.Five).Percentage(success.Count)})");
            _output.WriteLine($"Bug Trap: \t\t{success.Count(r => r.Map == TestResult.MapType.Bug)} ({success.Count(r => r.Map == TestResult.MapType.Bug).Percentage(success.Count)})");
            _output.WriteLine($"Tunnel: \t\t{success.Count(r => r.Map == TestResult.MapType.Tunnel)} ({success.Count(r => r.Map == TestResult.MapType.Tunnel).Percentage(success.Count)})");
            _output.WriteLine(" ");

            _output.WriteLine(" ");
            _output.WriteLine("Maps averages:");
            _output.WriteLine($"Five blocks: \tT:{results.Where(r => r.Map == TestResult.MapType.Five).Average(r => r.Time)} " +
                              $"\tS:{results.Where(r => r.Map == TestResult.MapType.Five).Average(r => r.Steps)}" +
                              $"\tP:{results.Where(r => r.Map == TestResult.MapType.Five).Average(r => r.NewPlanCount)}");

            _output.WriteLine($"Bug Trap: \t\tT:{results.Where(r => r.Map == TestResult.MapType.Bug).Average(r => r.Time)} " +
                              $"\tS:{results.Where(r => r.Map == TestResult.MapType.Bug).Average(r => r.Steps)}" +
                              $"\tP:{results.Where(r => r.Map == TestResult.MapType.Bug).Average(r => r.NewPlanCount)}");

            _output.WriteLine($"Tunnel: \t\tT:{results.Where(r => r.Map == TestResult.MapType.Tunnel).Average(r => r.Time)} " +
                              $"\tS:{results.Where(r => r.Map == TestResult.MapType.Tunnel).Average(r => r.Steps)}" +
                              $"\tP:{results.Where(r => r.Map == TestResult.MapType.Tunnel).Average(r => r.NewPlanCount)}");
            _output.WriteLine(" ");
            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Extensive-SuccessResults.txt", success.Select(l => l.Line));


            var scatters = new List<Scatter>();

            foreach (var result in success)
            {
                var i = scatters.FirstOrDefault(i => i.GoalDistance == result.GoalDistance && i.GrowthSize == result.GrowthSize);
                if (i == default)
                {
                    var scatter = new Scatter(result.GrowthSize, result.GoalDistance);
                    scatter.Times.Add(result.Time);
                    scatters.Add(scatter);
                }
                else
                {
                    i.Times.Add(result.Time);
                    i.Counter += 1;
                }
                    
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).ThenBy(s => s.Times.Average()).Take(30))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.GrowthSize},{scatter.GoalDistance} - {scatter.Times.Average()}");
            }
        }


        public class Scatter
        {
            public Scatter(
                //TestResult.MapType map, 
                int growthSize, int goalDistance)
            {
                GoalDistance = goalDistance;
                //Map = map;
                GrowthSize = growthSize;
            }

            //public TestResult.MapType Map { get; }
            public int GrowthSize { get; }
            public int GoalDistance { get; }
            public int Counter { get; set; } = 1;
            public List<int> Times = new List<int>();
        }
    }
}
