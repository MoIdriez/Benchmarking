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
            await Show(Wall.WallOne);
        }

        [Fact]
        public async Task WallTwo()
        {
            await Show(Wall.WallTwo);
        }

        [Fact]
        public async Task WallThree()
        {
            await Show(Wall.WallThree);
        }

        [Fact]
        public async Task SlitOne()
        {
            await Show(Wall.SlitOne);
        }

        [Fact]
        public async Task SlitTwo()
        {
            await Show(Wall.SlitTwo);
        }

        [Fact]
        public async Task SlitThree()
        {
            await Show(Wall.SlitThree);
        }

        [Fact]
        public async Task RoomOne()
        {
            await Show(Wall.RoomOne);
        }

        [Fact]
        public async Task RoomTwo()
        {
            await Show(Wall.RoomTwo);
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
