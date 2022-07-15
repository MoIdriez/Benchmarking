using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Generation;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Helper;
using Benchmarking.Thesis.Maps;
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
        public async Task SpeedTest()
        {
            var r = new Random();
            _output.WriteLine("te343est");
            for (var i = 0; i < 5; i++)
            {
                var maps = ThesisMaps.GetGeneratedMaps(r);
            }
        }

        [Fact]
        public void ObstacleMapStats()
        {
            var r = new Random();

            var mapGenerators = new List<(string mapIdentifier, MapGenerator generator)>
            {
                ("ObstacleOne", new MapGenerator(200, 200, new IntRange(10, 15), new IntRange(3, 5))),
                ("ObstacleTwo", new MapGenerator(200, 200, new IntRange(70, 75), new IntRange(3, 5))),
                ("ObstacleThree", new MapGenerator(200, 200, new IntRange(130, 135), new IntRange(3, 5))),
                ("ObstacleFour", new MapGenerator(200, 200, new IntRange(190, 195), new IntRange(3, 5))),
                ("ObstacleFive", new MapGenerator(200, 200, new IntRange(260, 265), new IntRange(3, 5))),
                ("ObstacleSix", new MapGenerator(200, 200, new IntRange(330, 335), new IntRange(3, 5))),
                ("ObstacleSeven", new MapGenerator(200, 200, new IntRange(390, 395), new IntRange(3, 5))),
                ("ObstacleEight", new MapGenerator(200, 200, new IntRange(450, 455), new IntRange(3, 5))),
                ("ObstacleNine", new MapGenerator(200, 200, new IntRange(510, 515), new IntRange(3, 5))),
                ("ObstacleTen", new MapGenerator(200, 200, new IntRange(560, 565), new IntRange(3, 5)))
            };

            foreach (var t in mapGenerators)
            {
                var maps = new List<int[,]>();
                var steps = new List<int>();
                while (maps.Count != 10)
                {
                    var map = t.generator.GetInstance();
                    var robot = new Robot(MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.General), 30);
                    var goal = MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.General);
                    var plan = AStar.Plan(map, robot.Location, goal);
                    if (!robot.Location.Equals(new Point(0, 0)) && !goal.Equals(new Point(0, 0)) && plan.Any() && plan.Count > 150)
                    {
                        maps.Add(map);
                        steps.Add(plan.Count);
                    }
                }
                _output.WriteLine($"{t.mapIdentifier}: {maps.Average(m => m.Occupation())} | {steps.Average()}");
            }
        }

        [Fact]
        public void TunnelMapStats()
        {
            var r = new Random();

            var mapGenerators = new List<(string mapIdentifier, MapGenerator generator)>
            {
                ("TunnelOne", new MapGenerator(200, 200, new IntRange(11, 15), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelTwo", new MapGenerator(200, 200, new IntRange(21, 25), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelThree", new MapGenerator(200, 200, new IntRange(31, 35), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelFour", new MapGenerator(200, 200, new IntRange(41, 45), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelFive", new MapGenerator(200, 200, new IntRange(51, 55), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelSix", new MapGenerator(200, 200, new IntRange(61, 65), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelSeven", new MapGenerator(200, 200, new IntRange(71, 75), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelEight", new MapGenerator(200, 200, new IntRange(81, 85), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelNine", new MapGenerator(200, 200, new IntRange(91, 95), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7))),
                ("TunnelTen", new MapGenerator(200, 200, new IntRange(110, 155), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7)))
            };

            foreach (var t in mapGenerators)
            {
                var maps = new List<int[,]>();
                var steps = new List<int>();
                while (maps.Count != 10)
                {
                    var map = t.generator.GetInstance();
                    var robot = new Robot(MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Robot), 30);
                    var goal = MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Goal);
                    var plan = AStar.Plan(map, robot.Location, goal);
                    if (!robot.Location.Equals(new Point(0,0)) &&!goal.Equals(new Point(0,0)) && plan.Any() && plan.Count > 150)
                    {
                        maps.Add(map);
                        steps.Add(plan.Count);
                    }
                }
                _output.WriteLine($"{t.mapIdentifier}: {maps.Average(m => m.Occupation())} | {steps.Average()}");
            }
        }

        [Fact]
        public async Task ShowObstacles()
        {
            var r = new Random();
            int[,] map;
            Robot robot;
            Point goal;
            do
            {
                var mapG = new MapGenerator(200, 200, new IntRange(600, 605), new IntRange(3, 5));
                map = mapG.GetInstance();
                robot = new Robot(MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Robot), 30);
                goal = MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.Goal);
            } while (!AStar.Plan(map, robot.Location, goal).Any());
            
            //_output.WriteLine($"{AStar.Plan(map, robot.Location, goal).Any()}");
            await Viewer.Image("test.png", map, robot, goal);
        }

        [Fact]
        public async Task ShowDungeon()
        {
            var r = new Random();

            int[,] map;
            Robot robot;
            Point goal;
            List<Point> plan;
            do
            {
                var mapG = new MapGenerator(200, 200, new IntRange(110, 115), new IntRange(7, 10), new IntRange(7, 10), new IntRange(25, 40), new IntRange(5, 7));
                map = mapG.GetInstance();
                robot = new Robot(MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.General), 30);
                goal = MapGenerator.GetEmptyLocation(map, r, MapGenerator.LocationType.General);
                plan = AStar.Plan(map, robot.Location, goal);
            } while (!plan.Any() && plan.Count > 150);

            
            
            _output.WriteLine($"{AStar.Plan(map, robot.Location, goal).Any()}");
            await Viewer.Image("test.png", map, robot, goal);
        }
    }
}
