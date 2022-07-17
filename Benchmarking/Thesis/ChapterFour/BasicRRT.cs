using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Core.Navigation;
using Benchmarking.Core.Navigation.Dijkstra;
using Benchmarking.Core.Navigation.Models;
using Benchmarking.Core.Navigation.Reactive;
using Benchmarking.Core.Navigation.Tree;
using Benchmarking.Helper;
using Benchmarking.Thesis.ChapterFour.Data;
using Benchmarking.Thesis.ChapterThree;
using Benchmarking.Thesis.Maps;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterFour
{
    public class BasicRRT
    {
        private readonly ITestOutputHelper _output;
        private readonly Random _random = new();

        public BasicRRT(ITestOutputHelper output)
        {
            _output = output;
        }

        public class Data
        {
            public Data(string line)
            {
                var items = line.Split(",");

                //Id = int.Parse(items[0]);
                Approach = (Evaluate.ApproachType)Enum.Parse(typeof(Evaluate.ApproachType), items[0]);
                MapName = items[1];
                Success = bool.Parse(items[2]);
                Time = double.Parse(items[3]);
                Steps = int.Parse(items[4]);
                AverageVisibility = double.Parse(items[5]);
                PathSmoothness = double.Parse(items[6]);
                NewPlanCount = int.Parse(items[7]);
                GrowthSize = int.Parse(items[8]);
                GoalDistance = int.Parse(items[9]);
            }

            public int Id { get; set; }
            public Evaluate.ApproachType Approach { get; set; }
            public string MapName { get; set; }
            public bool Success { get; set; }
            public double Time { get; set; }
            public int Steps { get; set; }
            public double AverageVisibility { get; set; }
            public double PathSmoothness { get; set; }
            public int NewPlanCount { get; set; }
            public int GrowthSize { get; }
            public int GoalDistance { get; }

        }

        [Fact]
        public async Task RRT()
        {
            var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.WallThree, _random);

            var settings = new RRTSettings(15, 30);
            var result = new RRT(map, robot, goal, 3000, settings).Run();

            _output.WriteLine(result);
            await Viewer.Image($"rrt-{Guid.NewGuid()}.png", map, robot, goal);
        }

        [Fact]
        public async Task RRTExtended()
        {
            var (map, robot, goal) = ThesisMaps.GenerateMap(ThesisMaps.CorridorThree, _random);
            var settings = new RRTSettings(15, 30);
            var result = new RRTExtended(map, robot, goal, 3000, settings).Run();

            _output.WriteLine(result);
            await Viewer.Image($"rrtstar-{Guid.NewGuid()}.png", map, robot, goal);
        }

        [Fact]
        public void ParameterExploration()
        {
            var sw = new Stopwatch();
            var sb = new StringBuilder();

            var r = new Random();
            var fileName = $"RRTSearch-{Guid.NewGuid()}.txt";

            

            sw.Start();

            for (var gs = 10; gs < 100; gs+=20)
            {
                for (var gd = 5; gd < 30; gd+=10)
                {
                    var rrtSettings = new RRTSettings(gs, gd);
                    var rrtExtendedSettings = new RRTSettings(gs, gd);
                    for (var i = 0; i < 5; i++)
                    {
                        var maps = ThesisMaps.GetGeneratedMaps(r);
                        foreach (var map in maps)
                        {
                            Run(Evaluate.ApproachType.RRT,
                                new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000,
                                    rrtSettings), i, map);
                            Run(Evaluate.ApproachType.RRTExtended,
                                new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000,
                                    rrtExtendedSettings), i, map);
                        }

                        File.AppendAllText(fileName, sb.ToString());
                        sb = new StringBuilder();
                        _output.WriteLine($"{i}: {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds):g}");
                    }
                }
            }


            void Run(Evaluate.ApproachType approach, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                try
                {
                    method.Run();
                    sb.AppendLine($"{approach},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F}{method.AdditionalMetrics()}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Exception: {i}|{approach}: {ex.Message} - {ex.StackTrace}");
                }
            }
        }

        [Fact]
        public void RRTAnalysis()
        {
            var all = File.ReadAllLines(FileRef.RRTSearch2).Select(l => new Data(l)).Where(d => d.Approach == Evaluate.ApproachType.RRT).ToList();

            _output.WriteLine($"Final Analysis: Average: {all.Count(d => d.Success).Percentage(all.Count)}");
            _output.WriteLine("----------------------------------------------------------");
            var success = all.Where(a => a.Success).ToList();
            _output.WriteLine($"Average Step: {all.Average(s => s.Steps):F} | Average success Step: {success.Average(s => s.Steps):F}");
            _output.WriteLine($"StdDev: {success.Select(s => s.Steps).StandardDeviation():F} | Min: {success.Select(s => s.Steps).Min():F} | 25: {success.Select(s => s.Steps).Percentile(0.25):F} | 75: {success.Select(s => s.Steps).Percentile(0.75):F} | Max: {success.Select(s => s.Steps).Max():F}");
            _output.WriteLine($"ALL StdDev: {all.Select(s => s.Steps).StandardDeviation():F} | Min: {all.Select(s => s.Steps).Min():F} | 25: {all.Select(s => s.Steps).Percentile(0.25):F} | 75: {all.Select(s => s.Steps).Percentile(0.75):F} | Max: {all.Select(s => s.Steps).Max():F}");
            _output.WriteLine("----------------------------------------------------------");
            _output.WriteLine($"Average Visibility: {all.Average(s => s.AverageVisibility):F} | Average success Visibility: {success.Average(s => s.AverageVisibility):F}");
            _output.WriteLine($"StdDev: {success.Select(s => s.AverageVisibility).StandardDeviation():F} | Min: {success.Select(s => s.AverageVisibility).Min():F} | 25: {success.Select(s => s.AverageVisibility).Percentile(0.25):F} | 75: {success.Select(s => s.AverageVisibility).Percentile(0.75):F} | Max: {success.Select(s => s.AverageVisibility).Max():F}");
            _output.WriteLine($"ALL StdDev: {all.Select(s => s.AverageVisibility).StandardDeviation():F} | Min: {all.Select(s => s.AverageVisibility).Min():F} | 25: {all.Select(s => s.AverageVisibility).Percentile(0.25):F} | 75: {all.Select(s => s.AverageVisibility).Percentile(0.75):F} | Max: {all.Select(s => s.AverageVisibility).Max():F}");
            _output.WriteLine("----------------------------------------------------------");
            _output.WriteLine($"Average Path Smoothness: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Average(s => s.PathSmoothness):F} | Average success Path Smoothness: {success.Average(s => s.PathSmoothness):F}");
            _output.WriteLine($"StdDev: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).StandardDeviation():F} | Min: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Min():F} | 25: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.25):F} | 75: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.75):F} | Max: {success.Select(s => s.PathSmoothness).Max():F}");
            _output.WriteLine($"ALL StdDev: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).StandardDeviation():F} | Min: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Min():F} | 25: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.25):F} | 75: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.75):F} | Max: {all.Select(s => s.PathSmoothness).Max():F}");
            _output.WriteLine("==========================================================");

            var mapGroups = all
               //.Where(g => !g.MapName.Contains("One"))
               .GroupBy(g => g.MapName)
               .OrderBy(g => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), g.Key))).ToList();
            var mapSuccess = mapGroups.Select(g => new { Map = g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), All = g }).ToList();

            _output.WriteLine("BY MAP TYPE");

            _output.WriteLine("----------------------------------------------------------");
            _output.WriteLine($"Standard deviation: {mapSuccess.Select(m => m.Success).StandardDeviation():F}");
            _output.WriteLine($"Max: {mapSuccess.Select(m => m.Success).Max():F}");
            _output.WriteLine($"Min: {mapSuccess.Select(m => m.Success).Min():F}");
            _output.WriteLine($"25%: {mapSuccess.Select(m => m.Success).ToArray().Percentile(0.25):F}");
            _output.WriteLine($"75%: {mapSuccess.Select(m => m.Success).ToArray().Percentile(0.75):F}");
            _output.WriteLine("----------------------------------------------------------");
            foreach (var g in mapSuccess)
            {
                _output.WriteLine($"{g.Map}: \t{g.Success:F}%");
                var top5 = g.All.GroupBy(g => new { g.GrowthSize, g.GoalDistance })
                    .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) })
                    .OrderByDescending(g => g.Success).ToList();

                //_output.WriteLine($"Top: {string.Join(" || ", top5.Select(t => $"{t.Key.ObstacleRange},{t.Key.ObstacleConstant},{t.Key.AttractiveConstant}: {t.Success:F}%"))}");

            }

            _output.WriteLine("==========================================================");
            _output.WriteLine("BY PARAMETERS");
            var paramGroups = all
                    //.Where(g => !g.MapName.Contains("One"))
                    .GroupBy(g => new { g.GrowthSize, g.GoalDistance })
                    .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), All = g })
                    .OrderByDescending(g => g.Success).ToList();

            _output.WriteLine("----------------------------------------------------------");
            _output.WriteLine($"Standard deviation: {paramGroups.Select(m => m.Success).StandardDeviation():F}");
            _output.WriteLine($"Max: {paramGroups.Select(m => m.Success).Max():F}");
            _output.WriteLine($"Min: {paramGroups.Select(m => m.Success).Min():F}");
            _output.WriteLine($"25%: {paramGroups.Select(m => m.Success).ToArray().Percentile(0.25):F}");
            _output.WriteLine($"75%: {paramGroups.Select(m => m.Success).ToArray().Percentile(0.75):F}");
            _output.WriteLine("----------------------------------------------------------");

            _output.WriteLine($"==========================================================");
            _output.WriteLine($"Max: {success.Max(s => s.Steps)}, Min:{success.Min(s => s.Steps)}");

            foreach (var g in paramGroups.Take(5))
            {
                _output.WriteLine($"{g.Key.GrowthSize},{g.Key.GoalDistance}: \t{g.Success:F}%");

                var maps = g.All
                    //.Where(g => !g.MapName.Contains("One"))
                    .GroupBy(g => g.MapName)
                    .Select(s => new { Map = s.Key, Success = s.Count(r => r.Success).Percentage(s.Count()) })
                    .OrderByDescending(g => g.Success).Take(7).ToList();

                _output.WriteLine($"Top: {string.Join(" || ", maps.Select(t => $"{t.Map}: {t.Success:F}%"))}");
            }

            _output.WriteLine("==========================================================");
            var paths = all
                .GroupBy(g => new { g.GrowthSize, g.GoalDistance })
                .Select(g => new { g.Key, Steps = g.Average(a => a.Steps) })
                .OrderBy(g => g.Steps).ToList();

            foreach (var g in paths.Take(5))
            {
                _output.WriteLine($"{g.Key.GrowthSize},{g.Key.GoalDistance}: \t{g.Steps:F}");
            }

        }

        [Fact]
        public void ParameterAnalysis()
        {
            var all = File.ReadAllLines(FileRef.RRTSearch3).Select(l => new Data(l)).ToList();

            var rrt = all
                .Where(a => a.Approach == Evaluate.ApproachType.RRT)
                .GroupBy(g => new { g.GrowthSize, g.GoalDistance })
                .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), TimeSuccess = g.Where(a => a.Success).Max(a => a.Time) })
                .OrderByDescending(g => g.Success).ToList();

            foreach (var g in rrt.Take(5))
            {
                _output.WriteLine($"{g.Key.GrowthSize},{g.Key.GoalDistance}: \t{g.Success:F} \t{g.TimeSuccess:F}");
            }

            _output.WriteLine("==========================================================");
            rrt = all
                .Where(a => a.Approach == Evaluate.ApproachType.RRTExtended)
                .GroupBy(g => new { g.GrowthSize, g.GoalDistance })
                .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), TimeSuccess = g.Where(a => a.Success).Max(a => a.Time) })
                .OrderByDescending(g => g.Success).ToList();

            foreach (var g in rrt.Take(5))
            {
                _output.WriteLine($"{g.Key.GrowthSize},{g.Key.GoalDistance}: \t{g.Success:F} \t{g.TimeSuccess:F}");
            }
        }
    }
}
