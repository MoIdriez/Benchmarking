using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Navigation.Models
{
    public class PheromoneSettings
    {
        public double Constant { get; }
        public double StrengthIncrease { get; }
        public int Range { get; }

        public PheromoneSettings(double constant, double strengthIncrease, int range)
        {
            Constant = constant;
            StrengthIncrease = strengthIncrease;
            Range = range;
        }

        public override string ToString()
        {
            return $"{Constant},{StrengthIncrease},{Range}";
        }
    }
}
