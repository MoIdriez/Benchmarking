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
    public class CompareFields
    {
        private readonly ITestOutputHelper _output;
        public CompareFields(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void Compare()
        {
            _output.WriteLine("Compare Fields Analysis");

            var basic = File.ReadAllLines(FileRef.ExtensiveSearch).Select(l => new BasicPotentialFieldAnalysis.Data(l)).ToList();
            var extended = File.ReadAllLines(FileRef.ExtensiveSearchPheromone).Select(l => new PheromonePotentialFieldAnalysis.PfData(l)).ToList();

            var plt = new ScottPlot.Plot(600, 400);

            var basicBars = basic.GroupBy(b => b.MapName).OrderBy(g => ToOrderValue(g.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).ToArray();
            var extendedBars = extended.GroupBy(b => b.MapName).OrderBy(g => ToOrderValue(g.Key)).Select(g => new { g.Key, Success = g.Count(r => r.Success).Percentage(g.Count()) }).ToArray();
            _output.WriteLine($"==========================================================");
            _output.WriteLine("Basic:");
            foreach (var basicBar in basicBars) _output.WriteLine($"{basicBar.Key}: {basicBar.Success}");
            _output.WriteLine("Extended:");
            foreach (var basicBar in extendedBars) _output.WriteLine($"{basicBar.Key}: {basicBar.Success}");


            _output.WriteLine($"==========================================================");
            var bar1 = plt.AddBar(basicBars.Select(s => s.Success).ToArray(), new double[] { 1, 3, 5, 8, 10, 12, 15, 17, 19, 22, 24, 26, 29, 31, 33, 36, 38, 40 });
            bar1.HatchStyle = HatchStyle.StripedUpwardDiagonal;
            bar1.FillColor = Color.Gray;
            bar1.FillColorHatch = Color.Black;
            bar1.Label = "Potential Field";

            var bar2 = plt.AddBar(extendedBars.Select(s => s.Success).ToArray(), new double[] { 2, 4, 6, 9, 11, 13, 16, 18, 20, 23, 25, 27, 30, 32, 34, 37, 39, 41 });
            bar2.HatchStyle = HatchStyle.StripedWideDownwardDiagonal;
            bar2.FillColor = Color.DodgerBlue;
            bar2.FillColorHatch = Color.DeepSkyBlue;
            bar2.Label = "Pheromone Potential Field";

            // adjust axis limits so there is no padding below the bar graph
            plt.SetAxisLimits(yMin: 0, yMax: 100);

            plt.XTicks(new double[] { 3, 10, 17, 24, 31, 38 }, new[] { "Walls", "Slits", "Rooms", "PlankPiles", "Corridors", "BugTraps" });

            foreach (var i in new double[] { 7, 14, 21, 28, 35})
            {
                plt.AddVerticalLine(i, Color.Black, 2, LineStyle.Dot);
            }
            

            // add a legend to display each labeled bar plot
            plt.Legend(location: Alignment.UpperRight);
            new FormsPlotViewer(plt).ShowDialog();
        }

        public static int ToOrderValue(string mapName)
        {
            if (mapName == "WallOne") return 0;
            if (mapName == "WallTwo") return 1;
            if (mapName == "WallThree") return 2;

            if (mapName == "SlitOne") return 3;
            if (mapName == "SlitTwo") return 4;
            if (mapName == "SlitThree") return 5;

            if (mapName == "RoomOne") return 6;
            if (mapName == "RoomTwo") return 7;
            if (mapName == "RoomThree") return 8;

            if (mapName == "PlankPileOne") return 9;
            if (mapName == "PlankPileTwo") return 10;
            if (mapName == "PlankPileThree") return 11;

            if (mapName == "CorridorOne") return 12;
            if (mapName == "CorridorTwo") return 13;
            if (mapName == "CorridorThree") return 14;

            if (mapName == "BugTrapOne") return 15;
            if (mapName == "BugTrapTwo") return 16;
            if (mapName == "BugTrapThree") return 17;
            return -1;
        }
    }
}
