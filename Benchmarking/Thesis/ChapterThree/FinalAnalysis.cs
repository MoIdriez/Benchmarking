//using System;
//using System.Collections.Generic;
//using System.Drawing;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Benchmarking.Core.Map;
//using Benchmarking.Thesis.ChapterThree.Data;
//using ScottPlot;
//using ScottPlot.Drawing;
//using Xunit;
//using Xunit.Abstractions;

//namespace Benchmarking.Thesis.ChapterThree
//{
//    public class FinalAnalysis
//    {
//        private readonly ITestOutputHelper _output;
//        private readonly Random _random = new();

//        public FinalAnalysis(ITestOutputHelper output)
//        {
//            _output = output;
//        }

//        [Fact]
//        public void QuickCheck()
//        {
//            var baseLine = File.ReadAllLines(FileRef.BaseLine).Select(l => new BaseLineCalculations.Data(l) as BaseLineCalculations.IData).ToList();
//            var basic = File.ReadAllLines(FileRef.FinalPotential).Select(l => new BasicPotentialFieldAnalysis.Data(l) as Evaluate.IData).ToList();
//            var extended = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l)).ToList();

//            var mapNames = baseLine.Select(m => m.MapName).Distinct().OrderBy(CompareFields.ToOrderValue).ToList();

//            _output.WriteLine($"Success: {baseLine.Count(s => s.Success).Percentage(baseLine.Count):F}|{extended.Count(s => s.Success).Percentage(extended.Count):F}|{basic.Count(s => s.Success).Percentage(basic.Count):F}");
//            foreach (var map in mapNames)
//            {
//                _output.WriteLine($"{map}: {baseLine.Count(b => b.MapName == map && b.Success).Percentage(baseLine.Count(b => b.MapName == map)):F}| {extended.Count(b => b.MapName == map && b.Success).Percentage(extended.Count(b => b.MapName == map)):F}| {basic.Count(b => b.MapName == map && b.Success).Percentage(basic.Count(b => b.MapName == map)):F}");
//            }
//            _output.WriteLine($"__________________________________________");

//            _output.WriteLine($"Path length:" +
//                              $" ({baseLine.Average(c => c.Steps):F}|{baseLine.Where(b => b.Success).Average(c => c.Steps):F})" +
//                              $" ({extended.Average(c => c.Steps):F}|{extended.Where(b => b.Success).Average(c => c.Steps):F})" +
//                              $" ({basic.Average(c => c.Steps):F}|{basic.Where(b => b.Success).Average(c => c.Steps):F})");
//        }

//        [Fact]
//        public void GenerateScatterPheromone()
//        {
//            var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l)).ToList();

//            foreach (var scatter in all.GroupBy(g => new { g.Range, g.Constant, g.StrengthIncrease }).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }))
//                _output.WriteLine($"{scatter.Key.Constant},{scatter.Key.StrengthIncrease},{scatter.Key.Range},{scatter.Success}");
//        }

//        [Fact]
//        public void CalculateEffectivenessPotential()
//        {
//            var all = File.ReadAllLines(FileRef.FinalPotential).Select(l => new BasicPotentialFieldAnalysis.Data(l) as Evaluate.IData).ToList();
//            var evaluate = new Evaluate(all);
//            evaluate.Compare(_output);
//        }

//        [Fact]
//        public void CalculateEffectivenessPheromone()
//        {
//            //var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l) as Evaluate.IData).ToList();
//            //var evaluate = new Evaluate(all);
//            //evaluate.Compare(_output);
//        }

//        [Fact]
//        public void ComparisonByMapType()
//        {
//            //var basic = File.ReadAllLines(FileRef.FinalPotential)
//            //    .Select(l => new BasicPotentialFieldAnalysis.Data(l)).ToList();
//            //var extended = File.ReadAllLines(FileRef.FinalPheromone)
//            //    .Select(l => new PheromonePotentialFieldAnalysis.PfData(l)).ToList();

