using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.SingleRun
{
    public class PotentialFieldTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PotentialFieldTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task RunAndShow()
        {
            var (map, robot, goal) = DefaultMaps.GetBugTrapMap(new Random());
            
            var settings = new PfSettings(10, 1, 1);
            var method = new PotentialField(map, robot, goal, 1000, settings);
            var result = method.Run();
            _testOutputHelper.WriteLine(result);
            await Viewer.Image("test.png", map, robot, goal);
        }

        [Fact]
        public async Task SectionRunAndShow()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_testOutputHelper, $"PotentialField-{Guid.NewGuid()}.txt");

            for (var or = 1; or < 20; or += 10)
            for (var oc = 1; oc <= 10; oc += 5)
            for (var ac = 1; ac <= 10; ac += 5)
            for (var i = 0; i < 2; i++)
            {
                var maps = DefaultMaps.AllMaps(new Random());
                var settings = new PfSettings(or, oc, ac);
                foreach (var (map, robot, goal, mapName) in maps)
                {
                    tasks.Add(RunMethod(sn, map, robot, goal, mapName, settings));
                }
            }
            sn.Run(tasks.Count, tasks.Count);
            await Task.WhenAll(tasks);
            sn.Result();
        }

        [Fact]
        public void ParameterCounter()
        {
            var c = 0;
            for (var or = 1; or < 40; or++)
            for (var oc = 1; oc <= 20; oc++)
            for (var ac = 1; ac <= 20; ac++)
            //for (var i = 0; i < 100; i++)
            for (var m = 0; m < 3; m++)
                c++;
            
            _testOutputHelper.WriteLine(c.ToString());
        }

        [Fact]
        public async Task ParameterEvaluation()
        {
            var sn = new StateNotifier(_testOutputHelper, $"PotentialField-{Guid.NewGuid()}.txt");

            var settings = new List<PfSettings>();
            for (var or = 1; or < 40; or++)
            for (var oc = 1; oc <= 20; oc++)
            for (var ac = 1; ac <= 20; ac++)
            {
                settings.Add(new PfSettings(or, oc, ac));
            }

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

        private async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName, PfSettings settings)
        {
            var result = await Task.Run(() => new PotentialField(map, robot, goal, 1000, settings).Run());
            sn.NotifyCompletion(new[] { $"{mapName},{result}" });
        }
    }
}
