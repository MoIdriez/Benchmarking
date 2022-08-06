using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Benchmarking.Helper;
using Benchmarking.Thesis.ChapterFour.Data;
using Benchmarking.Thesis.ChapterThree;
using Xunit;
using Xunit.Abstractions;
using Approach = Benchmarking.Thesis.ChapterThree.Evaluate.ApproachType;

namespace Benchmarking.Thesis.ChapterFour
{
    public class Tables
    {
        private readonly ITestOutputHelper _output;
        private readonly List<Evaluate.EvaluateData> _data;

        public Tables(ITestOutputHelper output)
        {
            _output = output;
            _data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();
        }

        [Fact]
        public void ChapterFourPfBig()
        {
            MiniInfo(Approach.PotentialField);
            MiniInfo(Approach.PheromoneField);
        }

        private void MiniInfo(Approach approach)
        {
            _output.WriteLine($"{approach}");
            var sb = new StringBuilder();
            //sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).Average():F}, ");
            //sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).Average():F}, ");

            //sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).StandardDeviation():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).Min():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).Percentile(0.25):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).Percentile(0.5):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).Percentile(0.75):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Steps).Max():F} || ");

            //sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).StandardDeviation():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).Min():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).Percentile(0.25):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).Percentile(0.5):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).Percentile(0.75):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Steps).Max():F} \\\\");
            _output.WriteLine(sb.ToString());
            
            sb = new StringBuilder();
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Visibility).Min():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Visibility).Percentile(0.25):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Visibility).Percentile(0.5):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Visibility).Percentile(0.75):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.Visibility).Max():F} || ");

            //sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Visibility).StandardDeviation():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Visibility).Min():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Visibility).Percentile(0.25):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Visibility).Percentile(0.5):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Visibility).Percentile(0.75):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.Visibility).Max():F} \\\\");
            _output.WriteLine(sb.ToString());
            
            sb = new StringBuilder();
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.PathSmoothness).Min():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.PathSmoothness).Percentile(0.25):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.PathSmoothness).Percentile(0.5):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.PathSmoothness).Percentile(0.75):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach).Select(s => s.PathSmoothness).Max():F} || ");

            //sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.PathSmoothness).StandardDeviation():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.PathSmoothness).Min():F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.PathSmoothness).Percentile(0.25):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.PathSmoothness).Percentile(0.5):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.PathSmoothness).Percentile(0.75):F}, ");
            sb.Append($"{_data.Where(d => d.Approach == approach && d.Success).Select(s => s.PathSmoothness).Max():F} \\\\");
            _output.WriteLine(sb.ToString());
        }

        [Fact]
        public void OverallScore()
        {
            var eval = new Evaluate(_data);
            var columns = new[] { "Approach", "Overall", "Success", "Path Length", "Path Smoothness", "Time", "Visibility" };
            var caption = "This table shows the scores of each metric in the range of [0.0-1.0] for all maps";
            var label = "scores_overall";
            var rows = new List<string[]>();
            var approaches = new[]
            {
                Approach.BaseLine, Approach.AStar, Approach.RRT, Approach.RRTExtended, Approach.PotentialField,
                Approach.PheromoneField
            };
            foreach (var approach in approaches)
            {
                var score = eval.Score(approach, 0.75, 0.75, 0.25);
                var success = eval.CalculateSuccess(approach).Any() ? eval.CalculateSuccess(approach).Average(a => a.Value) : -1;
                var path = eval.Path(approach).Any() ? eval.Path(approach).Average(a => a.Value) : -1;
                var smoothness = eval.PathSmoothness(approach).Any() ? eval.PathSmoothness(approach).Average(a => a.Value) : -1;
                var duration = eval.Duration(approach).Any() ? eval.Duration(approach).Average(a => a.Value) : -1;
                var visibility = eval.Visibility(approach).Any() ? eval.Visibility(approach).Average(a => a.Value) : -1;

                rows.Add(new []{ $"{approach}", $"{score:F}", $"{success:F}", $"{path:F}", $"{smoothness:F}", $"{duration:F}", $"{visibility:F}" });
            }

            var tableLines = Latex.Table(columns, rows, caption, label);

            foreach (var line in tableLines)
            {
                _output.WriteLine(line);
            }
        }

        [Fact]
        public void ScoreStaticMaps()
        {
            var eval = new Evaluate(_data.Where(m => Evaluate.IsStaticMap(m.Map)).ToList());
            var columns = new[] { "Approach", "Overall", "Success", "Path Length", "Path Smoothness", "Time", "Visibility" };
            var caption = "This table shows the scores of each metric in the range of [0.0-1.0] for all static maps";
            var label = "scores_static";
            var rows = new List<string[]>();
            var approaches = new[]
            {
                Approach.BaseLine, Approach.AStar, Approach.RRT, Approach.RRTExtended, Approach.PotentialField,
                Approach.PheromoneField
            };
            foreach (var approach in approaches)
            {
                var score = eval.Score(approach, 0.75, 0.75, 0.25);
                var success = eval.CalculateSuccess(approach).Any() ? eval.CalculateSuccess(approach).Average(a => a.Value) : -1;
                var path = eval.Path(approach).Any() ? eval.Path(approach).Average(a => a.Value) : -1;
                var smoothness = eval.PathSmoothness(approach).Any() ? eval.PathSmoothness(approach).Average(a => a.Value) : -1;
                var duration = eval.Duration(approach).Any() ? eval.Duration(approach).Average(a => a.Value) : -1;
                var visibility = eval.Visibility(approach).Any() ? eval.Visibility(approach).Average(a => a.Value) : -1;

                rows.Add(new []{ $"{approach}", $"{score:F}", $"{success:F}", $"{path:F}", $"{smoothness:F}", $"{duration:F}", $"{visibility:F}" });
            }

            var tableLines = Latex.Table(columns, rows, caption, label);

            foreach (var line in tableLines)
            {
                _output.WriteLine(line);
            }
        }

        [Fact]
        public void ScoreGeneratedMaps()
        {
            var eval = new Evaluate(_data.Where(m => !Evaluate.IsStaticMap(m.Map)).ToList());
            var columns = new[] { "Approach", "Overall", "Success", "Path Length", "Path Smoothness", "Time", "Visibility" };
            var caption = "This table shows the scores of each metric in the range of [0.0-1.0] for all generated maps";
            var label = "scores_gen";
            var rows = new List<string[]>();
            var approaches = new[]
            {
                Approach.BaseLine, Approach.AStar, Approach.RRT, Approach.RRTExtended, Approach.PotentialField,
                Approach.PheromoneField
            };
            foreach (var approach in approaches)
            {
                var score = eval.Score(approach, 0.75, 0.75, 0.25);
                var success = eval.CalculateSuccess(approach).Any() ? eval.CalculateSuccess(approach).Average(a => a.Value) : -1;
                var path = eval.Path(approach).Any() ? eval.Path(approach).Average(a => a.Value) : -1;
                var smoothness = eval.PathSmoothness(approach).Any() ? eval.PathSmoothness(approach).Average(a => a.Value) : -1;
                var duration = eval.Duration(approach).Any() ? eval.Duration(approach).Average(a => a.Value) : -1;
                var visibility = eval.Visibility(approach).Any() ? eval.Visibility(approach).Average(a => a.Value) : -1;

                rows.Add(new []{ $"{approach}", $"{score:F3}", $"{success:F3}", $"{path:F}", $"{smoothness:F}", $"{duration:F}", $"{visibility:F}" });
            }

            var tableLines = Latex.Table(columns, rows, caption, label);

            foreach (var line in tableLines)
            {
                _output.WriteLine(line);
            }
        }

        [Fact]
        public void RrtFailTable()
        {
            var genMaps = Evaluate.GetGenMaps();
            var staticMaps = Evaluate.GetStaticMaps();

            var data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();
            
            

        }


        [Fact]
        public void AppendixTables()
        {
            foreach (Evaluate.MapType mapType in Enum.GetValues(typeof(Evaluate.MapType)))
            {
                var eval = new Evaluate(_data.Where(m => m.Map == mapType).ToList());

                var columns = new[] { $"\\textbf{{{mapType}}}", "Overall", "Success", "Path Sc", "Smooth Sc", "Time Sc", "Vis Sc" };
                var caption = $"This table shows the scores for the map type: '{mapType}' in the range of [0.0-1.0]";
                var label = $"scores_{mapType}";
                var rows = new List<string[]>();
                var approaches = new[]
                {
                    Approach.BaseLine, Approach.AStar, Approach.RRT, Approach.RRTExtended, Approach.PotentialField,
                    Approach.PheromoneField
                };
                foreach (var approach in approaches)
                {
                    var score = eval.Score(approach, 0.75, 0.75, 0.25);
                    var success = eval.CalculateSuccess(approach).Any() ? eval.CalculateSuccess(approach).Average(a => a.Value) : 0;
                    var path = eval.Path(approach).Any() ? eval.Path(approach).Average(a => a.Value) : 0;
                    var smoothness = eval.PathSmoothness(approach).Any() ? eval.PathSmoothness(approach).Average(a => a.Value) : 0;
                    var duration = eval.Duration(approach).Any() ? eval.Duration(approach).Average(a => a.Value) : 0;
                    var visibility = eval.Visibility(approach).Any() ? eval.Visibility(approach).Average(a => a.Value) : 0;
                    
                    rows.Add(new[] { $"{approach}", $"{score:F3}", $"{success:F}", $"{path:F}", $"{smoothness:F}", $"{duration:F}", $"{visibility:F}" });
                }

                var tableLines = Latex.Table(columns, rows, caption, label);

                foreach (var line in tableLines)
                {
                    _output.WriteLine(line);
                }

                _output.WriteLine(string.Empty);
                _output.WriteLine(string.Empty);
                _output.WriteLine(string.Empty);
            }
        }
    }
}
