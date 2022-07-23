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
                var scores = (from Evaluate.ApproachType approach in Enum.GetValues(typeof(Evaluate.ApproachType)) select (approach, value: eval.Score(approach, 0.75, 0.75, 0.25))).ToList();
                foreach (var score in scores.OrderByDescending(s => s.value))
                {
                    _output.WriteLine($"{score.approach}: {score.value:F}" +
                                      $" | S:{(eval.CalculateSuccess(score.approach).Any() ? eval.CalculateSuccess(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | P:{(eval.Path(score.approach).Any() ? eval.Path(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | D:{(eval.Duration(score.approach).Any() ? eval.Duration(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | V:{(eval.Visibility(score.approach).Any() ? eval.Visibility(score.approach).Average(a => a.Value) : 0)}" +
                                      $" | M:{(eval.PathSmoothness(score.approach).Any() ? eval.PathSmoothness(score.approach).Average(a => a.Value) : 0)}");
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