//            //var basicEval = new Evaluate(basic);
//            //var extendedEval = new Evaluate(extended);

//            //var pathPlot = PlotTwoMetric(basicEval.Path().Select(p => p.value).ToArray(),
//            //    extendedEval.Path().Select(p => p.value).ToArray(), "Path Length").Render();
//            //var durationPlot = PlotTwoMetric(basicEval.Duration().Select(p => p.value).ToArray(),
//            //    extendedEval.Duration().Select(p => p.value).ToArray(), "Duration").Render();
//            //var visibilityPlot = PlotTwoMetric(basicEval.Visibility().Select(p => p.value).ToArray(),
//            //    extendedEval.Visibility().Select(p => p.value).ToArray(), "Visibility").Render();
//            //var pathSmoothnessPlot = PlotTwoMetric(basicEval.PathSmoothness().Select(p => p.value).ToArray(),
//            //    extendedEval.PathSmoothness().Select(p => p.value).ToArray(), "Path Roughness").Render();


//            //using var bmp = new Bitmap(1200, 500);
//            //using var gfx = Graphics.FromImage(bmp);
//            //gfx.DrawImage(pathPlot, 0, 0);
//            //gfx.DrawImage(durationPlot, 600, 0);
//            //gfx.DrawImage(visibilityPlot, 0, 250);
//            //gfx.DrawImage(pathSmoothnessPlot, 600, 250);
//            //bmp.Save("MultiPlot.bmp");
//        }

//        //[Fact]
//        //public void ComparisonByMapTypeOnlySuccess()
//        //{
//        //    var basic = File.ReadAllLines(FileRef.FinalPotential).Select(l => new BasicPotentialFieldAnalysis.Data(l) as Evaluate.IData).ToList();
//        //    var extended = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l) as Evaluate.IData).ToList();

//        //    var basicEval = new Evaluate(basic);
//        //    var extendedEval = new Evaluate(extended);

//        //    var pathPlot = PlotTwoMetric(basicEval.Path(true).Select(p => p.value).ToArray(), extendedEval.Path(true).Select(p => p.value).ToArray(), "Path Length").Render();
//        //    var durationPlot = PlotTwoMetric(basicEval.Duration(true).Select(p => p.value).ToArray(), extendedEval.Duration(true).Select(p => p.value).ToArray(), "Duration").Render();
//        //    var visibilityPlot = PlotTwoMetric(basicEval.Visibility(true).Select(p => p.value).ToArray(), extendedEval.Visibility(true).Select(p => p.value).ToArray(), "Visibility").Render();
//        //    var pathSmoothnessPlot = PlotTwoMetric(basicEval.PathSmoothness(true).Select(p => p.value).ToArray(), extendedEval.PathSmoothness(true).Select(p => p.value).ToArray(), "Path Roughness").Render();


//        //    using var bmp = new Bitmap(1200, 500);
//        //    using var gfx = Graphics.FromImage(bmp);
//        //    gfx.DrawImage(pathPlot, 0, 0);
//        //    gfx.DrawImage(durationPlot, 600, 0);
//        //    gfx.DrawImage(visibilityPlot, 0, 250);
//        //    gfx.DrawImage(pathSmoothnessPlot, 600, 250);
//        //    bmp.Save("MultiPlotOnlySuccess.bmp");
//        //}

//        [Fact]
//        public void Comparisons()
//        {
//            var basic = File.ReadAllLines(FileRef.FinalPotential).Select(l => new BasicPotentialFieldAnalysis.Data(l) as Evaluate.IData).ToList();
//            var extended = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l) as Evaluate.IData).ToList();
            
//            // success on all map types
//            //DefaultPlot(basic);
//            //DefaultPlot(extended);

//            //  comparison between the potential fields
//            var plt = new Plot(600, 400);

