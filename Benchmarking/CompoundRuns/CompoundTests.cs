using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Tree;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.CompoundRuns
{
    public class CompoundTests
    {
        private readonly ITestOutputHelper _output;

        public CompoundTests(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task AStarTest()
        {
            var (map, robot, goal) = DefaultMaps.GetFiveBlockMap(new Random());
            var method = new AStar(map, robot, goal, 1000);
            var result = method.Run();
            _output.WriteLine(result);
            await Viewer.Image("test.png", method.ExploredMap, robot, goal);

            var an = new RunAnalyzer(method);
            _output.WriteLine($"Success: {an.Success()}");
            _output.WriteLine($"StepEfficiency: {an.StepEfficiency()}");
            _output.WriteLine($"TimeEfficiency: {an.TimeEfficiency()}");
            _output.WriteLine($"ExplorationEfficiency: {an.ExplorationEfficiency()}");
            _output.WriteLine($"StepRedundancy: {an.StepRedundancy()}");
            _output.WriteLine($"StepVisibility: {an.StepVisibility()}");
        }

        [Fact]
        public async Task RRTTest()
        {
            var (map, robot, goal) = DefaultMaps.GetFiveBlockMap(new Random());
            var method = new RRT(map, robot, goal, 1000, new RRTSettings(30, 30));
            var result = method.Run();
            _output.WriteLine(result);
            await Viewer.Image("test.png", method.ExploredMap, robot, goal);

            var an = new RunAnalyzer(method);
            _output.WriteLine($"Success: {an.Success()}");
            _output.WriteLine($"StepEfficiency: {an.StepEfficiency()}");
            _output.WriteLine($"TimeEfficiency: {an.TimeEfficiency()}");
            _output.WriteLine($"ExplorationEfficiency: {an.ExplorationEfficiency()}");
            _output.WriteLine($"StepRedundancy: {an.StepRedundancy()}");
            _output.WriteLine($"StepVisibility: {an.StepVisibility()}");
        }
    }
}
