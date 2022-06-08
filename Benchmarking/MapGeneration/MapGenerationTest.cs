using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Generation;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Helper;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.MapGeneration
{
    public class MapGenerationTest
    {
        private readonly ITestOutputHelper _output;

        public MapGenerationTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task ShowObstacles()
        {
            var r = new Random();
            var mapG = new MapGenerator(200, 200, new IntRange(5, 7), new IntRange(35, 45));
            var map = mapG.GetInstance();
            var robot = new Robot(MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Robot), 30);
            var goal = MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Goal);
            _output.WriteLine($"{AStar.Plan(map, robot.Location, goal).Any()}");
            await Viewer.Image("test.png", map, robot, goal);
        }

        [Fact]
        public async Task ShowDungeon()
        {
            var r = new Random();
            var mapG = new MapGenerator(200, 200, new IntRange(3, 6), new IntRange(10, 20), new IntRange(10, 20), new IntRange(15, 20), new IntRange(5, 10));
            var map = mapG.GetInstance();
            var robot = new Robot(MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Robot), 30);
            var goal = MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Goal);
            _output.WriteLine($"{AStar.Plan(map, robot.Location, goal).Any()}");
            await Viewer.Image("test.png", map, robot, goal);
        }
    }
}
