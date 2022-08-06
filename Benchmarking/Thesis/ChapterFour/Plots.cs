using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using Benchmarking.Thesis.ChapterFour.Data;
using Benchmarking.Thesis.ChapterThree;
using ScottPlot;
using ScottPlot.Drawing;
using ScottPlot.Plottable;
using Xunit;
using Xunit.Abstractions;
using MapType = Benchmarking.Thesis.ChapterThree.Evaluate.MapType;
using ApproachType = Benchmarking.Thesis.ChapterThree.Evaluate.ApproachType;

namespace Benchmarking.Thesis.ChapterFour
{
    public class Plots
    {
        private readonly ITestOutputHelper _output;
        private readonly List<Evaluate.EvaluateData> _data;

        public Plots(ITestOutputHelper output)
        {
            _output = output;
            _data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();
        }

        public class Cp
        {
            public Cp(int id, MapType mapType, ApproachType approach, Evaluate eval)
            {
                Id = id;
                MapType = mapType;
                ApproachType = approach;
                Score = eval.Score(approach, 0.75, 0.75, 0.25);
                Success = eval.CalculateSuccess(approach).Any() ? eval.CalculateSuccess(approach).Average(a => a.Value) : 0;
                Path = eval.Path(approach).Any() ? eval.Path(approach).Average(a => a.Value) : 0;
                PathSmoothness = eval.PathSmoothness(approach).Any() ? eval.PathSmoothness(approach).Average(a => a.Value) : 0;
                Duration = eval.Duration(approach).Any() ? eval.Duration(approach).Average(a => a.Value) : 0;
                Visibility = eval.Visibility(approach).Any() ? eval.Visibility(approach).Average(a => a.Value) : 0;
            }

            public int Id { get; }
            public MapType MapType { get; }
            public ApproachType ApproachType { get; set; }
            public double Score { get; set; }
            public double Success { get; set; }
            public double Path { get; set; }
            public double PathSmoothness { get; set; }
            public double Duration { get; set; }
            public double Visibility { get; set; }

            public string BetterAt(Cp aStar)
            {
                if (Score > aStar.Score) return $"{Id}-{MapType}-{ApproachType}: Score: {Score} > A* {aStar.Score}";
                if (Success > aStar.Success) return $"{Id}-{MapType}-{ApproachType}: Success: {Success} > A* {aStar.Success}";
                if (Path > aStar.Path) return $"{Id}-{MapType}-{ApproachType}: Path: {Path} > A* {aStar.Path}";
                if (PathSmoothness > aStar.PathSmoothness) return $"{Id}-{MapType}-{ApproachType}: PathSmoothness: {PathSmoothness} > A* {aStar.PathSmoothness}";
                if (Duration > aStar.Duration) return $"{Id}-{MapType}-{ApproachType}: Duration: {Duration} > A* {aStar.Duration}";
                if (Visibility > aStar.Visibility) return $"{Id}-{MapType}-{ApproachType}: Score: {Visibility} > A* {aStar.Visibility}";
                return string.Empty;
            }
        }

        [Fact]
        public void ChapterFourPathLengthPercentile()
        {
            var plt = new Plot();

            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 2.00, 1001.00, 1001.00, 1001.00, 1001.00 }, Color.DarkGreen, lineStyle: LineStyle.Dash, label: "Potential Field All Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 2.00, 23.25, 31.00, 38.00, 196.00 }, Color.DarkGreen, label: "Potential Field Successful Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 2.00, 58.75, 190.50, 865.50, 1001.00 }, Color.DodgerBlue, lineStyle: LineStyle.Dash, label: "Pheromone Potential Field All Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 2.00, 44.00, 102.00, 238.00, 986.00 }, Color.DodgerBlue, label: "Pheromone Potential Field Successful Runs");

            // customize the axis labels
            plt.Title("Path Length Evaluation");
            plt.XLabel("Length Percentile");
            plt.YLabel("Path Length");

