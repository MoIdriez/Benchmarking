using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Helper;
using Benchmarking.Thesis.ChapterThree;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;
using Point = Benchmarking.Core.Map.Point;
using Rectangle = Benchmarking.Core.Map.Rectangle;

namespace Benchmarking.Thesis.ChapterFour
{
    public class RobotNav
    {
        private readonly Random _random = new();
        private readonly ITestOutputHelper _output;
        readonly PheromoneSettings _pheromoneSettings = new(1, 5, 8);
        readonly PotentialFieldSettings _potentialFieldSettings = new(3, 4, 9);
        

        public RobotNav(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task Walls()
        {
            await ViewWithObstacles(ThesisMaps.WallOne());
            await ViewWithObstacles(ThesisMaps.WallTwo());
            await ViewWithObstacles(ThesisMaps.WallThree());
        }

        [Fact]
        public async Task Slits()
        {
            await ViewWithObstacles(ThesisMaps.SlitOne());
            await ViewWithObstacles(ThesisMaps.SlitTwo());
            await ViewWithObstacles(ThesisMaps.SlitThree());
        }

        [Fact]
        public async Task Rooms()
        {
            await View(ThesisMaps.GenerateMap(ThesisMaps.RoomOne, _random).map, default, default, default);
            await View(ThesisMaps.GenerateMap(ThesisMaps.RoomTwo, _random).map, default, default, default);
            await View(ThesisMaps.GenerateMap(ThesisMaps.RoomThree, _random).map, default, default, default);
        }

        [Fact]
        public async Task PlankPiles()
        {
            await ViewWithObstacles(ThesisMaps.PlankPileOne());
            await ViewWithObstacles(ThesisMaps.PlankPileTwo());
            await ViewWithObstacles(ThesisMaps.PlankPileThree());
        }

        [Fact]
        public async Task Corridors()
        {
            await ViewWithObstacles(ThesisMaps.CorridorOne());
            await ViewWithObstacles(ThesisMaps.CorridorTwo());
            await ViewWithObstacles(ThesisMaps.CorridorThree());
        }

        [Fact]
        public async Task BugTraps()
        {
            await ViewWithObstacles(ThesisMaps.BugTrapOne());
            await ViewWithObstacles(ThesisMaps.BugTrapTwo());
            await ViewWithObstacles(ThesisMaps.BugTrapThree());
        }

        [Fact]
        public async Task Obstacles()
        {
            var maps = ThesisMaps.GenerateObstacleMaps(_random);

            foreach (var (mapName, map, robot, goal) in maps)
            {
                await View(map, default, default, default, $"ChapterThree-{mapName}-{Guid.NewGuid()}.png");
            }
        }

        [Fact]
        public async Task Tunnels()
        {
            var maps = ThesisMaps.GenerateTunnelMaps(_random);

            foreach (var (mapName, map, robot, goal) in maps)
            {
                await View(map, default, default, default, $"ChapterThree-{mapName}-{Guid.NewGuid()}.png");
            }
        }

        [Fact]
        public async Task SimplePheromoneWall()
        {
            var (map, _, _) = ThesisMaps.GenerateMap(ThesisMaps.WallTwo, _random);
            var robot = new Robot(new Point(10, 24), 30);
            var goal = new Point(40, 24);
            PotentialField pf = new PotentialField(map, new Robot(robot.Location, 30), goal, 1000, _potentialFieldSettings);
            pf.Run();
            
            await View(pf.Map, pf.Robot.Steps.First().Location, pf.Goal, pf.Robot.Steps.Select(s => s.Location).ToList());
        }

        [Fact]
        public async Task PotentialFieldAndPheromoneBugThree()
        {
            PotentialField pf;
            PheromonePotentialField ppf;

            do
            {
                var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.BugTrapThree, _random);
                pf = new PotentialField(map, new Robot(robot.Location, 30), goal, 1000, _potentialFieldSettings);
                ppf = new PheromonePotentialField(map, new Robot(robot.Location, 30), goal, 1000, _potentialFieldSettings, _pheromoneSettings);
                pf.Run();
                ppf.Run();
            } while (!ppf.HasSeenGoal);

            await View(pf.Map, pf.Robot.Steps.First().Location, pf.Goal, pf.Robot.Steps.Select(s => s.Location).ToList());
            await View(ppf.Map, ppf.Robot.Steps.First().Location, ppf.Goal, ppf.Robot.Steps.Select(s => s.Location).ToList());
        }