//            var basicBars = basic.GroupBy(b => b.MapName).OrderBy(g => CompareFields.ToOrderValue(g.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).ToArray();
//            var extendedBars = extended.GroupBy(b => b.MapName).OrderBy(g => CompareFields.ToOrderValue(g.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).ToArray();
            
//            _output.WriteLine($"==========================================================");
//            var bar1 = plt.AddBar(basicBars.Select(s => s.Success).ToArray(), new double[] { 1, 3, 5, 8, 10, 12, 15, 17, 19, 22, 24, 26, 29, 31, 33, 36, 38, 40 });
//            bar1.HatchStyle = HatchStyle.StripedUpwardDiagonal;
//            bar1.FillColor = Color.Gray;
//            bar1.FillColorHatch = Color.Black;
//            bar1.Label = "Potential Field";

//            var bar2 = plt.AddBar(extendedBars.Select(s => s.Success).ToArray(), new double[] { 2, 4, 6, 9, 11, 13, 16, 18, 20, 23, 25, 27, 30, 32, 34, 37, 39, 41 });
//            bar2.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
//            bar2.FillColor = Color.DodgerBlue;
//            bar2.FillColorHatch = Color.DeepSkyBlue;
//            bar2.Label = "Pheromone Potential Field";

//            // adjust axis limits so there is no padding below the bar graph
//            plt.SetAxisLimits(yMin: 0, yMax: 100);

//            plt.XTicks(new double[] { 3, 10, 17, 24, 31, 38 }, new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });

//            foreach (var i in new double[] { 7, 14, 21, 28, 35 })
//            {
//                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
//            }


//            // add a legend to display each labeled bar plot
//            plt.Legend(location: Alignment.UpperRight);
//            new FormsPlotViewer(plt).ShowDialog();
//        }

//        //private void DefaultPlot(List<Evaluate.IData> data)
//        //{
//        //    var success = new Evaluate(data).CalculateSuccess();

//        //    var plt = new Plot();

//        //    var bars = success.Select(s => s.value).ToArray();
//        //    var labels = success.Select(s => s.mapName).ToArray();
//        //    var positions = Enumerable.Range(0, 18).Select(x => (double)x).ToArray();

//        //    plt.AddBar(bars, positions);
//        //    plt.XTicks(positions, labels);
//        //    plt.SetAxisLimits(yMin: 0);

//        //    new FormsPlotViewer(plt).ShowDialog();
//        //}

//        private Plot PlotTwoMetric(double[] basic, double[] extended, string title)
//        {
//            var plt = new Plot(600, 250);

//            _output.WriteLine($"==========================================================");
//            var bar1 = plt.AddBar(basic, new double[] { 1, 3, 5, 8, 10, 12, 15, 17, 19, 22, 24, 26, 29, 31, 33, 36, 38, 40 });
//            bar1.HatchStyle = HatchStyle.StripedUpwardDiagonal;
//            bar1.FillColor = Color.Gray;
//            bar1.FillColorHatch = Color.Black;
//            bar1.Label = "Potential Field";

//            var bar2 = plt.AddBar(extended, new double[] { 2, 4, 6, 9, 11, 13, 16, 18, 20, 23, 25, 27, 30, 32, 34, 37, 39, 41 });
//            bar2.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
//            bar2.FillColor = Color.DodgerBlue;
//            bar2.FillColorHatch = Color.DeepSkyBlue;
//            bar2.Label = "Pheromone Potential Field";

//            // adjust axis limits so there is no padding below the bar graph
//            plt.SetAxisLimits(yMin: 0, yMax: 500);
//            plt.Title(title);
//            plt.XTicks(new double[] { 3, 10, 17, 24, 31, 38 }, new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });

//            foreach (var i in new double[] { 7, 14, 21, 28, 35 })
//            {
//                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
//            }


//            // add a legend to display each labeled bar plot
//            //plt.Legend(location: Alignment.UpperRight);
//            return plt;
//        }
//    }
//}
