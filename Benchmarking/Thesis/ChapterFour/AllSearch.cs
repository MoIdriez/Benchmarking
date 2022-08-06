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
    public class AllSearch
    {
        private readonly ITestOutputHelper _output;
        private readonly Random _random = new();

        public AllSearch(ITestOutputHelper output)
        {
            _output = output;
        }
        


        [Fact]
        public void AllConsecutiveRuns()
        {
            var r = new Random();
            var sw = new Stopwatch();
            var sb = new StringBuilder();

            var potentialFieldSettings = new PotentialFieldSettings(3, 4, 9);
            var pheromoneSettings = new PheromoneSettings(1, 5, 8);
            
            var rrtSettings = new RRTSettings(12, 16);
            var rrtExtendedSettings = new RRTSettings(14, 14);
            
            var rrtSettingsGen = new RRTSettings(40, 30);
            var rrtExtendedSettingsGen = new RRTSettings(20, 30);

            var fileName = $"AllRunsFinal-{Guid.NewGuid()}.txt";

            sw.Start();
            for (var i = 51; i < 100; i++)
            {
                foreach (var map in ThesisMaps.GetStaticMaps(r))
                {
                    Run(Evaluate.ApproachType.BaseLine, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, baseLine: true), i, map);
                    Run(Evaluate.ApproachType.AStar, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000), i, map);
                    Run(Evaluate.ApproachType.PotentialField, new PotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings), i, map);
                    Run(Evaluate.ApproachType.PheromoneField, new PheromonePotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings, pheromoneSettings), i, map);
                    Run(Evaluate.ApproachType.RRT, new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtSettings), i, map);
                    Run(Evaluate.ApproachType.RRTExtended, new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtExtendedSettings), i, map);
                }

                foreach (var map in ThesisMaps.GetGeneratedMaps(r))
                {
                    Run(Evaluate.ApproachType.BaseLine, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, baseLine: true), i, map);
                    Run(Evaluate.ApproachType.AStar, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000), i, map);
                    Run(Evaluate.ApproachType.PotentialField, new PotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings), i, map);
                    Run(Evaluate.ApproachType.PheromoneField, new PheromonePotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings, pheromoneSettings), i, map);
                    Run(Evaluate.ApproachType.RRT, new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtSettingsGen), i, map);
                    Run(Evaluate.ApproachType.RRTExtended, new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtExtendedSettingsGen), i, map);
                }

                File.AppendAllText(fileName, sb.ToString());
                sb = new StringBuilder();
                _output.WriteLine($"{i}: {TimeSpan.FromMilliseconds(sw.ElapsedMilliseconds):g}");
            }

            void Run(Evaluate.ApproachType approach, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                try
                {
                    method.Run();
                    sb.AppendLine($"{i},{approach},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Exception: {i}|{approach}: {ex.Message} - {ex.StackTrace}");
                }

            }

        }


        [Fact]
        public async Task AllCombinedRuns()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"AllCombinedRuns-{Guid.NewGuid()}.txt");

            var r = new Random();

            var potentialFieldSettings = new PotentialFieldSettings(3, 4, 9);
            var pheromoneSettings = new PheromoneSettings(1, 5, 8);
            var rrtSettings = new RRTSettings(12, 16);
            var rrtExtendedSettings = new RRTSettings(14, 14);

            for (var i = 0; i < 1000; i++)
            {
                var staticMaps = ThesisMaps.GetStaticMaps(r);
                var generatedMaps = ThesisMaps.GetGeneratedMaps(r);
                var maps = staticMaps.Concat(generatedMaps).ToList();
                foreach (var map in maps)
                {
                    tasks.Add(Run(Evaluate.ApproachType.BaseLine, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, baseLine: true), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.AStar, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.PotentialField, new PotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.PheromoneField, new PheromonePotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings, pheromoneSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.RRT, new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.RRTExtended, new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtExtendedSettings), i, map));
                }
            }
            _output.WriteLine($"Ready to run");
            sn.Run(tasks.Count, 100);
            await Task.WhenAll(tasks);
            sn.Result();

            async Task Run(Evaluate.ApproachType approach, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                try
                {
                    await Task.Run(method.Run);
                    sn.NotifyCompletion(
                        $"{i},{approach},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Exception: {i}|{approach}: {ex.Message} - {ex.StackTrace}");
                }

            }
        }


        [Fact]
        public async Task AllGeneratedRuns()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"AllGeneratedSearch-{Guid.NewGuid()}.txt");

            var r = new Random();

            var potentialFieldSettings = new PotentialFieldSettings(3, 4, 9);
            var pheromoneSettings = new PheromoneSettings(1, 5, 8);
            var rrtSettings = new RRTSettings(12, 16);
            var rrtExtendedSettings = new RRTSettings(14, 14);

            for (var i = 0; i < 1000; i++)
            {
                var maps = ThesisMaps.GetGeneratedMaps(r);
                foreach (var map in maps)
                {
                    tasks.Add(Run(Evaluate.ApproachType.BaseLine, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, baseLine: true), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.AStar, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.PotentialField, new PotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.PheromoneField, new PheromonePotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings, pheromoneSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.RRT, new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.RRTExtended, new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtExtendedSettings), i, map));
                }
            }
            
            sn.Run(tasks.Count, 1);
            await Task.WhenAll(tasks);
            sn.Result();

            async Task Run(Evaluate.ApproachType approach, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                try
                {
                    await Task.Run(method.Run);
                    sn.NotifyCompletion($"{i},{approach},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Exception: {i}|{approach}: {ex.Message} - {ex.StackTrace}");
                }
                
            }
        }

        [Fact]
        public async Task AllRun()
        {
            var tasks = new List<Task>();
            var sn = new StateNotifier(_output, $"AllSearch-{Guid.NewGuid()}.txt");

            var r = new Random();

            var potentialFieldSettings = new PotentialFieldSettings(3, 4, 9);
            var pheromoneSettings = new PheromoneSettings(1, 5, 8);
            var rrtSettings = new RRTSettings(12, 16);
            var rrtExtendedSettings = new RRTSettings(14, 14);

            for (var i = 0; i < 500; i++)
            {
                var maps = ThesisMaps.GetStaticMaps(r);
                foreach (var map in maps)
                {
                    tasks.Add(Run(Evaluate.ApproachType.BaseLine, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, baseLine: true), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.AStar, new AStar(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.PotentialField, new PotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.PheromoneField, new PheromonePotentialField(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, potentialFieldSettings, pheromoneSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.RRT, new RRT(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtSettings), i, map));
                    tasks.Add(Run(Evaluate.ApproachType.RRTExtended, new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtExtendedSettings), i, map));
                }
            }

            sn.Run(tasks.Count, 100);
            await Task.WhenAll(tasks);
            sn.Result();

            async Task Run(Evaluate.ApproachType approach, NavigationalMethod method, int i, (string mapName, int[,] map, Robot robot, Point goal) map)
            {
                try
                {
                    await Task.Run(method.Run);
                    sn.NotifyCompletion(
                        $"{i},{approach},{map.mapName},{method.HasSeenGoal},{method.Time:F},{method.Robot.Steps.Count},{method.AverageVisibility:F},{method.PathSmoothness:F}");
                }
                catch (Exception ex)
                {
                    _output.WriteLine($"Exception: {i}|{approach}: {ex.Message} - {ex.StackTrace}");
                }
                
            }
        }

        [Fact]
        public async Task ApproachMapping()
        {
            var data = await GetSearchData();
            
            _output.WriteLine("Evaluating maps on their best approaches");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.MapType mapType in Enum.GetValues(typeof(Evaluate.MapType)))
            {
                _output.WriteLine("==========================================================");
                _output.WriteLine($"Map: {mapType}");
                _output.WriteLine("----------------------------------------------------------");
                var all = data.Where(m => m.Map == mapType).ToList();
                var eval = new Evaluate(all);
                var scores = (from Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)) select (approach, value: eval.GetScore(approach))).ToList();
                foreach (var score in scores.OrderByDescending(s => s.value))
                {
                    _output.WriteLine($"{score.approach}: {score.value:F}" +
                                      $" | {(eval.CalculateSuccess(score.approach).Average(a => a.Value))}" +
                                      $" | {(eval.Path(score.approach).Any() ? eval.Path(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | {(eval.Duration(score.approach).Any() ? eval.Duration(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | {(eval.Visibility(score.approach).Any() ? eval.Visibility(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | {(eval.PathSmoothness(score.approach).Any() ? eval.PathSmoothness(score.approach).Average(a => a.Value) : 0)}");
                }

            }
        }

        [Fact]
        public async Task EvaluateSearch()
        {
            //var data = await GetSearchData();
            // var data = (await File.ReadAllLinesAsync(FileRef.AllCombinedRuns1)).Select(r => new Evaluate.EvaluateData(r)).ToList();
            var data = await GetCombinedData();

            // get stats on success
            _output.WriteLine("Average Success Rate");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                var all = data.Where(d => d.Approach == approach).ToList();
                _output.WriteLine($"{approach}: {all.Count(d => d.Success).Percentage(all.Count):F}%");
            }
            _output.WriteLine("==========================================================");

            // get stats on each metric
            _output.WriteLine("Path metric");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                var all = data.Where(d => d.Approach == approach).ToList();
                var success = all.Where(a => a.Success).ToList();
                _output.WriteLine($"{approach}:");
                _output.WriteLine($"Average Step: {all.Average(s => s.Steps):F} | Average success Step: {success.Average(s => s.Steps):F}");
                _output.WriteLine($"StdDev: {success.Select(s => s.Steps).StandardDeviation():F} | Min: {success.Select(s => s.Steps).Min():F} | 25: {success.Select(s => s.Steps).Percentile(0.25):F} | 75: {success.Select(s => s.Steps).Percentile(0.75):F} | Max: {success.Select(s => s.Steps).Max():F}");
                _output.WriteLine($"ALL StdDev: {all.Select(s => s.Steps).StandardDeviation():F} | Min: {all.Select(s => s.Steps).Min():F} | 25: {all.Select(s => s.Steps).Percentile(0.25):F} | 75: {all.Select(s => s.Steps).Percentile(0.75):F} | Max: {all.Select(s => s.Steps).Max():F}");
                _output.WriteLine("----------------------------------------------------------");
            }

            _output.WriteLine("Time metric");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                var all = data.Where(d => d.Approach == approach).ToList();
                var success = all.Where(a => a.Success).ToList();
                _output.WriteLine($"{approach}:");
                _output.WriteLine($"Average Visibility: {all.Average(s => s.Time):F} | Average success Visibility: {success.Average(s => s.Time):F}");
                _output.WriteLine($"StdDev: {success.Select(s => s.Time).StandardDeviation():F} | Min: {success.Select(s => s.Time).Min():F} | 25: {success.Select(s => s.Time).Percentile(0.25):F} | 75: {success.Select(s => s.Time).Percentile(0.75):F} | Max: {success.Select(s => s.Time).Max():F}");
                _output.WriteLine($"ALL StdDev: {all.Select(s => s.Time).StandardDeviation():F} | Min: {all.Select(s => s.Time).Min():F} | 25: {all.Select(s => s.Time).Percentile(0.25):F} | 75: {all.Select(s => s.Time).Percentile(0.75):F} | Max: {all.Select(s => s.Time).Max():F}");
                _output.WriteLine("----------------------------------------------------------");
            }

            _output.WriteLine("Visibility metric");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                var all = data.Where(d => d.Approach == approach).ToList();
                var success = all.Where(a => a.Success).ToList();
                _output.WriteLine($"{approach}:");
                _output.WriteLine($"Average Visibility: {all.Average(s => s.Visibility):F} | Average success Visibility: {success.Average(s => s.Visibility):F}");
                _output.WriteLine($"StdDev: {success.Select(s => s.Visibility).StandardDeviation():F} | Min: {success.Select(s => s.Visibility).Min():F} | 25: {success.Select(s => s.Visibility).Percentile(0.25):F} | 75: {success.Select(s => s.Visibility).Percentile(0.75):F} | Max: {success.Select(s => s.Visibility).Max():F}");
                _output.WriteLine($"ALL StdDev: {all.Select(s => s.Visibility).StandardDeviation():F} | Min: {all.Select(s => s.Visibility).Min():F} | 25: {all.Select(s => s.Visibility).Percentile(0.25):F} | 75: {all.Select(s => s.Visibility).Percentile(0.75):F} | Max: {all.Select(s => s.Visibility).Max():F}");
                _output.WriteLine("----------------------------------------------------------");
            }

            _output.WriteLine("Path Smoothness metric");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                var all = data.Where(d => d.Approach == approach).ToList();
                var success = all.Where(a => a.Success).ToList();
                _output.WriteLine($"{approach}:");
                _output.WriteLine($"Average Path Smoothness: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Average(s => s.PathSmoothness):F} | Average success Path Smoothness: {success.Average(s => s.PathSmoothness):F}");
                _output.WriteLine($"StdDev: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).StandardDeviation():F} | Min: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Min():F} | 25: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.25):F} | 75: {success.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.75):F} | Max: {success.Select(s => s.PathSmoothness).Max():F}");
                _output.WriteLine($"ALL StdDev: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).StandardDeviation():F} | Min: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Min():F} | 25: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.25):F} | 75: {all.Where(a => !double.IsNaN(a.PathSmoothness)).Select(s => s.PathSmoothness).Percentile(0.75):F} | Max: {all.Select(s => s.PathSmoothness).Max():F}");
                _output.WriteLine("----------------------------------------------------------");
            }

            _output.WriteLine("==========================================================");
            
            _output.WriteLine("Success by Map Type");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.MapType mapType in Enum.GetValues(typeof(Evaluate.MapType)))
            {
                var all = data.Where(d => d.Map == mapType).ToList();
                _output.WriteLine($"{mapType}: {all.Count(d => d.Success).Percentage(all.Count):F}%");

                var approachesLine = string.Empty;
                foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
                {
                    var approaches = all.Where(a => a.Approach == approach).ToList();
                    approachesLine += $"{approach}: {approaches.Count(d => d.Success).Percentage(approaches.Count):F}% |";
                }
                _output.WriteLine(approachesLine);
            }

            _output.WriteLine("==========================================================");
            _output.WriteLine("Evaluate Scores:");
            _output.WriteLine("----------------------------------------------------------");
            
            var eval = new Evaluate(data);
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                
                var calculateSuccess = eval.CalculateSuccess(approach);
                var path = eval.Path(approach);
                var duration = eval.Duration(approach);
                var visibility = eval.Visibility(approach);
                var pathSmoothness = eval.PathSmoothness(approach);

                var score = (calculateSuccess.Average(a => a.Value)
                            + path.Average(a => a.Value)
                            + duration.Average(a => a.Value)
                            + visibility.Average(a => a.Value)
                            + pathSmoothness.Average(a => a.Value)) / 5.0;
                _output.WriteLine($"{approach}: Overall score: {score:F}");
                _output.WriteLine("Success: " + string.Join(" | ", calculateSuccess.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("Path: " + string.Join(" | ", path.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("Duration: " + string.Join(" | ", duration.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("Visibility: " + string.Join(" | ", visibility.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("PathSmoothness: " + string.Join(" | ", pathSmoothness.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("----------------------------------------------------------");
            }
        }

        [Fact]
        public void SpeedTest()
        {
            var maps = ThesisMaps.GetGeneratedMaps();
            var map = maps.FirstOrDefault(m => m.mapName == "TunnelOne");

            var rrtExtendedSettings = new RRTSettings(14, 14);
            var results = new RRTExtended(map.map, new Robot(map.robot.Location, map.robot.FovLength), map.goal, 1000, rrtExtendedSettings).Run();
            _output.WriteLine(results);
        }

        [Fact]
        public async Task EvaluateScores()
        {
            //var data = await GetCombinedData();
            var data = File.ReadAllLines(ChapterThree.Data.FileRef.FinalAll).Select(l => new Evaluate.EvaluateData(l)).ToList();
            
            _output.WriteLine($"BS: {data.Where(d => d.Approach == Evaluate.ApproachType.BaseLine).Max(d => d.Time)}");
            var eval = new Evaluate(data);
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {

                var calculateSuccess = eval.CalculateSuccess(approach);
                var path = eval.Path(approach);
                var duration = eval.Duration(approach);
                var visibility = eval.Visibility(approach);
                var pathSmoothness = eval.PathSmoothness(approach);

                var score = eval.GetScore(approach);
                _output.WriteLine($"{approach}: Overall score: {score:F}");
                //_output.WriteLine("Success: " + string.Join(" | ", calculateSuccess.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Path: " + string.Join(" | ", path.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Duration: " + string.Join(" | ", duration.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Visibility: " + string.Join(" | ", visibility.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("PathSmoothness: " + string.Join(" | ", pathSmoothness.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("----------------------------------------------------------");
            }

            _output.WriteLine("==========================================================");
            eval = new Evaluate(data.Where(d => !d.Map.ToString().Contains("Tunnel") && !d.Map.ToString().Contains("Obstacles")).ToList());
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {

                var calculateSuccess = eval.CalculateSuccess(approach);
                var path = eval.Path(approach);
                var duration = eval.Duration(approach);
                var visibility = eval.Visibility(approach);
                var pathSmoothness = eval.PathSmoothness(approach);

                var score = eval.GetScore(approach);
                _output.WriteLine($"{approach}: Overall score: {score:F}");
                //_output.WriteLine("Success: " + string.Join(" | ", calculateSuccess.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Path: " + string.Join(" | ", path.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Duration: " + string.Join(" | ", duration.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Visibility: " + string.Join(" | ", visibility.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("PathSmoothness: " + string.Join(" | ", pathSmoothness.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("----------------------------------------------------------");
            }

            _output.WriteLine("==========================================================");
            eval = new Evaluate(data.Where(d => d.Map.ToString().Contains("Tunnel") || d.Map.ToString().Contains("Obstacles")).ToList());
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {

                var calculateSuccess = eval.CalculateSuccess(approach);
                var path = eval.Path(approach);
                var duration = eval.Duration(approach);
                var visibility = eval.Visibility(approach);
                var pathSmoothness = eval.PathSmoothness(approach);

                var score = eval.GetScore(approach);
                _output.WriteLine($"{approach}: Overall score: {score:F}");
                //_output.WriteLine("Success: " + string.Join(" | ", calculateSuccess.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Path: " + string.Join(" | ", path.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Duration: " + string.Join(" | ", duration.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("Visibility: " + string.Join(" | ", visibility.Select(d => $"{d.Key}: {d.Value:F}%")));
                //_output.WriteLine("PathSmoothness: " + string.Join(" | ", pathSmoothness.Select(d => $"{d.Key}: {d.Value:F}%")));
                _output.WriteLine("----------------------------------------------------------");
            }
        }

        

        private async Task<List<Evaluate.EvaluateData>> GetSearchData()
        {
            var search1 = (await File.ReadAllLinesAsync(FileRef.AllSearch1)).Select(r => new Evaluate.EvaluateData(r)).ToList();
            var search2 = (await File.ReadAllLinesAsync(FileRef.AllSearch2)).Select(r => new Evaluate.EvaluateData(r)).ToList();
            var maxSearch1 = search1.Max(s => s.Id);
            foreach (var evaluateData in search2)
            {
                evaluateData.Id += maxSearch1 + 1;
            }

            search1.AddRange(search2);
            return search1;
        }
        private async Task<List<Evaluate.EvaluateData>> GetCombinedData()
        {
            var search1 = (await File.ReadAllLinesAsync(FileRef.AllCombinedRuns1)).Select(r => new Evaluate.EvaluateData(r)).ToList();
            var search2 = (await File.ReadAllLinesAsync(FileRef.AllCombinedRuns2)).Select(r => new Evaluate.EvaluateData(r)).ToList();
            var maxSearch1 = search1.Max(s => s.Id);
            foreach (var evaluateData in search2)
            {
                evaluateData.Id += maxSearch1 + 1;
            }

            search1.AddRange(search2);
            return search1;
        }


    }
}
