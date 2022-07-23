using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
