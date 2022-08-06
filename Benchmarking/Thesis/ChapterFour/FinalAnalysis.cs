using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Benchmarking.Thesis.ChapterFour.Data;
using Benchmarking.Thesis.ChapterThree;
using Xunit;
using Xunit.Abstractions;

namespace Benchmarking.Thesis.ChapterFour
{
    public class FinalAnalysis
    {
        private readonly ITestOutputHelper _output;

        public FinalAnalysis(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public void TreeComparisons()
        {
            var data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();
            var staticMaps = Evaluate.GetStaticMaps();
            var genMaps = Evaluate.GetGenMaps();
            _output.WriteLine($"RRT Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"RRT Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"RRT Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"RRT Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success)}");
            _output.WriteLine($"RRT Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRT)}");

            _output.WriteLine($"RRTExtended Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"RRTExtended Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"RRTExtended Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"RRTExtended Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success)}");
            _output.WriteLine($"RRTExtended Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended)}");

            _output.WriteLine($"Static maps");
            _output.WriteLine($"RRT Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"RRT Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"RRT Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"RRT Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && staticMaps.Contains(r.Map))}");
            _output.WriteLine($"RRT Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRT && staticMaps.Contains(r.Map))}");

            _output.WriteLine($"RRTExtended Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"RRTExtended Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"RRTExtended Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"RRTExtended Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && staticMaps.Contains(r.Map))}");
            _output.WriteLine($"RRTExtended Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && staticMaps.Contains(r.Map))}");

            _output.WriteLine($"Generated maps");
            _output.WriteLine($"RRT Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"RRT Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"RRT Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"RRT Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRT && !r.Success && genMaps.Contains(r.Map))}");
            _output.WriteLine($"RRT Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRT && genMaps.Contains(r.Map))}");

            _output.WriteLine($"RRTExtended Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"RRTExtended Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"RRTExtended Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"RRTExtended Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && genMaps.Contains(r.Map))}");
            _output.WriteLine($"RRTExtended Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && genMaps.Contains(r.Map))}");

            foreach (var mapType in genMaps)
            {
                _output.WriteLine($"{mapType}:");
                _output.WriteLine($"RRT Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && mapType == (r.Map)).Count(r => r.Steps > 1000)}");
                _output.WriteLine($"RRT Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && mapType == (r.Map)).Count(r => r.Time > 9_999)}");
                _output.WriteLine($"RRT Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && mapType == (r.Map))}");
                _output.WriteLine($"RRT Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && mapType == (r.Map))}");

                _output.WriteLine($"RRTExtended Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && mapType == (r.Map)).Count(r => r.Steps > 1000)}");
                _output.WriteLine($"RRTExtended Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && mapType == (r.Map)).Count(r => r.Time > 9_999)}");
                _output.WriteLine($"RRTExtended Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && !r.Success && mapType == (r.Map))}");
                _output.WriteLine($"RRTExtended Total S: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && r.Success && mapType == (r.Map))}");
                _output.WriteLine($"RRTExtended Total: {data.Count(r => r.Approach == Evaluate.ApproachType.RRTExtended && mapType == (r.Map))}");
            }

        }

        [Fact]
        public void PffComparisons()
        {
            var data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();
            var staticMaps = Evaluate.GetStaticMaps();
            var genMaps = Evaluate.GetGenMaps();
            _output.WriteLine($"PheromoneField Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"PheromoneField Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"PheromoneField Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"PheromoneField Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success)}");
            _output.WriteLine($"PheromoneField Total: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField)}");

            _output.WriteLine($"Static maps");
            
            _output.WriteLine($"PheromoneField Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"PheromoneField Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"PheromoneField Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && staticMaps.Contains(r.Map)).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"PheromoneField Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && staticMaps.Contains(r.Map))}");
            _output.WriteLine($"PheromoneField Total: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && staticMaps.Contains(r.Map))}");

            _output.WriteLine($"Generated maps");
            _output.WriteLine($"PheromoneField Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Steps > 1000)}");
            _output.WriteLine($"PheromoneField Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Time > 9_999)}");
            _output.WriteLine($"PheromoneField Other Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && genMaps.Contains(r.Map)).Count(r => r.Steps < 1000 && r.Time < 9_999)}");
            _output.WriteLine($"PheromoneField Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && genMaps.Contains(r.Map))}");
            _output.WriteLine($"PheromoneField Total: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && genMaps.Contains(r.Map))}");

            foreach (var mapType in genMaps)
            {
                _output.WriteLine($"{mapType}:");
                _output.WriteLine($"PheromoneField Steps Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && mapType == (r.Map)).Count(r => r.Steps > 1000)}");
                _output.WriteLine($"PheromoneField Time Fail: {data.Where(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && mapType == (r.Map)).Count(r => r.Time > 9_999)}");
                _output.WriteLine($"PheromoneField Total Fail: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && !r.Success && mapType == (r.Map))}");
                _output.WriteLine($"PheromoneField Total S: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && r.Success && mapType == (r.Map))}");
                _output.WriteLine($"PheromoneField Total: {data.Count(r => r.Approach == Evaluate.ApproachType.PheromoneField && mapType == (r.Map))}");
            }

        }

        [Fact]
        public void Overview()
        {
            var data = File.ReadAllLines(FileRef.AllRunsFinal).Select(d => new Evaluate.EvaluateData(d)).ToList();

            EvaluateScore(new Evaluate(data));
            _output.WriteLine("==========================================================");
            _output.WriteLine($"StaticMaps");
            EvaluateScore(new Evaluate(data.Where(m => Evaluate.IsStaticMap(m.Map)).ToList()));
            _output.WriteLine("==========================================================");
            _output.WriteLine($"GenMaps");
            EvaluateScore(new Evaluate(data.Where(m => !Evaluate.IsStaticMap(m.Map)).ToList()));
            _output.WriteLine("==========================================================");


            _output.WriteLine("Evaluating maps on their best approaches");
            _output.WriteLine("----------------------------------------------------------");
            foreach (Evaluate.MapType mapType in Enum.GetValues(typeof(Evaluate.MapType)))
            {
                _output.WriteLine("==========================================================");
                _output.WriteLine($"Map: {mapType}");
                _output.WriteLine("----------------------------------------------------------");
                var all = data.Where(m => m.Map == mapType).ToList();
                var eval = new Evaluate(all);
                var scores = (Enum.GetValues(typeof(Evaluate.ApproachType))
                    .Cast<Evaluate.ApproachType>()
                    .Select(approach => (approach,
                        value: eval.Score(approach, pathWeight: 0.5, durationWeight: 0.5, pathSmoothnessWeight: 0.5)))).ToList();
                foreach (var score in scores.OrderByDescending(s => s.value))
                {
                    //if (score.approach is not (Evaluate.ApproachType.AStar))
                        _output.WriteLine($"{score.approach}:\t" +
                                          $"{(score.approach == Evaluate.ApproachType.RRT ? "\t\t" : "")}" +
                                          $"{(score.approach == Evaluate.ApproachType.AStar ? "\t\t" : "")}" +
                                          $"{(score.approach == Evaluate.ApproachType.BaseLine ? "\t" : "")}" +
                                          $"{score.value:F3}" +
                                          $" | S:{(eval.CalculateSuccess(score.approach).Any() ? eval.CalculateSuccess(score.approach).Average(a => a.Value) : 0):F3}" +
                                          $" | P:{(eval.Path(score.approach).Any() ? eval.Path(score.approach).Average(a => a.Value) : 0):F3}" +
                                          $" | D:{(eval.Duration(score.approach).Any() ? eval.Duration(score.approach).Average(a => a.Value) : 0):F3}" +
                                          $" | V:{(eval.Visibility(score.approach).Any() ? eval.Visibility(score.approach).Average(a => a.Value) : 0):F3}" +
                                          $" | M:{(eval.PathSmoothness(score.approach).Any() ? eval.PathSmoothness(score.approach).Average(a => a.Value) : 0):F3}");
                }
            }
        }



        private void EvaluateScore(Evaluate eval)
        {
            foreach (Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)))
            {
                var score = eval.Score(approach, 0.75, 0.75, 0.25);
                _output.WriteLine($"{approach}: Overall score: {score:F}" +
                    $" | S:{(eval.CalculateSuccess(approach).Any() ? eval.CalculateSuccess(approach).Average(a => a.Value) : -1):F}" +
                    $" | P:{(eval.Path(approach).Any() ? eval.Path(approach).Average(a => a.Value) : -1):F}" +
                    $" | D:{(eval.Duration(approach).Any() ? eval.Duration(approach).Average(a => a.Value) : -1):F}" +
                    $" | V:{(eval.Visibility(approach).Any() ? eval.Visibility(approach).Average(a => a.Value) : -1):F}" +
                    $" | M:{(eval.PathSmoothness(approach).Any() ? eval.PathSmoothness(approach).Average(a => a.Value) : -1):F}");
                _output.WriteLine("----------------------------------------------------------");
            }
        }

    }
}
