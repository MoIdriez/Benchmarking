using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Navigation.Models
{
    public class RRTSettings
    {
        public int GrowthSize { get; }
        public int GoalDistance { get; }

        public RRTSettings(int growthSize, int goalDistance)
        {
            GrowthSize = growthSize;
            GoalDistance = goalDistance;
        }
    }
}
