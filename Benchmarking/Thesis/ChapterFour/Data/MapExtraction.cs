using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterFour.Data
{
    public class MapExtraction
    {
        private readonly ITestOutputHelper _output;

        public MapExtraction(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Run()
        {
            var maps = GetStaticMaps();

            foreach (var map in maps)
            {
                _output.WriteLine($"======================================== \\\\");
                _output.WriteLine($"{map.Name}: W:{map.v.size.w} H:{map.v.size.h} \\\\");
                _output.WriteLine($"Robot Rectangle: X:[{map.v.robot?.MinX}-{map.v.robot?.MaxX}] |  Y:[{map.v.robot?.MinY}-{map.v.robot?.MaxY}] \\\\");
                _output.WriteLine($"Goal Rectangle: X:[{map.v.goal?.MinX}-{map.v.goal?.MaxX}] |  Y:[{map.v.goal?.MinY}-{map.v.goal?.MaxY}] \\\\");
                _output.WriteLine($"---------------------------------------- \\\\");
                foreach (var obstacle in map.v.obstacles)
                {
                    _output.WriteLine($"Obstacle Rectangle: X:[{obstacle.MinX}-{obstacle.MaxX}] |  Y:[{obstacle.MinY}-{obstacle.MaxY}] \\\\");
                }
            }
        }

        public static (string Name, ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) v)[] GetStaticMaps(Random? rr = null)
        {
            var r = rr ?? new Random();

            var maps = new Func<((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal)>[]
            {
                WallOne,
                WallTwo,
                WallThree,

                SlitOne,
                SlitTwo,
                SlitThree,

                RoomOne,
                RoomTwo,
                RoomThree,

                PlankPileOne,
                PlankPileTwo,
                PlankPileThree,

                CorridorOne,
                CorridorTwo,
                CorridorThree,

                BugTrapOne,
                BugTrapTwo,
                BugTrapThree,
            };
            return maps.Select(m =>
            {
                var v = m.Invoke();
                return (m.Method.Name, v);
            }).ToArray();
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) WallOne()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(25, 26, 0, 10)
            };
            var robot = new Rectangle(2, 23, 2, 47);
            var goal = new Rectangle(27, 47, 2, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) WallTwo()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(25, 26, 0, 25)
            };
            var robot = new Rectangle(2, 23, 2, 47);
            var goal = new Rectangle(27, 47, 2, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) WallThree()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(25, 26, 0, 35)
            };
            var robot = new Rectangle(2, 23, 2, 47);
            var goal = new Rectangle(27, 47, 2, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }

        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) SlitOne()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(25, 26, 0, 7),
                new(25, 26, 40, 48)
            };
            var robot = new Rectangle(2, 23, 2, 47);
            var goal = new Rectangle(27, 47, 2, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) SlitTwo()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(25, 26, 0, 14),
                new(25, 26, 33, 48)
            };
            var robot = new Rectangle(2, 23, 2, 47);
            var goal = new Rectangle(27, 47, 2, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) SlitThree()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(25, 26, 0, 21),
                new(25, 26, 26, 48)
            };
            var robot = new Rectangle(2, 23, 2, 47);
            var goal = new Rectangle(27, 47, 2, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }

        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) RoomOne()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(0, 5, 16, 17),
                new(0, 5, 32, 33),

                new(16, 17, 0, 5),
                new(16, 17, 12, 21),
                new(16, 17, 28, 49),
                new(12, 21, 16, 17),
                new(12, 21, 32, 33),

                new(32, 33, 0, 5),
                new(32, 33, 12, 21),
                new(32, 33, 28, 49),
                new(28, 37, 16, 17),
                new(28, 37, 32, 33),

                new(45, 49, 16, 17),
                new(45, 49, 32, 33),

            };
            return ((map.Width(), map.Height()), obstacles, default, default);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) RoomTwo()
        {
            var map = MapExt.SetupMapWithBoundary(100, 100);
            var obstacles = new Rectangle[]
            {
                new(0, 8, 20, 21),
                new(0, 8, 40, 41),
                new(0, 21, 60, 61),

                new(15, 26, 20, 21),
                new(15, 26, 40, 41),
                new(15, 26, 80, 81),
                new(20, 21, 0, 21),
                new(20, 21, 35, 86),

                new(40, 41, 0, 21),
                new(40, 41, 35, 81),
                new(35, 46, 20, 21),
                new(35, 46, 40, 41),
                new(40, 51, 80, 81),

                new(40, 61, 60, 61),

                new(60, 61, 0, 21),
                new(60, 61, 60, 98),
                new(55, 66, 20, 21),

                new(60, 81, 40, 41),
                new(60, 81, 80, 81),

                new(80, 81, 0, 21),
                new(80, 98, 60, 61),

            };
            return ((map.Width(), map.Height()), obstacles, default, default);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) RoomThree()
        {
            var map = MapExt.SetupMapWithBoundary(200, 200);
            var obstacles = new Rectangle[]
            {
                new(20, 21, 40, 81),
                new(20, 21, 100, 121),
                new(20, 21, 140, 161),

                new(40, 41, 20, 61),
                new(40, 41, 100, 121),

                new(60, 61, 40, 61),
                new(60, 61, 120, 141),

                new(80, 81, 0, 81),
                new(80, 81, 140, 199),

                new(100, 101, 20, 61),

                new(120, 121, 120, 161),

                new(140, 141, 0, 21),
                new(140, 141, 60, 121),

                new(160, 161, 120, 181),

                new(20, 41, 20, 21),
                new(100, 121, 20, 21),

                new(120, 181, 40, 41),

                new(40, 61, 60, 61),

                new(60, 81, 80, 81),

                new(100, 141, 100, 101),

                new(20, 41, 120, 121),
                new(140, 161, 120, 121),

                new(60, 81, 140, 141),

                new(20, 61, 160, 161),
                new(100, 141, 160, 161),

                new(40, 61, 180, 181),

            };
            return ((map.Width(), map.Height()), obstacles, default, default);
        }

        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) PlankPileOne()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(10, 21, 16, 17),
                new(30, 41, 16, 17),
                new(10, 41, 32, 33),

            };
            var robot = new Rectangle(10, 41, 2, 7);
            var goal = new Rectangle(10, 41, 43, 47);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) PlankPileTwo()
        {
            var map = MapExt.SetupMapWithBoundary(100, 100);
            var obstacles = new Rectangle[]
            {
                new(15, 41, 25, 26),
                new(60, 86, 25, 26),

                new(15, 86, 50, 51),

                new(15, 41, 75, 76),
                new(60, 86, 75, 76),

            };
            var robot = new Rectangle(15, 86, 2, 20);
            var goal = new Rectangle(15, 86, 80, 97);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) PlankPileThree()
        {
            var map = MapExt.SetupMapWithBoundary(200, 200);
            var obstacles = new Rectangle[]
            {
                new(28, 56, 30, 31),
                new(84, 112, 30, 31),
                new(140, 168, 30, 31),

                new(0, 20, 60, 61),
                new(40, 80, 60, 61),
                new(120, 160, 60, 61),
                new(180, 198, 60, 61),

                new(22, 44, 90, 91),
                new(66, 88, 90, 91),
                new(110, 132, 90, 91),
                new(154, 176, 90, 91),

                new(0, 30, 120, 121),
                new(50, 150, 120, 121),
                new(170, 198, 120, 121),

                new(22, 44, 150, 151),
                new(66, 88, 150, 151),
                new(110, 132, 150, 151),
                new(154, 176, 150, 151),

                new(0, 30, 180, 181),
                new(50, 150, 180, 181),
                new(170, 198, 180, 181)
            };
            var robot = new Rectangle(5, 195, 5, 15);
            var goal = new Rectangle(5, 195, 185, 195);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }

        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) CorridorOne()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(13, 37, 0, 21),
                new(13, 37, 30, 49),
            };
            var robot = new Rectangle(3, 8, 5, 45);
            var goal = new Rectangle(43, 48, 5, 45);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) CorridorTwo()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(13, 23, 0, 21),
                new(23, 37, 0, 11),

                new(13, 30, 30, 49),
                new(30, 37, 18, 49),
            };
            var robot = new Rectangle(3, 8, 5, 45);
            var goal = new Rectangle(43, 48, 5, 45);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) CorridorThree()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(13, 37, 0, 7),
                new(13, 18, 0, 35),

                new(13, 28, 29, 37),
                new(23, 37, 14, 22),


                new(32, 37, 15, 49),
                new(13, 37, 43, 49),

            };
            var robot = new Rectangle(3, 8, 5, 45);
            var goal = new Rectangle(43, 48, 5, 45);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }

        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) BugTrapOne()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(10, 41, 15, 16),
                new(10, 11, 15, 36),
                new(40, 41, 15, 36),

            };
            var robot = new Rectangle(10, 41, 3, 7);
            var goal = new Rectangle(13, 38, 18, 25);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) BugTrapTwo()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(10, 41, 15, 16),
                new(10, 11, 15, 36),
                new(40, 41, 15, 36),

                new(10, 16, 35, 36),
                new(35, 41, 35, 36),

            };
            var robot = new Rectangle(10, 41, 3, 7);
            var goal = new Rectangle(13, 38, 18, 25);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
        public static ((int w, int h) size, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) BugTrapThree()
        {
            var map = MapExt.SetupMapWithBoundary(50, 50);
            var obstacles = new Rectangle[]
            {
                new(10, 41, 15, 16),
                new(10, 11, 15, 36),
                new(40, 41, 15, 36),

                new(10, 23, 35, 36),
                new(28, 41, 35, 36),
            };
            var robot = new Rectangle(10, 41, 3, 7);
            var goal = new Rectangle(13, 38, 18, 25);
            return ((map.Width(), map.Height()), obstacles, robot, goal);
        }
    }
}
