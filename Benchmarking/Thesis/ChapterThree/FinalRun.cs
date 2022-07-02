using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Helper;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class FinalRun
    {
        private readonly ITestOutputHelper _output;

        public FinalRun(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task RunFinal()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"FinalSearch-{Guid.NewGuid()}.txt");

            var r = new Random();

            var settings = new PotentialFieldSettings(3, 4, 9);
            var pheromoneSettings = new PheromoneSettings(1, 5, 8);

            for (var i = 0; i < 1000; i++)
            {
                var maps = ThesisMaps.GetMaps(r);
                foreach (var map in maps)
                {
                    tasks.Add(Run("BaseLine", new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, baseLine: true), i, map));
                    tasks.Add(Run("PotentialField", new PotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, settings), i, map));
                    tasks.Add(Run("PheromoneField", new PheromonePotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, settings, pheromoneSettings), i, map));
                }
            }

            sn.Run(tasks.Count, 100);
            await Task.WhenAll(tasks);
            sn.Result();

            async Task Run(string methodName, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                await Task.Run(method.Run);
                sn.NotifyCompletion($"{i},{methodName},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F}");
            }
        }
    }
}
