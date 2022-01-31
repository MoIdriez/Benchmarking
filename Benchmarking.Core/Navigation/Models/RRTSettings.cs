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

        public override string ToString()
        {
            return $"{GrowthSize},{GoalDistance}";
        }
    }
}
