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

                Constant = double.Parse(items[11]);
                StrengthIncrease = double.Parse(items[12]);
                Range = int.Parse(items[13]);
            }

            public double Constant { get; }
            public double StrengthIncrease { get; }
            public int Range { get; }
        }

        [Fact]
        public void FinalPheromoneScatter()
        {
            var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PfData(l)).ToList();
            var scatters = new List<BasicPotentialFieldAnalysis.Scatter>();

            foreach (var result in all.Where(w => w.Success))// && !w.MapName.Contains("One")))
            {
                var i = scatters.FirstOrDefault(i => Math.Abs(i.ObstacleRange - result.Constant) < 0.001 && Math.Abs(i.ObstacleConstant - result.StrengthIncrease) < 0.001 && Math.Abs(i.AttractiveConstant - result.Range) < 0.001);
                if (i == default)
                    scatters.Add(new BasicPotentialFieldAnalysis.Scatter(result.Constant, result.StrengthIncrease, result.Range));
                else
                    i.Counter += 1;
            }

            var byMap = all.GroupBy(m => m.MapName).Select(b => new { b.Key, Succes = b.Count(c => c.Success).Percentage(b.Count()) }).ToList();
            //var byParam = all.GroupBy(m => new { m.AttractiveConstant, m.ObstacleConstant, m.ObstacleRange }).Select(b => new { b.Key, Succes = b.Count(c => c.Success).Percentage(b.Count()) }).ToList();
            //var byParamFilter = all.Where(a => !a.MapName.Contains("One")).GroupBy(m => new { m.AttractiveConstant, m.ObstacleConstant, m.ObstacleRange }).Select(b => new { b.Key, Succes = b.Count(c => c.Success).Percentage(b.Count()) }).ToList();

            var byParam = scatters.Select(s => s.Counter.Percentage(360)).ToList();

            _output.WriteLine($"Success Rate: {all.Count(a => a.Success).Percentage(all.Count)}");
            _output.WriteLine($"MT: {(byMap.Select(g => g.Succes).StandardDeviation() * 100):F}\t{(byMap.Select(g => g.Succes).Min() * 100):F}\t{(byMap.Select(g => g.Succes).Percentile(0.25) * 100):F}\t{(byMap.Select(g => g.Succes).Percentile(0.5) * 100):F}\t{(byMap.Select(g => g.Succes).Percentile(0.75) * 100):F}\t{(byMap.Select(g => g.Succes).Max() * 100):F}");
            _output.WriteLine($"PT: {(byParam.StandardDeviation() * 100):F}\t{(byParam.Min() * 100):F}\t{(byParam.Percentile(0.25) * 100):F}\t{(byParam.Percentile(0.5) * 100):F}\t{(byParam.Percentile(0.75) * 100):F}\t{(byParam.Max() * 100):F}");
            //_output.WriteLine($"PT: {(byParam.Select(g => g.Succes).StandardDeviation() * 100):F}\t{(byParam.Select(g => g.Succes).Min() * 100):F}\t{(byParam.Select(g => g.Succes).Percentile(0.25) * 100):F}\t{(byParam.Select(g => g.Succes).Percentile(0.5) * 100):F}\t{(byParam.Select(g => g.Succes).Percentile(0.75) * 100):F}\t{(byParam.Select(g => g.Succes).Max() * 100):F}");
            //_output.WriteLine($"PTO: {(byParamFilter.Select(g => g.Succes).StandardDeviation() * 100):F}\t{(byParamFilter.Select(g => g.Succes).Min() * 100):F}\t{(byParamFilter.Select(g => g.Succes).Percentile(0.25) * 100):F}\t{(byParamFilter.Select(g => g.Succes).Percentile(0.5) * 100):F}\t{(byParamFilter.Select(g => g.Succes).Percentile(0.75) * 100):F}\t{(byParamFilter.Select(g => g.Succes).Max() * 100):F}");
            //var mapNames = all.Select(m => m.MapName).Distinct().ToList();
            //foreach (var mapName in mapNames)
            //{
            //    _output.WriteLine($"{mapName}");
            //}

            ////var max = 0.0;
            //foreach (var scatter in scatters)//.OrderByDescending(s => s.Counter).Take(10))
            //{
            //    //    //max = Math.Max(max, scatter.Counter.Percentage(360) * 100);
            //    _output.WriteLine($"{all.Count(i => !i.MapName.Contains("One") && Math.Abs(i.Constant - scatter.ObstacleRange) < 0.01 && Math.Abs(i.StrengthIncrease - scatter.ObstacleConstant) < 0.001 && Math.Abs(i.Range - scatter.AttractiveConstant) < 0.001)}"); 
            //    //    _output.WriteLine($"{scatter.ObstacleRange},{scatter.ObstacleConstant},{scatter.AttractiveConstant},{(scatter.Counter.Percentage(max) * 100)}");
            //}
            
        }

        [Fact]
        public void ExtensiveSearchAnalysis()
        {
            _output.WriteLine("Extensive Search Analysis");
            var all = File.ReadAllLines(FileRef.FinalPheromone).Select(l => new PfData(l)).ToList();
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
