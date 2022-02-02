using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Core.Navigation.Tree;
using Benchmarking.Helper;
using Benchmarking.SingleRun;
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
        public async Task FullRun()
        {
            var r = new Random();
            
            var sn = new StateNotifier(_output, $"FullRun-{Guid.NewGuid()}.txt");
            sn.Run(100 * 3 * 4, 100 * 3 * 4 / 100);

            // based of trial data gathered
            var pfSetting = new PotentialFieldSettings(5, 1, 18);
            var ppfSetting1 = new PotentialFieldSettings(3, 9, 9);
            var ppfSetting2 = new PheromoneSettings(1, 5, 7);
            var rrtSettings = new RRTSettings(41, 6);

            for (var i = 0; i < 100; i++)
            {
                var (map1, robot1, goal1) = DefaultMaps.GetFiveBlockMap(r);
                var (map2, robot2, goal2) = DefaultMaps.GetTunnelMap(r);
                var (map3, robot3, goal3) = DefaultMaps.GetBugTrapMap(r);

                // run the different maps
                var tasks = new List<Task>
                {
                    RunNavigationalMethod(sn, "0,0", new PotentialField(map1, new Robot(robot1.Location, robot1.FovLength), goal1, 1000, pfSetting)),
                    RunNavigationalMethod(sn, "0,1", new PotentialField(map2, new Robot(robot2.Location, robot2.FovLength), goal2, 1000, pfSetting)),
                    RunNavigationalMethod(sn, "0,2", new PotentialField(map3, new Robot(robot3.Location, robot3.FovLength), goal3, 1000, pfSetting)),

                    RunNavigationalMethod(sn, "1,0", new PheromonePotentialField(map1, new Robot(robot1.Location, robot1.FovLength), goal1, 1000, ppfSetting1, ppfSetting2)),
                    RunNavigationalMethod(sn, "1,1", new PheromonePotentialField(map2, new Robot(robot2.Location, robot2.FovLength), goal2, 1000, ppfSetting1, ppfSetting2)),
                    RunNavigationalMethod(sn, "1,2", new PheromonePotentialField(map3, new Robot(robot3.Location, robot3.FovLength), goal3, 1000, ppfSetting1, ppfSetting2)),

                    RunNavigationalMethod(sn, "2,0", new AStar(map1, new Robot(robot1.Location, robot1.FovLength), goal1, 1000)),
                    RunNavigationalMethod(sn, "2,1", new AStar(map2, new Robot(robot2.Location, robot2.FovLength), goal2, 1000)),
                    RunNavigationalMethod(sn, "2,2", new AStar(map3, new Robot(robot3.Location, robot3.FovLength), goal3, 1000)),

                    RunNavigationalMethod(sn, "3,0", new RRT(map1, new Robot(robot1.Location, robot1.FovLength), goal1, 1000, rrtSettings)),
                    RunNavigationalMethod(sn, "3,1", new RRT(map2, new Robot(robot2.Location, robot2.FovLength), goal2, 1000, rrtSettings)),
                    RunNavigationalMethod(sn, "3,2", new RRT(map3, new Robot(robot3.Location, robot3.FovLength), goal3, 1000, rrtSettings))
                };
                await Task.WhenAll(tasks);
            }
            sn.Result();
        }

        [Fact]
        public async Task FullRunAnalysis()
        {

        }

        private async Task RunNavigationalMethod(StateNotifier sn, string identifier, NavigationalMethod nvm)
        {
            await Task.Run(() =>
            {
                var run = nvm.Run();
                var analysis = new RunAnalyzer(nvm).OutPut;
                _output.WriteLine($"{identifier},{analysis}");
                sn.NotifyCompletion($"{identifier},{run},{analysis}");
            });
        }
        
    }
}
