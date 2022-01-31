using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.SingleRun
{
    public class PheromonePotentialFieldTest
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public PheromonePotentialFieldTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public async Task RunAndShow()
        {
            var (map, robot, goal) = DefaultMaps.GetBugTrapMap(new Random());

            var pheromoneSettings = new PheromoneSettings(5, 10, 10);

            var settings = new PotentialFieldSettings(5, 10, 10);
            var method = new PheromonePotentialField(map, robot, goal, 1000, settings, pheromoneSettings);
            var result = method.Run();
            _testOutputHelper.WriteLine(result);
            await Viewer.Image("test.png", map, robot, goal);
        }

        [Fact]
        public void ParameterCounter()
        {
            var c = 0;
            for (var or = 1; or <= 10; or += 2)
            for (var oc = 1; oc <= 10; oc += 2)
            for (var ac = 1; ac <= 10; ac += 2)
            for (var pc = 1; pc <= 10; pc += 2)
            for (var si = 1; si <= 10; si += 2)
            for (var pr = 1; pr <= 10; pr += 2)
            for (var i = 0; i < 10; i++)
            for (var m = 0; m < 3; m++)
                c++;

            _testOutputHelper.WriteLine(c.ToString("N"));
        }

        [Fact]
        public async Task ParameterEvaluation()
        {
            var sn = new StateNotifier(_testOutputHelper, $"PheromonePotentialField-{Guid.NewGuid()}.txt");

            var settings = new List<Tuple<PotentialFieldSettings, PheromoneSettings>>();
            for (var or = 1; or <= 10; or += 2)
            for (var oc = 1; oc <= 10; oc += 2)
            for (var ac = 1; ac <= 10; ac += 2)
            for (var pc = 1; pc <= 10; pc += 2)
            for (var si = 1; si <= 10; si += 2)
            for (var pr = 1; pr <= 10; pr += 2)
            {
                var potentialFieldSettings = new PotentialFieldSettings(or, oc, ac);
                var pheromoneSettings = new PheromoneSettings(pc, si, pr);
                settings.Add(new Tuple<PotentialFieldSettings, PheromoneSettings>(potentialFieldSettings, pheromoneSettings));
            }

            sn.Run(settings.Count * 10 * 3, settings.Count * 10 * 3 / 100);
            foreach (var setting in settings)
            {
                var tasks = new List<Task>();

                for (var i = 0; i < 10; i++)
                {
                    var maps = DefaultMaps.AllMaps(new Random());
                    foreach (var (map, robot, goal, mapName) in maps)
                    {
                        tasks.Add(RunMethod(sn, map, robot, goal, mapName, setting.Item1, setting.Item2));
                    }
                }

                await Task.WhenAll(tasks);
            }
            sn.Result();
        }


        private async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName, PotentialFieldSettings settings, PheromoneSettings pheromoneSettings)
        {
            var result = await Task.Run(() => new PheromonePotentialField(map, robot, goal, 1000, settings, pheromoneSettings).Run());
            sn.NotifyCompletion($"{mapName},{result}");
        }
    }
}
