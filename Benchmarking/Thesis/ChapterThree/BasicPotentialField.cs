using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Helper;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class BasicPotentialField
    {
        private readonly ITestOutputHelper _output;
        private readonly Random _random = new();

        public BasicPotentialField(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Test()
        {
            var t = ThesisMaps.WallOne;
            _output.WriteLine(t.Method.Name);
        }

        [Fact]
        public async Task BasicRunAndView()
        {
            var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.WallOne, _random);
            var settings = new PotentialFieldSettings(10, 1, 1);
            var result = new PotentialField(map, robot, goal, 1000, settings).Run();
            _output.WriteLine(result);
            await Viewer.Image($"bpf.png", map, robot, goal);
        }

        [Fact]
        public async Task WallOneParameterExploration()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"WallOneParameterExploration-{Guid.NewGuid()}.txt");

            for (var or = 1; or <= 10; or += 1)
            for (var oc = 1; oc <= 10; oc += 1)
            for (var ac = 1; ac <= 10; ac += 1)
            for (var i = 0; i < 10; i++)
            {
                var settings = new PotentialFieldSettings(or, oc, ac);
                var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.WallOne, _random);
                tasks.Add(RunMethod(sn, map, robot, goal, "WallOne", settings));
            }
            sn.Run(tasks.Count, 1000);
            await Task.WhenAll(tasks);
            sn.Result();
        }

        [Fact]
        public async Task WallTwoParameterExploration()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"WallTwoParameterExploration-{Guid.NewGuid()}.txt");

            for (var or = 1; or <= 10; or += 1)
            for (var oc = 1; oc <= 10; oc += 1)
            for (var ac = 1; ac <= 10; ac += 1)
            for (var i = 0; i < 10; i++)
            {
                var settings = new PotentialFieldSettings(or, oc, ac);
                var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.WallTwo, _random);
                tasks.Add(RunMethod(sn, map, robot, goal, "WallTwo", settings));
            }
            sn.Run(tasks.Count, 1000);
            await Task.WhenAll(tasks);
            sn.Result();
        }

        [Fact]
        public async Task WallThreeParameterExploration()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"WallThreeParameterExploration-{Guid.NewGuid()}.txt");

            for (var or = 1; or <= 10; or += 1)
            for (var oc = 1; oc <= 10; oc += 1)
            for (var ac = 1; ac <= 10; ac += 1)
            for (var i = 0; i < 10; i++)
            {
                var settings = new PotentialFieldSettings(or, oc, ac);
                var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.WallThree, _random);
                tasks.Add(RunMethod(sn, map, robot, goal, "WallThree", settings));
            }
            sn.Run(tasks.Count, 1000);
            await Task.WhenAll(tasks);
            sn.Result();
        }

        [Fact]
        public async Task ExtensiveRun()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"ExtensiveSearch-{Guid.NewGuid()}.txt");

            for (var or = 1; or <= 10; or += 1)
            for (var oc = 1; oc <= 10; oc += 1)
            for (var ac = 1; ac <= 10; ac += 1)
            for (var i = 0; i < 100; i++)
            {
                var settings = new PotentialFieldSettings(or, oc, ac);

            }
        }




        public static async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName, PotentialFieldSettings settings)
        {
            var result = await Task.Run(() => new PotentialField(map, robot, goal, 1000, settings).Run());
            sn.NotifyCompletion($"{mapName},{result}");
        }
    }
}
