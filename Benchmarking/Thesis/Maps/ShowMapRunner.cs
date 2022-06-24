using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.Maps
{
    public class ShowMapRunner
    {
        private readonly ITestOutputHelper _output;

        public ShowMapRunner(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task WallOne()
        {
            await Show(ThesisMaps.WallOne);
        }

        [Fact]
        public async Task WallTwo()
        {
            await Show(ThesisMaps.WallTwo);
        }

        [Fact]
        public async Task WallThree()
        {
            await Show(ThesisMaps.WallThree);
        }

        [Fact]
        public async Task SlitOne()
        {
            await Show(ThesisMaps.SlitOne);
        }

        [Fact]
        public async Task SlitTwo()
        {
            await Show(ThesisMaps.SlitTwo);
        }

        [Fact]
        public async Task SlitThree()
        {
            await Show(ThesisMaps.SlitThree);
        }

        [Fact]
        public async Task RoomOne()
        {
            await Show(ThesisMaps.RoomOne);
        }

        [Fact]
        public async Task RoomTwo()
        {
            await Show(ThesisMaps.RoomTwo);
        }

        [Fact]
        public async Task RoomThree()
        {
            await Show(ThesisMaps.RoomThree);
        }

        [Fact]
        public async Task PlankPileOne()
        {
            await Show(ThesisMaps.PlankPileOne);
        }

        [Fact]
        public async Task PlankPileTwo()
        {
            await Show(ThesisMaps.PlankPileTwo);
        }

        [Fact]
        public async Task PlankPileThree()
        {
            await Show(ThesisMaps.PlankPileThree);
        }

        [Fact]
        public async Task CorridorOne()
        {
            await Show(ThesisMaps.CorridorOne);
        }

        [Fact]
        public async Task CorridorTwo()
        {
            await Show(ThesisMaps.CorridorTwo);
        }

        [Fact]
        public async Task CorridorThree()
        {
            await Show(ThesisMaps.CorridorThree);
        }

        [Fact]
        public async Task BugTrapOne()
        {
            await Show(ThesisMaps.BugTrapOne);
        }

        [Fact]
        public async Task BugTrapTwo()
        {
            await Show(ThesisMaps.BugTrapTwo);
        }

        [Fact]
        public async Task BugTrapThree()
        {
            await Show(ThesisMaps.BugTrapThree);
        }


        private static async Task Show(Func<(int[,] map, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal)> setup)
        {
            var (map, obstacles, robot, goal) = setup.Invoke();

            foreach (var obstacle in obstacles)
                map.FillMap(obstacle, MapExt.WallPoint);

            if (goal != default)
                map.FillMap(goal, MapExt.GoalSpawn);
            
            if (robot != default)
                map.FillMap(robot, MapExt.RobotSpawn);

            await Viewer.Show(map, robot, goal);
        }
    }
}
