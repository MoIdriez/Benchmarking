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

            for (var i = 0; i < 100; i++)
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

            var all = File.ReadAllLines(FileRef.BaseLine).Select(l => new Data(l) as IData).ToList();
            
            _output.WriteLine("===SUCCESS===");
            CalculateSuccess(all);
            
            _output.WriteLine("===MOVEABLE===");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), a.Key))).Select(g => new { g.Key, IsStuck = g.Count(r => r.IsStuck).Percentage(g.Count()) }))
            {
                _output.WriteLine($"{mapType.Key}: {Math.Max(0, 100 - mapType.IsStuck)}");
            }
            _output.WriteLine("===PATH===");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), a.Key))).Select(g => new { g.Key, PathLength = g.Average(r => r.Steps) }))
            {
                _output.WriteLine($"{mapType.Key}: {Compare(PathOffSet, mapType.Key, mapType.PathLength)}");
            }
            _output.WriteLine("===DURATION===");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), a.Key))).Select(g => new { g.Key, Duration = g.Average(r => r.Time) }))
            {
                _output.WriteLine($"{mapType.Key}: {Compare(TimeOffset, mapType.Key, mapType.Duration)}");
            }
            _output.WriteLine("===VISIBILITY===");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), a.Key))).Select(g => new { g.Key, Visibility = (g.Average(r => r.AverageVisibility) - 5).Percentage(28.03) }))
            {
                _output.WriteLine($"{mapType.Key}: {mapType.Visibility}");
            }

            _output.WriteLine("===PathSmoothness===");
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), a.Key))).Select(g => new { g.Key, PathSmoothness = g.Average(r => r.PathSmoothness) }))
            {
                _output.WriteLine($"{mapType.Key}: {mapType.PathSmoothness})");
            }
        }

        private void CalculateSuccess(List<IData> all)
        {
            foreach (var mapType in all.GroupBy(a => a.MapName).OrderBy(a => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), a.Key)))
                         .Select(g => new {g.Key, Success = g.Count(r => r.Success).Percentage(g.Count())}))
            {
                _output.WriteLine($"{mapType.Key}: {mapType.Success}");
            }
        }

        public static async Task RunMethod(StateNotifier sn, int[,] map, Robot robot, Point goal, string mapName)
        {
            var result = await Task.Run(() => new AStar(map, robot, goal, 1000).Run());
            sn.NotifyCompletion($"{mapName},{result}");
        }
        
        public double Compare(Func<string, double> func, string mapName, double v, int scale = 3)
        {
            var offset = func.Invoke(mapName);
            var max = v * scale - offset;
            return 100 - (v - offset).Percentage(max);
        }

        public double TimeOffset(string mapName)
        {
            if (mapName == "WallOne") return 92.33;
            if (mapName == "WallTwo") return 89.59;
            if (mapName == "WallThree") return 109.51;

            if (mapName == "SlitOne") return 86.41;
            if (mapName == "SlitTwo") return 90.21;
            if (mapName == "SlitThree") return 87.34;

            if (mapName == "RoomOne") return 64.12;
            if (mapName == "RoomTwo") return 159.03;
            if (mapName == "RoomThree") return 465.19;

            if (mapName == "PlankPileOne") return 117.55;
            if (mapName == "PlankPileTwo") return 309.01;
            if (mapName == "PlankPileThree") return 1129.79;

            if (mapName == "CorridorOne") return 109.43;
            if (mapName == "CorridorTwo") return 111.03;
            if (mapName == "CorridorThree") return 190.85;

            if (mapName == "BugTrapOne") return 132.6;
            if (mapName == "BugTrapTwo") return 137.69;
            if (mapName == "BugTrapThree") return 150.9;
            return -1;
        }

        public double PathOffSet(string mapName)
        {
            if (mapName == "WallOne") return 31.36;
            if (mapName == "WallTwo") return 31.72;
            if (mapName == "WallThree") return 40.98;

            if (mapName == "SlitOne") return 30.01;
            if (mapName == "SlitTwo") return 31.9;
            if (mapName == "SlitThree") return 32.47;

            if (mapName == "RoomOne") return 26.32;
            if (mapName == "RoomTwo") return 53.43;
            if (mapName == "RoomThree") return 113.89;

            if (mapName == "PlankPileOne") return 45.38;
            if (mapName == "PlankPileTwo") return 88.67;
            if (mapName == "PlankPileThree") return 200.3;

            if (mapName == "CorridorOne") return 47.17;
            if (mapName == "CorridorTwo") return 48.83;
            if (mapName == "CorridorThree") return 87.86;

            if (mapName == "BugTrapOne") return 54.2;
            if (mapName == "BugTrapTwo") return 57.38;
            if (mapName == "BugTrapThree") return 63.74;
            return -1;
        }

        public interface IData
        {
            public string MapName { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }
            public long Time { get; set; }
            public int Steps { get; set; }
            public double AverageVisibility { get; set; }
            public double DistanceToGoal { get; set; }
            public double PathSmoothness { get; set; }
        }

        public class Data : IData
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
                DistanceToGoal = double.Parse(items[6]);
                PathSmoothness = double.Parse(items[7]);
                NewPlanCount = int.Parse(items[8]);
            }

            public string MapName { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }
            public long Time { get; set; }
            public int Steps { get; set; }
            public double AverageVisibility { get; set; }
            public double DistanceToGoal { get; set; }
            public double PathSmoothness { get; set; }
            public int NewPlanCount { get; set; }
        }
    }
}
