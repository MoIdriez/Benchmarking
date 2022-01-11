using System;
using System.Diagnostics;
using System.IO;
using Xunit.Abstractions;

namespace Benchmarking.Helper
{
    public class StateNotifier
    {
        private readonly ITestOutputHelper _output;
        private readonly string _fileName;
        private double _max;
        private double _notifyStep;

        private double _count;
        private readonly object _lck = new ();
        private readonly Stopwatch _sw = new();

        public StateNotifier(ITestOutputHelper output, string fileName)
        {
            _output = output;
            _fileName = fileName;
        }

        public void Run(double max, double notifyStep = 1)
        {
            _max = max;
            _notifyStep = notifyStep;
            _sw.Start();
        }

        public void Result()
        {
            _sw.Stop();
            _output.WriteLine($"================================== {TimeSpan.FromMilliseconds(_sw.ElapsedMilliseconds):g}");
        }

        public void NotifyCompletion(string[] content)
        {
            lock (_lck)
            {
                _count++;
                if (_count % _notifyStep == 0)
                {
                    _output.WriteLine($"{_count}/{_max} ({_count / _max * 100} %) - {TimeSpan.FromMilliseconds(_sw.ElapsedMilliseconds):g}");
                }
                File.AppendAllLines(_fileName, content);
            }
        }

    }
}
