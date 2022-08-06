using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Thesis.ChapterThree.Data;
using ScottPlot;
using ScottPlot.Drawing;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterThree
{
    public class FinalAnalysis
    {
        private readonly ITestOutputHelper _output;
        private readonly Random _random = new();

        public FinalAnalysis(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void QuickCheck()
        {
            var data = File.ReadAllLines(FileRef.FinalAll).Select(l => new Evaluate.EvaluateData(l)).ToList();
            var eval = new Evaluate(data);
            var mapNames = eval.BaselineData.Select(m => m.Map).Distinct().OrderBy(CompareFields.ToOrderValue).ToList();

            var baseSuccess = eval.CalculateSuccess(Evaluate.ApproachType.BaseLine);
            var pheromoneSuccess = eval.CalculateSuccess(Evaluate.ApproachType.PheromoneField);
            var potentialSuccess = eval.CalculateSuccess(Evaluate.ApproachType.PotentialField);
            _output.WriteLine($"Success: {baseSuccess.Average(s => s.Value):F}|{pheromoneSuccess.Average(s => s.Value):F}|{potentialSuccess.Average(s => s.Value):F}");
            foreach (var map in mapNames)
            {
                _output.WriteLine($"{map}: {baseSuccess[map]:F}| {pheromoneSuccess[map]:F}| {potentialSuccess[map]:F}");
            }
            _output.WriteLine($"__________________________________________");
            foreach (var evaluateData in data.Where(d => d.Approach == Evaluate.ApproachType.BaseLine && !d.Success))
            {
                _output.WriteLine($"{evaluateData.Id},{evaluateData.Map}");
            }
        }

        [Fact]
        public void GenerateScatterPheromone()
        {
            var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l)).ToList();

            foreach (var scatter in all.GroupBy(g => new { g.Range, g.Constant, g.StrengthIncrease }).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }))
                _output.WriteLine($"{scatter.Key.Constant},{scatter.Key.StrengthIncrease},{scatter.Key.Range},{scatter.Success}");
        }

        [Fact]
        public void PotentialFieldAllPlot()
        {
            var data = File.ReadAllLines(FileRef.FinalAll).Select(l => new Evaluate.EvaluateData(l)).ToList();
            var eval = new Evaluate(data);

            var plot1 = SimplePlot(eval.CalculateSuccess(Evaluate.ApproachType.PotentialField), 600, 500);//.OrderBy(d => getorde);//.OrderByDescending(d => d.Value);
            var plot2 = SimplePlot(eval.CalculateSuccess(Evaluate.ApproachType.PotentialField).OrderByDescending(d => d.Value).ToDictionary(x => x.Key, y => y.Value), 600, 500);//.OrderBy(d => getorde);//.OrderByDescending(d => d.Value);
            
            using var bmp = new Bitmap(1200, 500);
            using var gfx = Graphics.FromImage(bmp);
            gfx.DrawImage(plot1.Render(), 0, 0);
            gfx.DrawImage(plot2.Render(), 600, 0);
            bmp.Save("AllPotentialFieldThings.bmp");
        }

        private Plot SimplePlot(Dictionary<Evaluate.MapType, double> barInfo, int x, int y)
        {
            var plt = new Plot(x,y);
            //var barInfo = all.GroupBy(g => g.MapName).Select(g => new { MapName = g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).OrderByDescending(g => g.Success).ToArray();

            //plt.AddText("WallOne", 0, 5, size: 16, color: Color.Black);

            var bars = barInfo.Select(g => g.Value).ToArray();
            var positions = Enumerable.Range(0, 18).Select(x => (double)x).ToArray();
            var labels = barInfo.Select(g => Simplify(g.Key)).ToArray();
            
            //var bars = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var barsOrdered = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var positions = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17};
            //var labels = all.Select(g => g.MapName).Distinct().ToArray();

            plt.AddBar(bars, positions);
            plt.XTicks(positions, labels);
            plt.SetAxisLimits(yMin: 0);

            plt.XLabel("Environments");
            plt.YLabel("Average Success");

            return plt;
        }

        private string Simplify(Evaluate.MapType mapName)
        {
            return mapName.ToString()
                .Replace("Wall", "W")
                .Replace("Slit", "S")
                .Replace("Room", "R")
                .Replace("PlankPile", "P")
                .Replace("Corridor", "C")
                .Replace("BugTrap", "B")
                .Replace("One", "1")
                .Replace("Two", "2")
                .Replace("Three", "3");
        }

        [Fact]
        public void CalculateEffectivenessPheromone()
        {
            //var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l) as Evaluate.IData).ToList();
            //var evaluate = new Evaluate(all);
            //evaluate.Compare(_output);
        }

        [Fact]
        public void ComparisonByMapType()
        {
            var data = File.ReadAllLines(FileRef.FinalAll).Select(l => new Evaluate.EvaluateData(l)).ToList();
            var eval = new Evaluate(data);

            //_output.WriteLine($"Path Avg: {data.Where(d => d.Approach == Evaluate.ApproachType.BaseLine).Average(s => s.Steps)}| {data.Where(d => d.Approach == Evaluate.ApproachType.PheromoneField).Average(s => s.Steps)}| {data.Where(d => d.Approach == Evaluate.ApproachType.PotentialField).Average(s => s.Steps)}|");
            //_output.WriteLine($"Time Avg: {data.Where(d => d.Approach == Evaluate.ApproachType.BaseLine).Average(s => s.Time)}| {data.Where(d => d.Approach == Evaluate.ApproachType.PheromoneField).Average(s => s.Time)}| {data.Where(d => d.Approach == Evaluate.ApproachType.PotentialField).Average(s => s.Time)}|");

            var m = data.Count(d => d.Approach == Evaluate.ApproachType.PotentialField);
            var n = data.Count(d => d.Approach == Evaluate.ApproachType.PheromoneField);

            var ppath = eval.Path(Evaluate.ApproachType.PotentialField);
            var pfpath = eval.Path(Evaluate.ApproachType.PheromoneField);

            var pv = ppath.Select(p => p.Value).ToList();
            pv.Insert(10, 0);
            pv.Add(0);
            pv.Add(0);
            pv.Add(0);
            pv.Add(0);

            var pathPlot = PlotTwoMetric(pv.ToArray(), pfpath.Select(p => p.Value).ToArray(), "Path Length Score").Render();
            var doubles = eval.Duration(Evaluate.ApproachType.PotentialField).Select(p => p.Value).ToList();
            doubles.Insert(10, 0);
            doubles.Add(0);
            doubles.Add(0);
            doubles.Add(0);
            doubles.Add(0);
            var durationPlot = PlotTwoMetric(doubles.ToArray(), eval.Duration(Evaluate.ApproachType.PheromoneField).Select(p => p.Value).ToArray(), "Time Score").Render();
            var array = eval.Visibility(Evaluate.ApproachType.PotentialField).Select(p => p.Value).ToList();
            array.Insert(10, 0);
            array.Add(0);
            array.Add(0);
            array.Add(0);
            array.Add(0);
            var visibilityPlot = PlotTwoMetric(array.ToArray(), eval.Visibility(Evaluate.ApproachType.PheromoneField).Select(p => p.Value).ToArray(), "Visibility Score").Render();
            var basic = eval.PathSmoothness(Evaluate.ApproachType.PotentialField).Select(p => p.Value).ToList();
            basic.Insert(10, 0);
            basic.Add(0);
            basic.Add(0);
            basic.Add(0);
            basic.Add(0);
            var pathSmoothnessPlot = PlotTwoMetric(basic.ToArray(), eval.PathSmoothness(Evaluate.ApproachType.PheromoneField).Select(p => p.Value).ToArray(), "Path Smoothness Score").Render();

            using var bmp = new Bitmap(1200, 500);
            using var gfx = Graphics.FromImage(bmp);
            gfx.DrawImage(pathPlot, 0, 0);
            gfx.DrawImage(durationPlot, 600, 0);
            gfx.DrawImage(visibilityPlot, 0, 250);
            gfx.DrawImage(pathSmoothnessPlot, 600, 250);
            bmp.Save("MultiPlot.bmp");
        }

        //[Fact]
        //public void ComparisonByMapTypeOnlySuccess()
        //{
        //    var basic = File.ReadAllLines(FileRef.FinalPotential).Select(l => new BasicPotentialFieldAnalysis.Data(l) as Evaluate.IData).ToList();
        //    var extended = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l) as Evaluate.IData).ToList();

        //    var basicEval = new Evaluate(basic);
        //    var extendedEval = new Evaluate(extended);

        //    var pathPlot = PlotTwoMetric(basicEval.Path(true).Select(p => p.value).ToArray(), extendedEval.Path(true).Select(p => p.value).ToArray(), "Path Length").Render();
        //    var durationPlot = PlotTwoMetric(basicEval.Duration(true).Select(p => p.value).ToArray(), extendedEval.Duration(true).Select(p => p.value).ToArray(), "Duration").Render();
        //    var visibilityPlot = PlotTwoMetric(basicEval.Visibility(true).Select(p => p.value).ToArray(), extendedEval.Visibility(true).Select(p => p.value).ToArray(), "Visibility").Render();
        //    var pathSmoothnessPlot = PlotTwoMetric(basicEval.PathSmoothness(true).Select(p => p.value).ToArray(), extendedEval.PathSmoothness(true).Select(p => p.value).ToArray(), "Path Roughness").Render();


        //    using var bmp = new Bitmap(1200, 500);
        //    using var gfx = Graphics.FromImage(bmp);
        //    gfx.DrawImage(pathPlot, 0, 0);
        //    gfx.DrawImage(durationPlot, 600, 0);
        //    gfx.DrawImage(visibilityPlot, 0, 250);
        //    gfx.DrawImage(pathSmoothnessPlot, 600, 250);
        //    bmp.Save("MultiPlotOnlySuccess.bmp");
        //}

        [Fact]
        public void Comparisons()
        {
            //var basic = File.ReadAllLines(FileRef.FinalPotential).Select(l => new BasicPotentialFieldAnalysis.Data(l) as Evaluate.IData).ToList();
            //var extended = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l) as Evaluate.IData).ToList();

            //// success on all map types
            ////DefaultPlot(basic);
            ////DefaultPlot(extended);

            ////  comparison between the potential fields
            //var plt = new Plot(600, 400);

            //var basicBars = basic.GroupBy(b => b.MapName).OrderBy(g => CompareFields.ToOrderValue(g.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).ToArray();
            //var extendedBars = extended.GroupBy(b => b.MapName).OrderBy(g => CompareFields.ToOrderValue(g.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).ToArray();

            //_output.WriteLine($"==========================================================");
            //var bar1 = plt.AddBar(basicBars.Select(s => s.Success).ToArray(), new double[] { 1, 3, 5, 8, 10, 12, 15, 17, 19, 22, 24, 26, 29, 31, 33, 36, 38, 40 });
            //bar1.HatchStyle = HatchStyle.StripedUpwardDiagonal;
            //bar1.FillColor = Color.Gray;
            //bar1.FillColorHatch = Color.Black;
            //bar1.Label = "Potential Field";

            //var bar2 = plt.AddBar(extendedBars.Select(s => s.Success).ToArray(), new double[] { 2, 4, 6, 9, 11, 13, 16, 18, 20, 23, 25, 27, 30, 32, 34, 37, 39, 41 });
            //bar2.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
            //bar2.FillColor = Color.DodgerBlue;
            //bar2.FillColorHatch = Color.DeepSkyBlue;
            //bar2.Label = "Pheromone Potential Field";

            //// adjust axis limits so there is no padding below the bar graph
            //plt.SetAxisLimits(yMin: 0, yMax: 100);

            //plt.XTicks(new double[] { 3, 10, 17, 24, 31, 38 }, new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });

            //foreach (var i in new double[] { 7, 14, 21, 28, 35 })
            //{
            //    plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            //}


            //// add a legend to display each labeled bar plot
            //plt.Legend(location: Alignment.UpperRight);
            //new FormsPlotViewer(plt).ShowDialog();
        }

        //private void DefaultPlot(List<Evaluate.IData> data)
        //{
        //    var success = new Evaluate(data).CalculateSuccess();

        //    var plt = new Plot();

        //    var bars = success.Select(s => s.value).ToArray();
        //    var labels = success.Select(s => s.mapName).ToArray();
        //    var positions = Enumerable.Range(0, 18).Select(x => (double)x).ToArray();

        //    plt.AddBar(bars, positions);
        //    plt.XTicks(positions, labels);
        //    plt.SetAxisLimits(yMin: 0);

        //    new FormsPlotViewer(plt).ShowDialog();
        //}

        private Plot PlotTwoMetric(double[] basic, double[] extended, string title)
        {
            var plt = new Plot(600, 250);

            basic = basic.Select(b => b*100).ToArray();
            extended = extended.Select(b => b*100).ToArray();

            _output.WriteLine($"==========================================================");
            var bar1 = plt.AddBar(basic, new double[] { 1, 3, 5, 8, 10, 12, 15, 17, 19, 22, 24, 26, 29, 31, 33, 36, 38, 40 });
            bar1.HatchStyle = HatchStyle.StripedUpwardDiagonal;
            bar1.FillColor = Color.Gray;
            bar1.FillColorHatch = Color.Black;
            bar1.Label = "Potential Field";

            var bar2 = plt.AddBar(extended, new double[] { 2, 4, 6, 9, 11, 13, 16, 18, 20, 23, 25, 27, 30, 32, 34, 37, 39, 41 });
            bar2.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
            bar2.FillColor = Color.DodgerBlue;
            bar2.FillColorHatch = Color.DeepSkyBlue;
            bar2.Label = "Pheromone Potential Field";

            // adjust axis limits so there is no padding below the bar graph
            plt.SetAxisLimits(yMin: 0, yMax: 100);
            plt.Title(title);
            plt.XTicks(new double[] { 3, 10, 17, 24, 31, 38 }, new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });

            foreach (var i in new double[] { 7, 14, 21, 28, 35 })
            {
                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            }


            // add a legend to display each labeled bar plot
            //plt.Legend(location: Alignment.UpperRight);
            return plt;
        }
    }
}
