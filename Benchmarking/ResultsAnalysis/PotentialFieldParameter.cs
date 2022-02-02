using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.ResultsAnalysis
{
    public class PotentialFieldParameter
    {
        private readonly ITestOutputHelper _output;

        public PotentialFieldParameter(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// The parameter range explored in this test:
        /// obstacle range 1-40
        /// obstacle constant 1-20
        /// attractive constant 1-20
        ///
        /// per configuration of settings we created 100 trials
        /// each trial involved three maps; tunnel, bugTrap, and fiveBlock
        ///
        /// resulting in 4,680,000 total trials
        /// </summary>
        [Fact]
        public void BasicStats()
        {
            var results = TestResult.GetPotentialFieldExtensive();
            _output.WriteLine("Potential Field Extensive test");

            //results = results.Where(r => r.ObstacleRange <= 10).ToList();
            var success = results.Where(r => r.Success).ToList();
            _output.WriteLine($"Success rate: {success.Count} / {results.Count} ({success.Count.Percentage(results.Count)}%)");

            _output.WriteLine(" ");
            _output.WriteLine("Maps success rates:");
            _output.WriteLine($"Five blocks: \t{success.Count(r => r.Map == TestResult.MapType.Five)} ({success.Count(r => r.Map == TestResult.MapType.Five).Percentage(success.Count)})");
            _output.WriteLine($"Bug Trap: \t\t{success.Count(r => r.Map == TestResult.MapType.Bug)} ({success.Count(r => r.Map == TestResult.MapType.Bug).Percentage(success.Count)})");
            _output.WriteLine($"Tunnel: \t\t{success.Count(r => r.Map == TestResult.MapType.Tunnel)} ({success.Count(r => r.Map == TestResult.MapType.Tunnel).Percentage(success.Count)})");

            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Extensive-SuccessResults.txt", success.Select(l => l.Line));
        }

        [Fact]
        public void ScatterGen()
        {
            var results =
                TestResult.Read(
                    @"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Extensive-SuccessResults.txt");

            var scatters = new List<Scatter>();

            foreach (var result in results)//.Where(r => r.Map == TestResult.MapType.Five))
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
            

            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Extensive-FiveScatter.txt", scatters.Select(l => l.Line));
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
