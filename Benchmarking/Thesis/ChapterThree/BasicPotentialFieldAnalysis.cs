﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Thesis.ChapterThree.Data;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class BasicPotentialFieldAnalysis
    {
        private readonly ITestOutputHelper _output;
        public BasicPotentialFieldAnalysis(ITestOutputHelper output)
        {
            _output = output;
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
                ObstacleRange = int.Parse(items[6]);
                ObstacleConstant = int.Parse(items[7]);
                AttractiveConstant = int.Parse(items[8]);
            }
            public string MapName { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }
            public long Time { get; set; }
            public int Steps { get; set; }
            public double AverageVisibility { get; set; }
            public int ObstacleRange { get; set; }
            public int ObstacleConstant { get; set; }
            public int AttractiveConstant { get; set; }
        }

        [Fact]
        public void ExtensiveSearchAnalysis()
        {
            _output.WriteLine("Extensive Search Analysis");
            var all = File.ReadAllLines(FileRef.ExtensiveSearch).Select(l => new Data(l)).ToList();

            foreach (var g in all.GroupBy(g => g.MapName).OrderBy(g => g.Key))
            {
                _output.WriteLine($"Success {g.Key} rate: {g.Count(r => r.Success)} / {g.Count()} ({g.Count(r => r.Success).Percentage(g.Count())}%)");
            }
            _output.WriteLine($"==========================================================");
            _output.WriteLine($"Max: {all.Where(a => a.Success).Max(s => s.Steps)}, Min:{all.Where(a => a.Success).Min(s => s.Steps)}");
            //var enumerable = all
            //    .GroupBy(g => new { g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant })
            //    .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) })
            //    .OrderByDescending(g => g.Success)
            //    ;
            //foreach (var g in enumerable)
            //{
            //    _output.WriteLine($"{g.Key.ObstacleRange},{g.Key.ObstacleConstant},{g.Key.AttractiveConstant},{g.Success}");
            //}

            //_output.WriteLine($"==========================================================");

            //var e = all.GroupBy(g => g.MapName);
            //foreach (var gg in e)
            //{
            //    if (gg.Key.Contains("Bug") || gg.Key.Contains("WallOne") || gg.Key.Contains("SlitOne") || gg.Key.Contains("CorridorThree"))
            //        continue;
            //    var r = gg.ToArray()
            //        .GroupBy(g => new { g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant })
            //        .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) })
            //        .OrderByDescending(g => g.Success)
            //        .Take(15)
            //        ;
            //    _output.WriteLine($"Map: {gg.Key}");
            //    foreach (var rr in r)
            //    {
            //        _output.WriteLine($"{rr.Key.ObstacleRange},{rr.Key.ObstacleConstant},{rr.Key.AttractiveConstant},{rr.Success}");
            //    }
            //}



            //var plt = new ScottPlot.Plot();

            //var barInfo = all.GroupBy(g => g.MapName).Select(g => new { MapName = g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).OrderByDescending(g => g.Success).ToArray();

            //var bars = barInfo.Select(g => g.Success).ToArray();
            //var positions = Enumerable.Range(0, 18).Select(x => (double)x).ToArray();
            //var labels = barInfo.Select(g => g.MapName).ToArray();

            ////var bars = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            ////var barsOrdered = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            ////var positions = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17};
            ////var labels = all.Select(g => g.MapName).Distinct().ToArray();

            //plt.AddBar(bars, positions);
            //plt.XTicks(positions, labels);
            //plt.SetAxisLimits(yMin: 0);

            //new ScottPlot.FormsPlotViewer(plt).ShowDialog();
        }

        //double[] dataX = new double[] { 1, 2, 3, 4, 5 };
        //double[] dataY = new double[] { 1, 4, 9, 16, 25 };
        //var plt = new ScottPlot.Plot(400, 300);
        //plt.AddScatter(dataX, dataY);
        //new ScottPlot.FormsPlotViewer(plt).ShowDialog();




        [Fact]
        public void WallParameterExplorationAnalysis()
        {
            _output.WriteLine("WallOne Parameter Exploration Analysis");

            var wallOne = File.ReadAllLines(FileRef.WallOneParameterExploration).Select(l => { var items = l.Split(","); return new { HasSeenGoal = int.Parse(items[0]), IsStuck = int.Parse(items[1]), Time = int.Parse(items[2]), Steps = int.Parse(items[3]), ObstacleRange = int.Parse(items[4]), ObstacleConstant = int.Parse(items[5]), AttractiveConstant = int.Parse(items[6]),}; }).ToList();
            var wallTwo = File.ReadAllLines(FileRef.WallTwoParameterExploration).Select(l => { var items = l.Split(","); return new { HasSeenGoal = int.Parse(items[0]), IsStuck = int.Parse(items[1]), Time = int.Parse(items[2]), Steps = int.Parse(items[3]), ObstacleRange = int.Parse(items[4]), ObstacleConstant = int.Parse(items[5]), AttractiveConstant = int.Parse(items[6]),}; }).ToList();
            var wallThree = File.ReadAllLines(FileRef.WallThreeParameterExploration).Select(l => { var items = l.Split(","); return new { HasSeenGoal = int.Parse(items[0]), IsStuck = int.Parse(items[1]), Time = int.Parse(items[2]), Steps = int.Parse(items[3]), ObstacleRange = int.Parse(items[4]), ObstacleConstant = int.Parse(items[5]), AttractiveConstant = int.Parse(items[6]),}; }).ToList();

            _output.WriteLine($"Success WallOne rate: {wallOne.Count(r => r.HasSeenGoal == 1)} / {wallOne.Count} ({wallOne.Count(r => r.HasSeenGoal == 1).Percentage(wallOne.Count)}%)");
            _output.WriteLine($"Success WallTwo rate: {wallTwo.Count(r => r.HasSeenGoal == 1)} / {wallTwo.Count} ({wallTwo.Count(r => r.HasSeenGoal == 1).Percentage(wallTwo.Count)}%)");
            _output.WriteLine($"Success WallThree rate: {wallThree.Count(r => r.HasSeenGoal == 1)} / {wallThree.Count} ({wallThree.Count(r => r.HasSeenGoal == 1).Percentage(wallThree.Count)}%)");

            _output.WriteLine($"==========================================================");

            var scatters = new List<Scatter>();

            foreach (var result in wallOne.Where(w => w.HasSeenGoal == 1))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(10))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant}");
            }
            _output.WriteLine($"==========================================================");

            File.WriteAllLines(@"WallOneAnalysis.txt", scatters.Select(l => l.Line));

            scatters = new List<Scatter>();

            foreach (var result in wallTwo.Where(w => w.HasSeenGoal == 1))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(10))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant}");
            }

            File.WriteAllLines(@"WallTwoAnalysis.txt", scatters.Select(l => l.Line));

            _output.WriteLine($"==========================================================");

            scatters = new List<Scatter>();

            foreach (var result in wallThree.Where(w => w.HasSeenGoal == 1))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            foreach (var scatter in scatters.OrderByDescending(s => s.Counter).Take(10))
            {
                _output.WriteLine($"{scatter.Counter}:{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant}");
            }

            File.WriteAllLines(@"WallThreeAnalysis.txt", scatters.Select(l => l.Line));
        }

        public class Scatter
        {
            public Scatter(int obstacleRange, int obstacleConstant, int attractiveConstant)
            {
                ObstacleRange = obstacleRange;
                ObstacleConstant = obstacleConstant;
                AttractiveConstant = attractiveConstant;
            }
            public int ObstacleRange { get; set; }
            public int ObstacleConstant { get; set; }
            public int AttractiveConstant { get; set; }
            public int Counter { get; set; } = 1;

            public virtual string Line => $"{ObstacleRange},{ObstacleConstant},{AttractiveConstant},{Counter}";
        }

    }
}