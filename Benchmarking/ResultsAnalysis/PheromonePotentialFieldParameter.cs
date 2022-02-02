using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.ResultsAnalysis
{
    public class PheromonePotentialFieldParameter
    {
        private readonly ITestOutputHelper _output;

        public PheromonePotentialFieldParameter(ITestOutputHelper output)
        {
            _output = output;
        }

        /// <summary>
        /// The parameter range explored in this test:
        /// obstacle range      1-10
        /// obstacle constant   1-10
        /// attractive constant 1-10
        ///
        /// pheromone constant  1-10
        /// pheromone range     1-10
        /// strength increase   1-10
        ///
        /// per configuration of settings we created 10 trials
        /// each trial involved three maps; tunnel, bugTrap, and fiveBlock
        ///
        /// resulting in 468,700 total trials
        /// </summary>
        [Fact]
        public void BasicStats()
        {
            var results = TestResult.GetPheromonePotentialFieldExtensive();
            _output.WriteLine("Pheromone Field Extensive test");

            var success = results.Where(r => r.Success).ToList();
            _output.WriteLine($"Success rate: {success.Count} / {results.Count} ({success.Count.Percentage(results.Count)}%)");

            _output.WriteLine(" ");
            _output.WriteLine("Maps success rates:");
            _output.WriteLine($"Five blocks: \t{success.Count(r => r.Map == TestResult.MapType.Five)} ({success.Count(r => r.Map == TestResult.MapType.Five).Percentage(success.Count)})");
            _output.WriteLine($"Bug Trap: \t\t{success.Count(r => r.Map == TestResult.MapType.Bug)} ({success.Count(r => r.Map == TestResult.MapType.Bug).Percentage(success.Count)})");
            _output.WriteLine($"Tunnel: \t\t{success.Count(r => r.Map == TestResult.MapType.Tunnel)} ({success.Count(r => r.Map == TestResult.MapType.Tunnel).Percentage(success.Count)})");

            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PheromonePotentialFieldParameter\PheromonePotentialField-Extensive-SuccessResults.txt", success.Select(l => l.Line));

            var scatters = new List<Scatter>();

            foreach (var result in success)
            {
                var i = scatters.FirstOrDefault(i => 
                    i.ObstacleRange == result.ObstacleRange 
                    && i.ObstacleConstant == result.ObstacleConstant 
                    && i.AttractiveConstant == result.AttractiveConstant
                    && Math.Abs(i.Constant - result.Constant) < 0.0001
                    && Math.Abs(i.Si - result.StrengthIncrease) < 0.0001
                    && i.R == result.Range
                    );
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant, result.Constant, result.StrengthIncrease, result.Range, result.Map));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(30))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant},{scatter.Constant},{scatter.Si},{scatter.R}");
            }

            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PheromonePotentialFieldParameter\PheromonePotentialField-Extensive-AllScatter.txt", scatters.Select(l => l.Line));
            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PheromonePotentialFieldParameter\PheromonePotentialField-Extensive-FiveScatter.txt", scatters.Where(m => m.Map == TestResult.MapType.Five).Select(l => l.Line));
            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PheromonePotentialFieldParameter\PheromonePotentialField-Extensive-BugScatter.txt", scatters.Where(m => m.Map == TestResult.MapType.Bug).Select(l => l.Line));
            //File.WriteAllLines(@"D:\Research\Repos\Benchmarking\Benchmarking\Results\PheromonePotentialFieldParameter\PheromonePotentialField-Extensive-TunnelScatter.txt", scatters.Where(m => m.Map == TestResult.MapType.Tunnel).Select(l => l.Line));
        }


        public class Scatter : PotentialFieldParameter.Scatter
        {
            public TestResult.MapType Map { get; }
            public double Constant { get; }
            public double Si { get; }
            public int R { get; }

            public Scatter(int obstacleRange, int obstacleConstant, int attractiveConstant, double constant, double si, int r, TestResult.MapType map) : base(obstacleRange, obstacleConstant, attractiveConstant)
            {
                Constant = constant;
                Si = si;
                R = r;
                Map = map;
            }

            public override string Line => base.Line + $",{Constant},{Si},{R}";
        }

    }
}
