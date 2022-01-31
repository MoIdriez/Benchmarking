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
    public class ParameterListTest
    {
        private readonly ITestOutputHelper _output;

        public ParameterListTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Prepare()
        {
            var results = TestResult.GetPotentialFieldExtensive();

            var parameters = new List<ParameterResult>();

            var max = results.Count(r => r.AttractiveConstant == 3);
            var s = "or = [";
            for (var i = 1; i <= 20; i++)
                s += $" {results.Count(r => r.ObstacleRange == i && r.Success).Percentage(max)}";
            //parameters.Add(new ParameterResult("ObstacleRange", i, results.Count(r => r.ObstacleRange == i && r.Success)));
            _output.WriteLine($"{s}];");

            s = "oc = [";
            for (var i = 1; i <= 20; i++)
                s += $" {results.Count(r => r.ObstacleConstant == i && r.Success).Percentage(max)}";
            //parameters.Add(new ParameterResult("ObstacleConstant", i, results.Count(r => r.ObstacleConstant == i && r.Success)));
            _output.WriteLine($"{s}];");

            s = "ac = [";
            for (var i = 1; i <= 20; i++)
                s += $" {results.Count(r => r.AttractiveConstant == i && r.Success).Percentage(max)}";
            //parameters.Add(new ParameterResult("AttractiveConstant", i, results.Count(r => r.AttractiveConstant == i && r.Success)));
            _output.WriteLine($"{s}]; %{results.Count(r => r.AttractiveConstant == 3)}");
            
            foreach (var parameterResult in parameters)
            {
                _output.WriteLine($"{parameterResult.Name}");
            }
        }

        [Fact]
        public void PreparePheromone()
        {
            var results = TestResult.GetPheromonePotentialFieldExtensive();

            var max = results.Count(r => r.AttractiveConstant == 3);
            _output.WriteLine($" %{max}");
            var s = "or = [";
            for (var i = 1; i <= 10; i += 2)
                s += $" {results.Count(r => r.ObstacleRange == i && r.Success).Percentage(max)}";
            _output.WriteLine($"{s}];");

            s = "oc = [";
            for (var i = 1; i <= 10; i += 2)
                s += $" {results.Count(r => r.ObstacleConstant == i && r.Success).Percentage(max)}";
            _output.WriteLine($"{s}];");

            s = "ac = [";
            for (var i = 1; i <= 10; i += 2)
                s += $" {results.Count(r => r.AttractiveConstant == i && r.Success).Percentage(max)}";
            _output.WriteLine($"{s}];");

            s = "pc = [";
            for (var i = 1; i <= 10; i += 2)
                s += $" {results.Count(r => Math.Abs(r.Constant - i) < 0.001 && r.Success).Percentage(max)}";
            _output.WriteLine($"{s}];");

            s = "si = [";
            for (var i = 1; i <= 10; i += 2)
                s += $" {results.Count(r => Math.Abs(r.StrengthIncrease - i) < 0.001 && r.Success).Percentage(max)}";
            _output.WriteLine($"{s}];");

            s = "pr = [";
            for (var i = 1; i <= 10; i += 2)
                s += $" {results.Count(r => r.Range == i && r.Success).Percentage(max)}";
            _output.WriteLine($"{s}];");

        }

        public class ParameterResult
        {
            public ParameterResult(string name, int reference, int value)
            {
                Name = name;
                Reference = reference;
                Value = value;
            }

            public string Name { get; set; }
            public int Reference { get; set; }
            public int Value { get; set; }
        }
    }
}
