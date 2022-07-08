using System;
using System.Collections.Generic;
using System.Linq;
using Benchmarking.Core.Map;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class Evaluate
    {
        public List<EvaluateData> Data;
        public List<EvaluateData> BaselineData;

        public Evaluate(List<EvaluateData> data)
        {
            Data = data.Where(d => d.Approach != ApproachType.BaseLine).ToList();
            BaselineData = data.Where(d => d.Approach == ApproachType.BaseLine).ToList();
        }
        public class EvaluateData
        {
            public EvaluateData(string line)
            {
                var items = line.Split(",");

                Id = int.Parse(items[0]);
                Approach = (ApproachType)Enum.Parse(typeof(ApproachType), items[1]);
                Map = (MapType)Enum.Parse(typeof(MapType), items[2]);
                Success = bool.Parse(items[3]);
                Time = double.Parse(items[4]);
                Steps = int.Parse(items[5]);
                Visibility = double.Parse(items[6]);
                PathSmoothness = double.Parse(items[7]);
            }
            public int Id { get; set; }
            public ApproachType Approach { get; set; }
            public MapType Map { get; set; }
            public bool Success { get; set; }
            public double Time { get; set; }
            public int Steps { get; set; }
            public double Visibility { get; set; }
            public double PathSmoothness { get; set; }
        }

        public enum ApproachType {
            BaseLine, PheromoneField, PotentialField, RRT, RRTExtended, AStar
        }

        public enum MapType {
            WallOne, WallTwo, WallThree, SlitOne, SlitTwo, SlitThree, RoomOne, RoomTwo, RoomThree, PlankPileOne, PlankPileTwo, PlankPileThree, CorridorOne, CorridorTwo, CorridorThree, BugTrapOne, BugTrapTwo, BugTrapThree
        }
        
        public Dictionary<MapType, double> CalculateSuccess(ApproachType approach)
        {
            return OrderedData(approach).ToDictionary(g => g.Key, g => g.Count(r => r.Success).Percentage(g.Count()));
        }

        public Dictionary<MapType, double> Path(ApproachType approach)
        {
            return OrderedData(approach).ToDictionary(g => g.Key, g => GetAverage(g.ToList(), GetBaseLinedPath));
        }

        public Dictionary<MapType, double> Duration(ApproachType approach)
        {
            return OrderedData(approach).ToDictionary(g => g.Key, g => GetAverage(g.ToList(), GetBaseLinedDuration));
        }

        // 28.03 is max visibility
        public Dictionary<MapType, double> Visibility(ApproachType approach)
        {
            return OrderedData(approach).ToDictionary(g => g.Key, g => g.Average(r => r.Visibility).Percentage(28.03));
        }

        public Dictionary<MapType, double> PathSmoothness(ApproachType approach)
        {
            return OrderedData(approach).ToDictionary(g => g.Key, GetVisibility);
        }

        private IOrderedEnumerable<IGrouping<MapType, EvaluateData>> OrderedData(ApproachType approach)
        {
            var data = approach == ApproachType.BaseLine ? BaselineData : Data.Where(d => d.Approach == approach);
            return data.GroupBy(a => a.Map).OrderBy(a => CompareFields.ToOrderValue(a.Key));
        }
        
        private double GetAverage(List<EvaluateData> data, Func<EvaluateData, double> func)
        {
            return data.Select(func.Invoke).Sum() / data.Count;
        }

        private double GetBaseLinedPath(EvaluateData data)
        {
            var baseLine = BaselineData.First(b => b.Id == data.Id);
            var baseLineMax = baseLine.Steps * 15;
            return 100 - (Math.Min(baseLineMax, data.Steps) - baseLine.Steps).Percentage(baseLineMax);
        }

        private double GetBaseLinedDuration(EvaluateData data)
        {
            var baseLine = BaselineData.First(b => b.Id == data.Id);
            var baseLineMax = baseLine.Time * 10;
            return 100 - (Math.Min(baseLineMax, data.Time) - baseLine.Time).Percentage(baseLineMax);
        }

        private double GetVisibility(IEnumerable<EvaluateData> data)
        {
            var vsData = data.Where(d => !double.IsNaN(d.PathSmoothness));
            var avg = vsData.Average(d => d.PathSmoothness);
            return 100 - Math.Min(30, avg).Percentage(30);
        }
    }
}
