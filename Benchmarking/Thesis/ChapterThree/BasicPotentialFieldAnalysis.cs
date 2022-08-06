using System;
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
                DistanceToGoal = double.Parse(items[6]);
                PathSmoothness = double.Parse(items[7]);
                ObstacleRange = int.Parse(items[8]);
                ObstacleConstant = int.Parse(items[9]);
                AttractiveConstant = int.Parse(items[10]);
                //ObstacleRange = int.Parse(items[6]);
                //ObstacleConstant = int.Parse(items[7]);
                //AttractiveConstant = int.Parse(items[8]);
            }
            public string MapName { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }
            public long Time { get; set; }
            public int Steps { get; set; }
            public double AverageVisibility { get; set; }
            public double DistanceToGoal { get; set; }
            public double PathSmoothness { get; set; }
            public int ObstacleRange { get; set; }
            public int ObstacleConstant { get; set; }
            public int AttractiveConstant { get; set; }
        }

        [Fact]
        public void ExtensiveSearch2Analysis()
        {
            _output.WriteLine("Extensive Search 2 Analysis");
            var all = File.ReadAllLines(FileRef.ExtensiveSearch2).Select(l => new Data(l)).ToList();

            foreach (var g in all.GroupBy(g => g.MapName).OrderBy(g => CompareFields.ToOrderValue((Evaluate.MapType)Enum.Parse(typeof(Evaluate.MapType), g.Key))))
            {
                _output.WriteLine($"Success {g.Key} rate: {g.Count(r => r.Success)} / {g.Count()} ({g.Count(r => r.Success).Percentage(g.Count())}%)");
            }
            _output.WriteLine($"==========================================================");
            _output.WriteLine($"Max: {all.Where(a => a.Success).Max(s => s.Steps)}, Min:{all.Where(a => a.Success).Min(s => s.Steps)}");
            var enumerable = all
                .GroupBy(g => new { g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant })
                .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), Distance = g.Average(a => a.Steps), Smoothness = g.Average(a => a.PathSmoothness), DG = g.Average(a => a.DistanceToGoal) })
                .OrderByDescending(g => g.Success)
                ;
            foreach (var g in enumerable)
            {
                _output.WriteLine($"{g.Key.ObstacleRange},{g.Key.ObstacleConstant},{g.Key.AttractiveConstant}: Success = {g.Success}, Steps = {Math.Round(g.Distance, 2)}, DistanceToGoal = {Math.Round(g.DG, 2)}, Smoothness = {Math.Round(g.Smoothness, 2)}");
            }
        }

        [Fact]
        public void FinalScatter()
        {
            var all = File.ReadAllLines(FileRef.FinalPotential).Select(l => new Data(l)).ToList();
            var scatters = new List<Scatter>();

            foreach (var result in all.Where(w => w.Success))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            var max = 0.0;
            foreach (var scatter in scatters)//.OrderByDescending(s => s.Counter).Take(10))
            {
                max = Math.Max(max, scatter.Counter.Percentage(360) * 100);
                _output.WriteLine($"{scatter.AttractiveConstant},{scatter.ObstacleConstant},{scatter.ObstacleRange},{(scatter.Counter.Percentage(360) * 100)}");
            }
            _output.WriteLine($"{max} - {(max * 0.75)}");
        }

        [Fact]
        public void FinalPfTable()
        {
            var all = File.ReadAllLines(FileRef.FinalPotential).Select(l => new Data(l)).ToList();
            var scatters = new List<Scatter>();

            foreach (var result in all.Where(w => w.Success))
            {
                var i = scatters.FirstOrDefault(i => i.ObstacleRange == result.ObstacleRange && i.ObstacleConstant == result.ObstacleConstant && i.AttractiveConstant == result.AttractiveConstant);
                if (i == default)
                    scatters.Add(new Scatter(result.ObstacleRange, result.ObstacleConstant, result.AttractiveConstant));
                else
                    i.Counter += 1;
            }

            var byMap = all.GroupBy(m => m.MapName).Select(b => new { b.Key, Succes = b.Count(c => c.Success).Percentage(b.Count())  }).ToList();
            var byParam = all.GroupBy(m => new { m.AttractiveConstant, m.ObstacleConstant, m.ObstacleRange }).Select(b => new { b.Key, Succes = b.Count(c => c.Success).Percentage(b.Count())  }).ToList();
            var byParamFilter = all.Where(a => !a.MapName.Contains("One")).GroupBy(m => new { m.AttractiveConstant, m.ObstacleConstant, m.ObstacleRange }).Select(b => new { b.Key, Succes = b.Count(c => c.Success).Percentage(b.Count())  }).ToList();
            _output.WriteLine($"Success Rate: {all.Count(a => a.Success).Percentage(all.Count)}");
            _output.WriteLine($"By map type: {byMap.Select(g => g.Succes).Percentile(0.25)*100} {byMap.Select(g => g.Succes).Percentile(0.5)*100}");
            _output.WriteLine($"By param type: {byParam.Select(g => g.Succes).Percentile(0.25)*100} {byParam.Select(g => g.Succes).Percentile(0.5)*100}");
            _output.WriteLine($"By param type -One: {byParamFilter.Select(g => g.Succes).Percentile(0.25)*100} {byParamFilter.Select(g => g.Succes).Percentile(0.5)*100}");
            var mapNames = all.Select(m => m.MapName).Distinct().ToList();
            foreach (var mapName in mapNames)
            {
                _output.WriteLine($"{mapName}");
            }

            //var max = 0.0;
            //foreach (var scatter in scatters)//.OrderByDescending(s => s.Counter).Take(10))
            //{
            //    max = Math.Max(max, scatter.Counter.Percentage(360) * 100);
            //    _output.WriteLine($"{scatter.AttractiveConstant},{scatter.ObstacleConstant},{scatter.ObstacleRange},{(scatter.Counter.Percentage(360) * 100)}");
            //}
            //_output.WriteLine($"{max} - {(max * 0.75)}");
        }

        [Fact]
        public void FinalAnalysis()
        {
            var all = File.ReadAllLines(FileRef.FinalPotential).Select(l => new Data(l)).ToList();
            
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
                var top5 = g.All.GroupBy(g => new {g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant})
                    .Select(g => new {g.Key, Success = g.Count(r => r.Success).Percentage(g.Count())})
                    .OrderByDescending(g => g.Success).ToList();

                //_output.WriteLine($"Top: {string.Join(" || ", top5.Select(t => $"{t.Key.ObstacleRange},{t.Key.ObstacleConstant},{t.Key.AttractiveConstant}: {t.Success:F}%"))}");

            }
            
            _output.WriteLine("==========================================================");
            _output.WriteLine("BY PARAMETERS");
            var paramGroups = all
                    //.Where(g => !g.MapName.Contains("One"))
                    .GroupBy(g => new { g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant })
                    .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), All = g})
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
            
            foreach (var g in paramGroups.Take(15))
            {
                _output.WriteLine($"{g.Key.ObstacleRange},{g.Key.ObstacleConstant},{g.Key.AttractiveConstant}: \t{g.Success:F}%");

                var maps = g.All
                    //.Where(g => !g.MapName.Contains("One"))
                    .GroupBy(g => g.MapName)
                    .Select(s => new { Map = s.Key, Success = s.Count(r => r.Success).Percentage(s.Count())})
                    .OrderByDescending(g => g.Success).Take(7).ToList();

                _output.WriteLine($"Top: {string.Join(" || ", maps.Select(t => $"{t.Map}: {t.Success:F}%"))}");
            }
        }

        [Fact]
        public void FinalPheromoneAnalysis()
        {
            var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l)).ToList();
            
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
                var top5 = g.All.GroupBy(g => new {g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant})
                    .Select(g => new {g.Key, Success = g.Count(r => r.Success).Percentage(g.Count())})
                    .OrderByDescending(g => g.Success).ToList();

                //_output.WriteLine($"Top: {string.Join(" || ", top5.Select(t => $"{t.Key.ObstacleRange},{t.Key.ObstacleConstant},{t.Key.AttractiveConstant}: {t.Success:F}%"))}");

            }
            
            _output.WriteLine("==========================================================");
            _output.WriteLine("BY PARAMETERS");
            var paramGroups = all
                    //.Where(g => !g.MapName.Contains("One"))
                    .GroupBy(g => new { g.Constant, g.StrengthIncrease, g.Range })
                    .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()), All = g})
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
            
            foreach (var g in paramGroups.Take(15))
            {
                _output.WriteLine($"{g.Key.Constant},{g.Key.StrengthIncrease},{g.Key.Range}: \t{g.Success:F}%");

                var maps = g.All
                    //.Where(g => !g.MapName.Contains("One"))
                    .GroupBy(g => g.MapName)
                    .Select(s => new { Map = s.Key, Success = s.Count(r => r.Success).Percentage(s.Count())})
                    .OrderByDescending(g => g.Success).Take(7).ToList();

                _output.WriteLine($"Top: {string.Join(" || ", maps.Select(t => $"{t.Map}: {t.Success:F}%"))}");
            }
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
            var enumerable = all
                .GroupBy(g => new { g.ObstacleRange, g.ObstacleConstant, g.AttractiveConstant })
                .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) })
                .OrderByDescending(g => g.Success)
                ;
            foreach (var g in enumerable)
            {
                _output.WriteLine($"{g.Key.ObstacleRange},{g.Key.ObstacleConstant},{g.Key.AttractiveConstant},{g.Success}");
            }

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
            public Scatter(double obstacleRange, double obstacleConstant, double attractiveConstant)
            {
                ObstacleRange = obstacleRange;
                ObstacleConstant = obstacleConstant;
                AttractiveConstant = attractiveConstant;
            }
            public double ObstacleRange { get; set; }
            public double ObstacleConstant { get; set; }
            public double AttractiveConstant { get; set; }
            public double Counter { get; set; } = 1;

            public virtual string Line => $"{ObstacleRange},{ObstacleConstant},{AttractiveConstant},{Counter}";
        }

    }
}
