using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Helper;
using Benchmarking.Thesis.ChapterThree.Data;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class BaseLineCalculations
    {
        private readonly ITestOutputHelper _output;
        public BaseLineCalculations(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task BaseLine()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"BaseLine-{Guid.NewGuid()}.txt");

            for (var i = 0; i < 20; i++)
            {
                var maps = ThesisMaps.GetMaps();
                tasks.AddRange(maps.Select(map => RunMethod(sn, map.map, map.robot, map.goal, map.mapName)));
            }
            sn.Run(tasks.Count);
            await Task.WhenAll(tasks);
            sn.Result();
        }

        [Fact]
        public void CalculateBaseLine()
        {
            _output.WriteLine("BaseLine Calculations");

            var all = File.ReadAllLines(FileRef.BaseLine).Select(l => new Data(l)).ToList();
            
            _output.WriteLine("=============");
            _output.WriteLine("===SUCCESS===");
            _output.WriteLine("=============");
            // success per map type
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue(a.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }))
            {
                _output.WriteLine($"{mapType.Key}: {mapType.Success}");
            }

            _output.WriteLine("=============");
            _output.WriteLine("===MOVEABLE===");
            _output.WriteLine("=============");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue(a.Key)).Select(g => new { g.Key, IsStuck = g.Count(r => r.IsStuck).Percentage(g.Count()) }))
            {
                _output.WriteLine($"{mapType.Key}: {Math.Max(0, 100 - mapType.IsStuck)}");
            }

            _output.WriteLine("=============");
            _output.WriteLine("===PATH===");
            _output.WriteLine("=============");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue(a.Key)).Select(g => new { g.Key, PathLength = g.Average(r => r.Steps) }))
            {
                _output.WriteLine($"{mapType.Key}: {PathCompare(mapType.Key, mapType.PathLength)}");
            }

            _output.WriteLine("=============");
            _output.WriteLine("===DURATION===");
            _output.WriteLine("=============");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue(a.Key)).Select(g => new { g.Key, Duration = g.Average(r => r.Time) }))
            {
                _output.WriteLine($"{mapType.Key}: {TimeCompare(mapType.Key, mapType.Duration)}");
            }

            _output.WriteLine("=============");
            _output.WriteLine("===VISIBILITY===");
            _output.WriteLine("=============");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue(a.Key)).Select(g => new { g.Key, Visibility = g.Average(r => r.AverageVisibility) }))
            {
                _output.WriteLine($"{mapType.Key}: {mapType.Visibility}");
                //_output.WriteLine($"{mapType.Key}: {TimeCompare(mapType.Key, mapType.Duration)}");
            }
        }

        public static async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName)
        {
            var result = await Task.Run(() => new AStar(map, robot, goal, 1000).Run());
            sn.NotifyCompletion($"{mapName},{result}");
        }

        public double TimeCompare(string mapName, double v, int scale = 3)
        {
            var offset = TimeOffset(mapName);
            var max = v * scale - offset;
            return 100 - (v - offset).Percentage(max);
        }

        public double PathCompare(string mapName, double v, int scale = 3)
        {
            var offset = PathOffSet(mapName);
            var max = v * scale - offset;
            return 100 - (v - offset).Percentage(max);
        }

        public double TimeOffset(string mapName)
        {
            if (mapName == "WallOne") return 92.25;
            if (mapName == "WallTwo") return 91.45;
            if (mapName == "WallThree") return 113.2;

            if (mapName == "SlitOne") return 95.45;
            if (mapName == "SlitTwo") return 78.1;
            if (mapName == "SlitThree") return 89;

            if (mapName == "RoomOne") return 61.35;
            if (mapName == "RoomTwo") return 159.5;
            if (mapName == "RoomThree") return 466.65;

            if (mapName == "PlankPileOne") return 115.65;
            if (mapName == "PlankPileTwo") return 304.4;
            if (mapName == "PlankPileThree") return 1040.5;

            if (mapName == "CorridorOne") return 108.4;
            if (mapName == "CorridorTwo") return 106.6;
            if (mapName == "CorridorThree") return 177.2;

            if (mapName == "BugTrapOne") return 127.9;
            if (mapName == "BugTrapTwo") return 133.45;
            if (mapName == "BugTrapThree") return 154.5;
            return -1;
        }

        public double PathOffSet(string mapName)
        {
            if (mapName == "WallOne") return 30;
            if (mapName == "WallTwo") return 33.6;
            if (mapName == "WallThree") return 42.05;

            if (mapName == "SlitOne") return 30.7;
            if (mapName == "SlitTwo") return 28.65;
            if (mapName == "SlitThree") return 32.9;

            if (mapName == "RoomOne") return 25.2;
            if (mapName == "RoomTwo") return 56.1;
            if (mapName == "RoomThree") return 123.9;

            if (mapName == "PlankPileOne") return 45.45;
            if (mapName == "PlankPileTwo") return 88.5;
            if (mapName == "PlankPileThree") return 206.05;

            if (mapName == "CorridorOne") return 48.15;
            if (mapName == "CorridorTwo") return 48.1;
            if (mapName == "CorridorThree") return 85.85;

            if (mapName == "BugTrapOne") return 53.25;
            if (mapName == "BugTrapTwo") return 57.25;
            if (mapName == "BugTrapThree") return 64.3;
            return -1;
        }

        public class Data
        {
            public Data(string line)
            {
                var items = line.Split(",");

                MapName = items[0];
                Success = bool.Parse(items[1]);
                IsStuck = bool.Parse(items[2]);
                Time = long.Parse(items[3]);
                Steps = int.Parse(items[4]);
                AverageVisibility = double.Parse(items[5]);
                NewPlanCount = int.Parse(items[6]);
            }
            public string MapName { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }
            public long Time { get; set; }
            public int Steps { get; set; }
            public double AverageVisibility { get; set; }
            public int NewPlanCount { get; set; }
        }
    }
}