        public async Task ViewWithObstacles((int[,] map, Rectangle[] obstacles, Rectangle? robot, Rectangle? goal) m)
        {
            var image = new Bitmap(m.map.Width(), m.map.Height());
            var grf = Graphics.FromImage(image);

            foreach (var obstacle in m.obstacles)
                m.map.FillMap(obstacle, MapExt.WallPoint);

            for (var x = 0; x < m.map.Width(); x++)
            {
                for (var y = 0; y < m.map.Height(); y++)
                {
                    if (m.map[x, y] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    if (m.map[x, y] == MapExt.DefaultValue)
                        image.SetPixel(x, y, Color.White);
                }
            }

            if (m.robot != default)
            {
                for (var x = m.robot.MinX; x <= m.robot.MaxX; x++)
                {
                    for (var y = m.robot.MinY; y <= m.robot.MaxY; y++)
                    {
                        image.SetPixel(x, y, Color.LightBlue);
                    }
                }
            }

            if (m.goal != default)
            {
                for (var x = m.goal.MinX; x <= m.goal.MaxX; x++)
                {
                    for (var y = m.goal.MinY; y <= m.goal.MaxY; y++)
                    {
                        image.SetPixel(x, y, Color.LightGreen);
                    }
                }
            }

            var fileName = $"ChapterFour-{Guid.NewGuid()}.png";
            await Task.Run(() => image.Save(fileName, ImageFormat.Png));
            Process.Start("cmd", $"/c {fileName}");
        }


        public async Task View(int[,] map, Point? robot, Point? goal, List<Point>? steps, string? fileName = default)
        {
            var image = new Bitmap(map.Width(), map.Height());
            var grf = Graphics.FromImage(image);
            for (var x = 0; x < map.Width(); x++)
            {
                for (var y = 0; y < map.Height(); y++)
                {
                    if (map[x, y] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    if (map[x, y] == MapExt.DefaultValue)
                        image.SetPixel(x, y, Color.White);
                }
            }

            foreach (var step in steps ?? new List<Point>())
            {
                image.SetPixel(step.X, step.Y, Color.SandyBrown);
            }

            if (robot != default)
                grf.FillEllipse(new SolidBrush(Color.LightBlue), robot.X - 2, robot.Y - 2, 3, 3);
            if (goal != default)
                grf.FillEllipse(new SolidBrush(Color.LightGreen), goal.X - 2, goal.Y - 2, 3, 3);

            fileName ??= $"ChapterFour-{Guid.NewGuid()}.png";
            await Task.Run(() => image.Save(fileName, ImageFormat.Png));
            Process.Start("cmd", $"/c {fileName}");
        }

        public Color ColorBasedOnPercent(decimal percent, params Color[] colors)
        {
            if (colors.Length == 0)
            {
                //I am using Transparent as my default color if nothing was passed
                return Color.Transparent;
            }
            if (percent > 1)
            {
                percent = percent / 100;
            }

            //find the two colors within your list of colors that the percent should fall between
            var colorRangeIndex = (colors.Length - 1) * percent;
            var minColorIndex = (int)Math.Truncate(colorRangeIndex);
            var maxColorIndex = minColorIndex + 1;
            var minColor = colors[minColorIndex];

            if (maxColorIndex < colors.Length)
            {
                var maxColor = colors[maxColorIndex];

                //get the differences between all the color values for the two colors you are fading between
                var aScale = maxColor.A - minColor.A;
                var redScale = maxColor.R - minColor.R;
                var greenScale = maxColor.G - minColor.G;
                var blueScale = maxColor.B - minColor.B;

                //the decimal distance of how "far" this color should be from the minColor in the range
                var gradientPct = colorRangeIndex - minColorIndex;

                //for each piece of the color (ARGB), add a percentage(gradientPct) of the distance between the two colors
                int getRGB(int originalRGB, int scale) => (int)Math.Round(originalRGB + (scale * gradientPct));

                return Color.FromArgb(getRGB(minColor.A, aScale), getRGB(minColor.R, redScale), getRGB(minColor.G, greenScale), getRGB(minColor.B, blueScale));
            }
            return minColor;
        }
    }
}
