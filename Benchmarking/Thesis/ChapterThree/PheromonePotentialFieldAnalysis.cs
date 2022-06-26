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
    public class PheromonePotentialFieldAnalysis
    {
        private readonly ITestOutputHelper _output;
        private readonly Random _random = new();

        public PheromonePotentialFieldAnalysis(ITestOutputHelper output)
        {
            _output = output;
        }

        public class PfData : BasicPotentialFieldAnalysis.Data
        {
            public PfData(string line) : base(line)
            {
                var items = line.Split(",");

                Constant = double.Parse(items[10]);
                StrengthIncrease = double.Parse(items[11]);
                Range = int.Parse(items[11]);
            }

            public double Constant { get; }
            public double StrengthIncrease { get; }
            public int Range { get; }
        }

        [Fact]
        public void ExtensiveSearchAnalysis()
        {
            _output.WriteLine("Extensive Search Analysis");
            var all = File.ReadAllLines(FileRef.ExtensiveSearchPheromone).Select(l => new PfData(l)).ToList();
            foreach (var g in all.GroupBy(g => g.MapName).OrderBy(g => g.Key))
            {
                _output.WriteLine($"Success {g.Key} rate: {g.Count(r => r.Success)} / {g.Count()} ({g.Count(r => r.Success).Percentage(g.Count())}%)");
            }
            _output.WriteLine($"==========================================================");
            _output.WriteLine($"Max: {all.Where(a => a.Success).Max(s => s.Steps)}, Min:{all.Where(a => a.Success).Min(s => s.Steps)}");

            var enumerable = all
                .GroupBy(g => new { g.Constant, g.StrengthIncrease, g.Range })
                .Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) })
                .OrderByDescending(g => g.Success)
                ;
            foreach (var g in enumerable)
            {
                _output.WriteLine($"{g.Key.Constant},{g.Key.StrengthIncrease},{g.Key.Range},{g.Success}");
            }

            var plt = new ScottPlot.Plot();

            var barInfo = all.GroupBy(g => g.MapName).Select(g => new { MapName = g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).OrderByDescending(g => g.Success).ToArray();

            var bars = barInfo.Select(g => g.Success).ToArray();
            var positions = Enumerable.Range(0, 18).Select(x => (double)x).ToArray();
            var labels = barInfo.Select(g => g.MapName).ToArray();

            //var bars = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var barsOrdered = all.GroupBy(g => g.MapName).Select(g => g.Count(r => r.Success).Percentage(g.Count())).ToArray();
            //var positions = new double[] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16, 17};
            //var labels = all.Select(g => g.MapName).Distinct().ToArray();

            plt.AddBar(bars, positions);
            plt.XTicks(positions, labels);
            plt.SetAxisLimits(yMin: 0);

            new ScottPlot.FormsPlotViewer(plt).ShowDialog();
        }
    }
}
