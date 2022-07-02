using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
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
        public async Task BasicRunAndView()
        {
            var finished = false;
            //while (!finished)
            //{
                var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.BugTrapThree, _random);
                //var settings = new PotentialFieldSettings(2, 2, 3);
                //var pheromones = new PheromoneSettings(1, 6, 6);
                //var result = new PotentialField(map, robot, goal, 3000, settings).Run();
                //var result = new PheromonePotentialField(map, robot, goal, 3000, settings, pheromones).Run();
                var result = new AStar(map, robot, goal, 3000).Run();
                //finished = new BasicPotentialFieldAnalysis.Data($"MapTest,{result}").Success;
                //finished = new PheromonePotentialFieldAnalysis.PfData($"MapTest,{result}").Success;
                //if (finished)
                //{
                    _output.WriteLine(result);
                    await Viewer.Image($"ppf-{Guid.NewGuid()}.png", map, robot, goal);
            //    }
            //}

            foreach (var step in robot.Steps)
            {
                _output.WriteLine($"{step.Location} - {map[step.Location.X, step.Location.Y]}");
            }
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
            for (var i = 0; i < 20; i++)
            {
                var settings = new PotentialFieldSettings(or, oc, ac);
                var maps = ThesisMaps.GetMaps();
                tasks.AddRange(maps.Select(map => RunMethod(sn, map.map, map.robot, map.goal, map.mapName, settings)));
            }
            sn.Run(tasks.Count, 1000);
            await Task.WhenAll(tasks);
            sn.Result();
        }

        [Fact]
        public async Task ExtensivePheromoneRun()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"PF-{Guid.NewGuid()}.txt");

            for (var c = 1; c <= 10; c += 1)
            for (var si = 1; si <= 10; si += 1)
            for (var r = 1; r <= 10; r += 1)
            for (var i = 0; i < 20; i++)
            {
                var settings = new PotentialFieldSettings(3, 4, 9);
                var pheromoneSettings = new PheromoneSettings(c, si, r);
                var maps = ThesisMaps.GetMaps();
                tasks.AddRange(maps.Select(map => RunPheromoneMethod(sn, map.map, map.robot, map.goal, map.mapName, settings, pheromoneSettings)));
            }
            sn.Run(tasks.Count, 1000);
            await Task.WhenAll(tasks);
            sn.Result();
        }
        
        public static async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName, PotentialFieldSettings settings)
        {
            var result = await Task.Run(() => new PotentialField(map, robot, goal, 1000, settings).Run());
            sn.NotifyCompletion($"{mapName},{result}");
        }
        
        public static async Task RunPheromoneMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName, PotentialFieldSettings settings, PheromoneSettings pheromoneSettings)
        {
            var result = await Task.Run(() => new PheromonePotentialField(map, robot, goal, 1000, settings, pheromoneSettings).Run());
            sn.NotifyCompletion($"{mapName},{result}");
        }
    }
}
