using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.SingleRun
{
    public class AStarTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public AStarTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task RunAndShow()
        {
            var (map, robot, goal) = DefaultMaps.GetFiveBlockMap(new Random());
            var method = new AStar(map, robot, goal, 10000);
            var result = method.Run();
            _testOutputHelper.WriteLine(result);
            await Viewer.Image("test.png", map, robot, goal);
        }

        [Fact]
        public async Task FullRun()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_testOutputHelper, $"AStar-{Guid.NewGuid()}.txt");
            for (var i = 0; i < 100; i++)
            {
                var maps = DefaultMaps.AllMaps(new Random());
                
                foreach (var (map, robot, goal, mapName) in maps)
                {
                    tasks.Add(RunMethod(sn, map, robot, goal, mapName));
                }
            }
            sn.Run(tasks.Count, tasks.Count/100);
            await Task.WhenAll(tasks);
            sn.Result();

        }

        private async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName)
        {
            var result = await Task.Run(() => new AStar(map, robot, goal, 1000).Run());

            sn.NotifyCompletion($"{mapName},{result}");
        }
    }
}
