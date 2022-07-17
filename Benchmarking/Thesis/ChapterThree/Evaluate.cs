using System;
using System.Collections.Generic;
using System.Linq;
using Benchmarking.Core.Map;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class Evaluate
    {
        private double _pathMultiplier = 22.12;
        private double _timeMultiplier = 128.41;
        private double _smoothnessMax = 28.03;

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
            WallOne, WallTwo, WallThree, SlitOne, SlitTwo, SlitThree, RoomOne, RoomTwo, RoomThree, PlankPileOne, PlankPileTwo, PlankPileThree, CorridorOne, CorridorTwo, CorridorThree, BugTrapOne, BugTrapTwo, BugTrapThree,
            ObstacleOne, ObstacleTwo, ObstacleThree, ObstacleFour, ObstacleFive, 
            //ObstacleSix, ObstacleSeven, ObstacleEight, ObstacleNine, ObstacleTen,
            TunnelOne, TunnelTwo, TunnelThree, TunnelFour, TunnelFive, 
            //TunnelSix, TunnelSeven, TunnelEight, TunnelNine, TunnelTen
        }

        public double GetScore(ApproachType approach)
        {
            var success = CalculateSuccess(approach).Any() ? CalculateSuccess(approach).Average(a => a.Value) : 0;
            var path = Path(approach).Any() ? Path(approach).Average(a => a.Value) : 0;
            var duration = Duration(approach).Any() ? Duration(approach).Average(a => a.Value) : 0;
            var visibility = Visibility(approach).Any() ? Visibility(approach).Average(a => a.Value) : 0;
            var pathSmoothness = PathSmoothness(approach).Any() ? PathSmoothness(approach).Average(a => a.Value): 0;
            return (success + path + duration + visibility + pathSmoothness) / 5.0;
        }

        public double Score(ApproachType approach, double pw, double dw, double psw)
        {
            var success = CalculateSuccess(approach).Any() ? CalculateSuccess(approach).Average(a => a.Value) : 0;
            var path = Path(approach).Any() ? Path(approach).Average(a => a.Value) : 0;
            var duration = Duration(approach).Any() ? Duration(approach).Average(a => a.Value) : 0;
            var visibility = Visibility(approach).Any() ? Visibility(approach).Average(a => a.Value) : 0;
            var pathSmoothness = PathSmoothness(approach).Any() ? PathSmoothness(approach).Average(a => a.Value): 0;

            //success /= 100;
            //path /= 100;
            //duration /= 100;
            //visibility /= 100;
            //pathSmoothness /= 100;

            return success * ((pw * path + (1.0 - pw) * visibility + (dw * duration + psw * pathSmoothness)) / 2.0);
        }
        
        public Dictionary<MapType, double> CalculateSuccess(ApproachType approach)
        {
            return OrderedData(approach, false).ToDictionary(g => g.Key, g => g.Count(r => r.Success).Percentage(g.Count()));
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
            return OrderedData(approach).ToDictionary(g => g.Key, g => g.Average(r => r.Visibility).PercentageFromRange(2, 28.03));
        }

        public Dictionary<MapType, double> PathSmoothness(ApproachType approach)
        {
            return OrderedData(approach).ToDictionary(g => g.Key, GetPathSmoothness);
        }

        private IOrderedEnumerable<IGrouping<MapType, EvaluateData>> OrderedData(ApproachType approach, bool successOnly = true)
        {
            var data = approach == ApproachType.BaseLine ? BaselineData : Data.Where(d => d.Approach == approach);
            if (successOnly)
                data = data.Where(d => d.Success);
            return data.GroupBy(a => a.Map).OrderBy(a => CompareFields.ToOrderValue(a.Key));
        }
        
        private double GetAverage(List<EvaluateData> data, Func<EvaluateData, double> func)
        {
            return data.Select(func.Invoke).Sum() / data.Count;
        }

        private double GetBaseLinedPath(EvaluateData data)
        {
            var baseLine = BaselineData.First(b => b.Id == data.Id && IsSameMapGroups(b, data));
            var baseLinedPath = Normalize(baseLine.Steps, data.Steps, _pathMultiplier);
            if (baseLinedPath > 100)
                baseLinedPath = baseLinedPath;
            return baseLinedPath;
        }

        private double GetBaseLinedDuration(EvaluateData data)
        {
            var baseLine = BaselineData.First(b => b.Id == data.Id && IsSameMapGroups(b, data));
            var baseLinedDuration = Normalize(baseLine.Time, data.Time, _timeMultiplier);
            if (baseLinedDuration > 100)
                baseLinedDuration = baseLinedDuration;
            return baseLinedDuration;
        }

        private double GetPathSmoothness(IEnumerable<EvaluateData> data)
        {
            var vsData = data.Where(d => !double.IsNaN(d.PathSmoothness));
            var avg = vsData.Average(d => d.PathSmoothness);
            var pathSmoothness = 1 - avg.PercentageFromRange(0, 28);
            //Normalize(0, avg, 1);
            //Math.Min(30, avg).Percentage(30);
            //if (pathSmoothness > 100)
            //    pathSmoothness = pathSmoothness;
            return pathSmoothness;
        }

        private double Normalize(double baseLine, double v, double maxMultiplier)
        {
            var max = baseLine * maxMultiplier;
            var min = baseLine - (max - baseLine) / 9;
            return v.PercentageFromRange(max, min);
        }

        private static bool IsSameMapGroups(EvaluateData b, EvaluateData d)
        {
            return b.Map == d.Map;
            //return IsStaticMap(b.Map) == IsStaticMap(d.Map);
        }

        public static bool IsStaticMap(MapType map)
        {
            return new MapType[]
            {
                MapType.WallOne, MapType.WallTwo, MapType.WallThree, MapType.SlitOne, MapType.SlitTwo, MapType.SlitThree, MapType.RoomOne, MapType.RoomTwo, MapType.RoomThree, MapType.PlankPileOne,
                MapType.PlankPileTwo, MapType.PlankPileThree, MapType.CorridorOne, MapType.CorridorTwo, MapType.CorridorThree, MapType.BugTrapOne, MapType.BugTrapTwo,
                MapType.BugTrapThree
            }.Contains(map);
        }
    }
}
