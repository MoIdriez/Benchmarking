using System;
using System.Collections.Generic;
using System.Linq;
using Benchmarking.Core.Map;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class Evaluate
    {
        private readonly List<EvaluateData> _data;
        private readonly List<EvaluateData> _baselineData;

        public Evaluate(List<EvaluateData> data)
        {
            _data = data.Where(d => d.Approach != "BaseLine").ToList();
            _baselineData = data.Where(d => d.Approach == "BaseLine").ToList();
        }
        public class EvaluateData
        {
            public EvaluateData(string line)
            {
                var items = line.Split(",");

                Id = int.Parse(items[0]);
                Approach = items[1];
                Map = items[2];
                Success = bool.Parse(items[3]);
                Time = long.Parse(items[4]);
                Steps = int.Parse(items[5]);
                Visibility = double.Parse(items[6]);
                PathSmoothness = double.Parse(items[7]);
            }
            public int Id { get; set; }
            public string Approach { get; set; }
            public string Map { get; set; }
            public bool Success { get; set; }
            public long Time { get; set; }
            public int Steps { get; set; }
            public double Visibility { get; set; }
            public double PathSmoothness { get; set; }
        }

        public void Compare(ITestOutputHelper output)
        {
            output.WriteLine("============= Success");
            foreach (var mapType in CalculateSuccess())
            {
                output.WriteLine($"{mapType.mapName}: {mapType.value}");
            }
            output.WriteLine("============= Path");
            foreach (var mapType in Path())
            {
                output.WriteLine($"{mapType.mapName}: {mapType.value}");
            }
            output.WriteLine("============= Duration");
            foreach (var mapType in Duration())
            {
                output.WriteLine($"{mapType.mapName}: {mapType.value}");
            }
            output.WriteLine("============= Visibility");
            foreach (var mapType in Visibility())
            {
                output.WriteLine($"{mapType.mapName}: {mapType.value}");
            }
            output.WriteLine("============= PathSmoothness");
            foreach (var mapType in PathSmoothness())
            {
                output.WriteLine($"{mapType.mapName}: {mapType.value}");
            }
            output.WriteLine("=============");
        }
        
        public List<(string mapName, double value)> CalculateSuccess()
        {
            return OrderedData().Select(g => (g.Key, g.Count(r => r.Success).Percentage(g.Count()))).ToList();
        }

        public List<(string mapName, double value)> Path()
        {
            return OrderedData().Select(g => (g.Key, GetAverage(g, GetBaseLinedPath))).ToList();
        }

        public List<(string mapName, double value)> Duration()
        {
            return OrderedData().Select(g => (g.Key, GetAverage(g, GetBaseLinedDuration))).ToList();
        }

        // 28.03 is max visibility
        public List<(string mapName, double value)> Visibility()
        {
            return OrderedData().Select(g => (g.Key, g.Average(r => r.Visibility).Percentage(28.03))).ToList();
        }

        public List<(string mapName, double value)> PathSmoothness()
        {
            return OrderedData().Select(g => (g.Key, g.Where(a => !double.IsNaN(a.PathSmoothness)).Average(r => r.PathSmoothness))).ToList();
        }

        private IOrderedEnumerable<IGrouping<string, EvaluateData>> OrderedData()
        {
            return _data.GroupBy(a => a.Map).OrderBy(a => CompareFields.ToOrderValue(a.Key));
        }
        
        private double GetAverage(IEnumerable<EvaluateData> data, Func<EvaluateData, int, double> func)
        {
            return data.Select(func.Invoke).Sum() / data.Count();
        }

        private double GetBaseLinedPath(EvaluateData data, int scale = 4)
        {
            var baseLine = _baselineData.First(b => b.Id == data.Id);
            return 100 - (data.Steps - baseLine.Steps).Percentage(baseLine.Steps * scale - baseLine.Steps);
        }

        private double GetBaseLinedDuration(EvaluateData data, int scale = 10)
        {
            var baseLine = _baselineData.First(b => b.Id == data.Id);
            return 100 - (data.Time - baseLine.Time).Percentage(baseLine.Time * scale - baseLine.Time);
        }

        public double Compare(Func<string, double> func, string mapName, double v, int scale = 3)
        {
            var offset = func.Invoke(mapName);
            var max = MaxV(scale, offset);
            return 100 - (v - offset).Percentage(max);
        }

        private static double MaxV(int scale, double offset)
        {
            return offset * scale - offset;
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

    }
}
