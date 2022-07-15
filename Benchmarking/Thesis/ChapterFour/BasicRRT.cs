using System;
using System.Collections.Generic;
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

                Id = int.Parse(items[0]);
                Approach = (Evaluate.ApproachType)Enum.Parse(typeof(Evaluate.ApproachType), items[1]);
                MapName = items[2];
                Success = bool.Parse(items[3]);
                Time = double.Parse(items[4]);
                Steps = int.Parse(items[5]);
                AverageVisibility = double.Parse(items[6]);
                PathSmoothness = double.Parse(items[7]);
                NewPlanCount = int.Parse(items[8]);
                GrowthSize = int.Parse(items[9]);
                GoalDistance = int.Parse(items[10]);
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
        public async Task ParameterExploration()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"RRTSearch-{Guid.NewGuid()}.txt");

            var r = new Random();

            for (var growthSize = 10; growthSize < 30; growthSize+=2)
            {
                for (var goalDistance = 10; goalDistance < 30; goalDistance+=2)
                {
                    var settings = new RRTSettings(growthSize, goalDistance);

                    for (var i = 0; i < 10; i++)
                    {
                        var maps = ThesisMaps.GetStaticMaps(r);
                        foreach (var map in maps)
                        {
                            tasks.Add(Run("RRT", new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, settings), i, map));
                            tasks.Add(Run("RRTExtended", new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, settings), i, map));
                        }
                    }

                }
            }

            sn.Run(tasks.Count, 100);
            await Task.WhenAll(tasks);
            sn.Result();

            async Task Run(string methodName, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                await Task.Run(method.Run);
                sn.NotifyCompletion($"{i},{methodName},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F},{method.AdditionalMetrics()}");
            }
        }
        
        [Fact]
        public void RRTAnalysis()
        {
            var all = File.ReadAllLines(FileRef.RRTSearch).Select(l => new Data(l)).Where(d => d.Approach == Evaluate.ApproachType.RRT).ToList();

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
    }
}
