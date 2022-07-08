using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Tree;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.SingleRun
{
    public class RRTExtendedTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public RRTExtendedTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task RunAndShow()
        {
            var (map, robot, goal) = DefaultMaps.GetFiveBlockMap(new Random());

            var settings = new RRTSettings(30, 30);
            var method = new RRTExtended(map, robot, goal, 1000, settings);
            var result = method.Run();
            _testOutputHelper.WriteLine(result);
            await Viewer.Image("test.png", map, robot, goal);

            _testOutputHelper.WriteLine($"R: {robot.Steps[0]}, G: {goal}");
            foreach (var robotStep in robot.Steps.Where(s => s.Location.InsideObstacle(DefaultMaps.GetFiveObstacles())))
            {
                _testOutputHelper.WriteLine($"{robotStep} - {map[robotStep.Location.X, robotStep.Location.Y]}");
            }
        }

        [Fact]
        public void ParameterCounter()
        {
            var c = 0;
            for (var s = 10; s <= 100; s += 5)
            for (var d = 10; d <= 100; d += 5)
            for (var i = 0; i < 100; i++)
            for (var m = 0; m < 3; m++)
                c++;

            _testOutputHelper.WriteLine(c.ToString("N"));
        }

        [Fact]
        public async Task FullRun()
        {
            var sn = new StateNotifier(_testOutputHelper, $"RRTExtended-{Guid.NewGuid()}.txt");

            var settings = new List<RRTSettings>();
            for (var s = 10; s <= 100; s += 5)
            for (var d = 10; d <= 100; d += 5)
                settings.Add(new RRTSettings(s, d));

            sn.Run(settings.Count * 100 * 3, settings.Count * 100 * 3 / 100);

            foreach (var setting in settings)
            {
                var tasks = new List<Task>();

                for (var i = 0; i < 100; i++)
                {
                    var maps = DefaultMaps.AllMaps(new Random());
                    foreach (var (map, robot, goal, mapName) in maps)
                    {
                        tasks.Add(RunMethod(sn, map, robot, goal, mapName, setting));
                    }
                }

                await Task.WhenAll(tasks);

            }
            sn.Result();
        }

        private async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName, RRTSettings settings)
        {
            try
            {
                var result = await Task.Run(() => new RRTExtended(map, robot, goal, 1000, settings).Run());
                sn.NotifyCompletion($"{mapName},{result}");
            }
            catch (Exception ex)
            {
                _testOutputHelper.WriteLine($"{ex.Message}, {ex.StackTrace}");
            }
            
        }
    }
}