            plt.Legend(location: Alignment.UpperLeft);

            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void ChapterFourVisibilityPercentile()
        {
            var plt = new Plot();

            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 2.60, 10.36, 12.80, 17.16, 25.41 }, Color.DarkGreen, lineStyle: LineStyle.Dash, label: "Potential Field All Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 4.04, 16.64, 19.06, 20.94, 24.44 }, Color.DarkGreen, label: "Potential Field Successful Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 2.64, 9.75, 12.70, 17.88, 25.47 }, Color.DodgerBlue, lineStyle: LineStyle.Dash, label: "Pheromone Potential Field All Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 4.04, 11.66, 14.14, 18.89, 24.77 }, Color.DodgerBlue, label: "Pheromone Potential Field Successful Runs");

            // customize the axis labels
            plt.Title("Visibility Evaluation");
            plt.XLabel("Visibility Percentile");
            plt.YLabel("Visibility");

            plt.Legend(location: Alignment.UpperLeft);

            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void ChapterFourPathSmoothnessPercentile()
        {
            var plt = new Plot();

            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 0.00, 9.61, 9.79, 9.84, 27.78 }, Color.DarkGreen, lineStyle: LineStyle.Dash, label: "Potential Field All Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 0.00, 1.03, 1.63, 6.75, 27.78 }, Color.DarkGreen, label: "Potential Field Successful Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 0.00, 1.14, 2.56, 5.04, 27.78 }, Color.DodgerBlue, lineStyle: LineStyle.Dash, label: "Pheromone Potential Field All Runs");
            plt.AddScatter(new[] {0, 25, 50, 75, 100.0}, new[] { 0.00, 1.54, 3.41, 5.89, 27.78 }, Color.DodgerBlue, label: "Pheromone Potential Field Successful Runs");

            // customize the axis labels
            plt.Title("Path Smoothness Evaluation");
            plt.XLabel("Path Smoothness Percentile");
            plt.YLabel("Path Smoothness");

            plt.Legend(location: Alignment.UpperLeft);

            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void SuccessRateInStaticPf()
        {
            var staticMaps = Evaluate.GetStaticMaps();
            var approaches = new[]
            {
                ApproachType.BaseLine, ApproachType.PotentialField, ApproachType.PheromoneField
            };

            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = staticMaps
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetPfStyles(plt, approachType, barValues);
            }

            plt.XTicks(GetPositions(11, staticMaps.Length / 3, 5).ToArray(), new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });
            plt.SetAxisLimits(yMin: 0);


            foreach (var i in GetPositions(3, staticMaps.Length, 3).ToArray().SkipLast(1))
            {
                plt.AddVerticalLine(i, Color.Black, 1, LineStyle.Dot);
            }

            foreach (var i in GetPositions(11, staticMaps.Length / 3, 11).ToArray().SkipLast(1))
            {
                plt.AddVerticalLine(i, Color.Black, 2);
            }

            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.Legend(location: Alignment.UpperRight);
            plt.XLabel("Navigational Map Types");
            plt.YLabel("Score");
            plt.Title("The Potential Field Approaches in Static Environments");
            new FormsPlotViewer(plt).ShowDialog();

        }

        [Fact]
        public void BetterThanAStar()
        {
            var approaches = new[]
            {
                ApproachType.RRT, ApproachType.RRTExtended, ApproachType.PotentialField, ApproachType.PheromoneField
            };


            var better = new List<(Cp method, Cp aStar)>();

            var groups = _data.GroupBy(d => new { d.Id, d.Map });
            foreach (var group in groups)
            {
                var eval = new Evaluate(group.ToList());

                var aStar = new Cp(group.Key.Id, group.Key.Map, ApproachType.AStar, eval);

                var methods = approaches
                    .Select(a => new Cp(group.Key.Id, group.Key.Map, a, eval))
                    .ToList();

                foreach (var cp in methods)
                {
                    var result = cp.BetterAt(aStar);
                    if (!string.IsNullOrEmpty(result))
                    {
                        better.Add((cp, aStar));
                        _output.WriteLine(result);
                    }
                }
            }
            _output.WriteLine("==================================================================");
            _output.WriteLine("==================================================================");
            foreach (var approach in approaches)
            {
                _output.WriteLine($"{approach}: {better.Count(b => b.method.ApproachType == approach)}");
                foreach (MapType mapType in Enum.GetValues(typeof(MapType)))
                {
                    var count = better.Count(b => b.method.ApproachType == approach && b.method.MapType == mapType);
                    if (count > 0)
                    {
                        _output.WriteLine($"{approach}-{mapType}: {count} / {_data.Count(d => d.Approach == ApproachType.AStar && d.Map == mapType)}");
                        foreach (var cp in better.Where(b => b.method.ApproachType == approach && b.method.MapType == mapType))
                        {
                            _output.WriteLine(cp.method.BetterAt(cp.aStar));
                        }


                        _output.WriteLine("--------------------------------------------------------------");
                    }

                }

                _output.WriteLine("==================================================================");
            }
        }

        [Fact]
        public void ScoreInStaticMaps()
        {
            var staticMaps = Evaluate.GetStaticMaps();

            var approaches = new[]
            {
                ApproachType.BaseLine, ApproachType.AStar, ApproachType.RRT, ApproachType.RRTExtended, ApproachType.PotentialField, ApproachType.PheromoneField
            };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = staticMaps
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetDynamicStyles(plt, approachType, barValues);
            }

            plt.XTicks(GetPositions(21, staticMaps.Length / 3, 9).ToArray(), new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });
            plt.SetAxisLimits(yMin: 0);

            foreach (var i in GetPositions(20, staticMaps.Length / 3, 20).ToArray())
            {
                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            }
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.Legend(location: Alignment.UpperRight);
            plt.XLabel("Navigational Map Types");
            plt.YLabel("Score");
            plt.Title("All approaches in static environments");
            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void ScoreInGeneratedMaps()
        {
            var genMaps = Evaluate.GetGenMaps();

            var approaches = new[]
            {
                ApproachType.BaseLine, ApproachType.AStar, ApproachType.RRT, ApproachType.RRTExtended, ApproachType.PotentialField, ApproachType.PheromoneField
            };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = genMaps
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetDynamicStyles(plt, approachType, barValues);
            }

            plt.XTicks(new[] { 17.0, 51.0 }, new[] { "Obstacles", "Tunnels" });
            plt.SetAxisLimits(yMin: 0);

            plt.AddVerticalLine(34, Color.Black, 2, LineStyle.Dot);
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.Legend(location: Alignment.UpperRight);
            plt.XLabel("Navigational Map Types");
            plt.YLabel("Score");
            plt.Title("All approaches in generated environments");
            new FormsPlotViewer(plt).ShowDialog();
        }
        [Fact]
        public void SuccessRateInGeneratedTunnels()
        {
            var mapTypes = new[]
            {
                MapType.TunnelOne, MapType.TunnelTwo, MapType.TunnelThree, MapType.TunnelFour, MapType.TunnelFive
            };

            var approaches = new[]
            {
                ApproachType.BaseLine, ApproachType.AStar, ApproachType.RRT, ApproachType.RRTExtended, ApproachType.PotentialField, ApproachType.PheromoneField
            };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = mapTypes
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetStyles(plt, approachType, barValues);
            }
            //var bars = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var barsOrdered = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var positions = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17};
            //var labels = all.Select(g => g.MapName).Distinct().ToArray();


            plt.XTicks(new double[] { 3, 10, 17, 24, 31 }, mapTypes.Select(m => m.ToString()).ToArray());
            plt.SetAxisLimits(yMin: 0);

            foreach (var i in new double[] { 7, 14, 21, 28 })
            {
                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            }


            plt.Legend(location: Alignment.UpperRight);
            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void SuccessRateInGeneratedObstacles()
        {
            var mapTypes = new[]
            {
                MapType.ObstacleOne, MapType.ObstacleTwo, MapType.ObstacleThree, MapType.ObstacleFour,
                MapType.ObstacleFive
            };

            var approaches = new[]
            {
                ApproachType.BaseLine, ApproachType.AStar, ApproachType.RRT, ApproachType.RRTExtended, ApproachType.PotentialField,
                ApproachType.PheromoneField
            };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = mapTypes
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetStyles(plt, approachType, barValues);
            }
            //var bars = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var barsOrdered = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var positions = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17};
            //var labels = all.Select(g => g.MapName).Distinct().ToArray();


            plt.XTicks(new double[] { 3, 10, 17, 24, 31 }, new[] { "ObstacleOne", "ObstacleTwo", "ObstacleThree", "ObstacleFour", "ObstacleFive" });
            plt.SetAxisLimits(yMin: 0);

            foreach (var i in new double[] { 7, 14, 21, 28 })
            {
                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            }


            plt.Legend(location: Alignment.UpperRight);
            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void RRTVsRRTExtendedStatic()
        {
            var staticMaps = Evaluate.GetStaticMaps();
            var approaches = new[] { ApproachType.RRT, ApproachType.RRTExtended };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = staticMaps
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetRRTStyles(plt, approachType, barValues);
            }

            //plt.XTicks(GetPositions(2, staticMaps.Length, 1).ToArray(), staticMaps.Select(s => s.ToString()).ToArray());
            plt.XTicks(GetPositions(8, staticMaps.Length / 3, 4).ToArray(), new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });
            plt.SetAxisLimits(yMin: 0);

            foreach (var i in GetPositions(8, staticMaps.Length, 8).ToArray())
            {
                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            }
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.Legend(location: Alignment.UpperRight);
            plt.XLabel("Navigational Map Types");
            plt.YLabel("Score");
            plt.Title("RRT vs RRT* in static environments");
            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void RRTVsRRTExtendedGen()
        {
            var genMaps = Evaluate.GetGenMaps();
            var approaches = new[] { ApproachType.RRT, ApproachType.RRTExtended };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = genMaps
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.Score(approachType, 0.75, 0.75, 0.25))
                    .ToArray();
                SetRRTStyles(plt, approachType, barValues);
            }

            //plt.XTicks(GetPositions(2, genMaps.Length, 1).ToArray(), genMaps.Select(s => s.ToString()).ToArray());
            plt.XTicks(GetPositions(12, genMaps.Length / 5, 8).ToArray(), new[] { "Obstacles", "Tunnels" });
            plt.SetAxisLimits(yMin: 0);

            //foreach (var i in GetPositions(2, genMaps.Length, 2).ToArray())
            //{
            plt.AddVerticalLine(14, Color.Black, 2, LineStyle.Dot);
            //}
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.Legend(location: Alignment.UpperRight);
            plt.XLabel("Navigational Map Types");
            plt.YLabel("Score");
            plt.Title("RRT vs RRT* in generated environments");
            new FormsPlotViewer(plt).ShowDialog();
        }

        [Fact]
        public void RRTVsRRTExtendedGenSuccess()
        {
            var genMaps = Evaluate.GetGenMaps();
            var approaches = new[] { ApproachType.RRT, ApproachType.RRTExtended };
            var plt = new Plot();

            foreach (var approachType in approaches)
            {
                var barValues = genMaps
                    .Select(mapType => new Evaluate(_data.Where(m => m.Map == mapType).ToList()))
                    .Select(eval => eval.CalculateSuccess(approachType).Select(s => s.Value).Average())
                    .ToArray();
                SetRRTStyles(plt, approachType, barValues);
            }

            //plt.XTicks(GetPositions(2, genMaps.Length, 1).ToArray(), genMaps.Select(s => s.ToString()).ToArray());
            plt.XTicks(GetPositions(12, genMaps.Length / 5, 8).ToArray(), new[] { "Obstacles", "Tunnels" });
            plt.SetAxisLimits(yMin: 0);

            //foreach (var i in GetPositions(2, genMaps.Length, 2).ToArray())
            //{
            plt.AddVerticalLine(14, Color.Black, 2, LineStyle.Dot);
            //}
            plt.SetAxisLimits(yMin: 0, yMax: 1);

            plt.Legend(location: Alignment.UpperRight);
            plt.XLabel("Navigational Map Types");
            plt.YLabel("Success Rate");
            plt.Title("RRT vs RRT* in generated environments");
            new FormsPlotViewer(plt).ShowDialog();
        }

        private static void SetRRTStyles(Plot plt, ApproachType approachType, double[] values)
        {
            BarPlot bar;
            switch (approachType)
            {
                case ApproachType.RRT:
                    bar = plt.AddBar(values.ToArray(), GetPositions(2, values.Length, 0).ToArray());
                    bar.FillColor = Color.DarkOrange;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.RRTExtended:
                    bar = plt.AddBar(values.ToArray(), GetPositions(2, values.Length, 1).ToArray());
                    bar.FillColor = Color.DarkRed;
                    bar.Label = $"{approachType}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(approachType), approachType, null);
            }
        }

        private static IEnumerable<double> GetPositions(int approachCount, int mapCount, int startPosition)
        {
            var step = approachCount + 1;
            for (var i = 0; i < mapCount; i++)
            {
                yield return startPosition + (step * i);
            }
        }

        private static void SetPfStyles(Plot plt, ApproachType approachType, double[] values)
        {
            BarPlot bar;
            switch (approachType)
            {
                case ApproachType.BaseLine:
                    bar = plt.AddBar(values.ToArray(), GetPositions(3, values.Length, 0).ToArray());
                    bar.HatchStyle = HatchStyle.StripedWideUpwardDiagonal;
                    bar.FillColor = Color.DarkGreen;
                    bar.FillColorHatch = Color.ForestGreen;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.PotentialField:
                    bar = plt.AddBar(values.ToArray(), GetPositions(3, values.Length, 1).ToArray());
                    bar.HatchStyle = HatchStyle.StripedUpwardDiagonal;
                    bar.FillColor = Color.Gray;
                    bar.FillColorHatch = Color.Black;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.PheromoneField:
                    bar = plt.AddBar(values.ToArray(), GetPositions(3, values.Length, 2).ToArray());
                    bar.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
                    bar.FillColor = Color.DodgerBlue;
                    bar.FillColorHatch = Color.DeepSkyBlue;
                    bar.Label = $"{approachType}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(approachType), approachType, null);
            }
        }

        /*
         * bar1.FillColor = Color.Gray;
            bar1.FillColorHatch = Color.Black;
            bar1.Label = "Potential Field";

            var bar2 = plt.AddBar(extendedBars.Select(s => s.Success).ToArray(), new double[] { 2, 4, 6, 9, 11, 13, 16, 18, 20, 23, 25, 27, 30, 32, 34, 37, 39, 41 });
            bar2.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
            bar2.FillColor = Color.DodgerBlue;
            bar2.FillColorHatch = Color.DeepSkyBlue;
            bar2.Label = "Pheromone Potential Field";
         */

        private static void SetDynamicStyles(Plot plt, ApproachType approachType, double[] values)
        {
            BarPlot bar;
            switch (approachType)
            {
                case ApproachType.BaseLine:
                    bar = plt.AddBar(values.ToArray(), GetPositions(6, values.Length, 0).ToArray());
                    bar.FillColor = Color.DarkGreen;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.AStar:
                    bar = plt.AddBar(values.ToArray(), GetPositions(6, values.Length, 1).ToArray());
                    bar.FillColor = Color.Aquamarine;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.RRT:
                    bar = plt.AddBar(values.ToArray(), GetPositions(6, values.Length, 2).ToArray()); ;
                    bar.FillColor = Color.DarkOrange;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.RRTExtended:
                    bar = plt.AddBar(values.ToArray(), GetPositions(6, values.Length, 3).ToArray()); ;
                    bar.FillColor = Color.DarkRed;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.PotentialField:
                    bar = plt.AddBar(values.ToArray(), GetPositions(6, values.Length, 4).ToArray()); ;
                    bar.FillColor = Color.LightSkyBlue;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.PheromoneField:
                    bar = plt.AddBar(values.ToArray(), GetPositions(6, values.Length, 5).ToArray()); ;
                    bar.FillColor = Color.Navy;
                    bar.Label = $"{approachType}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(approachType), approachType, null);
            }
        }
        private static void SetStyles(Plot plt, ApproachType approachType, double[] values)
        {
            BarPlot bar;
            switch (approachType)
            {
                case ApproachType.BaseLine:
                    bar = plt.AddBar(values.ToArray(), new double[] { 1, 8, 15, 22, 29 });
                    bar.FillColor = Color.DarkGreen;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.AStar:
                    bar = plt.AddBar(values.ToArray(), new double[] { 2, 9, 16, 23, 30 });
                    bar.FillColor = Color.Aquamarine;
                    bar.FillColorHatch = Color.Black;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.RRT:
                    bar = plt.AddBar(values.ToArray(), new double[] { 3, 10, 17, 24, 31 });
                    bar.FillColor = Color.DarkOrange;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.RRTExtended:
                    bar = plt.AddBar(values.ToArray(), new double[] { 4, 11, 18, 25, 32 });
                    bar.FillColor = Color.DarkRed;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.PotentialField:
                    bar = plt.AddBar(values.ToArray(), new double[] { 5, 12, 19, 26, 33 });
                    bar.FillColor = Color.LightSkyBlue;
                    bar.Label = $"{approachType}";
                    break;
                case ApproachType.PheromoneField:
                    bar = plt.AddBar(values.ToArray(), new double[] { 6, 13, 20, 27, 34 });
                    bar.FillColor = Color.Navy;
                    bar.Label = $"{approachType}";
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(approachType), approachType, null);
            }
        }
    }
}
