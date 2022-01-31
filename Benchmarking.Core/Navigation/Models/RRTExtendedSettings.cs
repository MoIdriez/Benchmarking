namespace Benchmarking.Core.Navigation.Models
{
    public class RRTExtendedSettings : RRTSettings
    {
        public int SearchSize { get; }

        public RRTExtendedSettings(int growthSize, int goalDistance, int searchSize) : base(growthSize, goalDistance)
        {
            SearchSize = searchSize;
        }
        public override string ToString()
        {
            return $"{GrowthSize},{GoalDistance},{SearchSize}";
        }
    }
}
