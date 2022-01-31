namespace Benchmarking.Core.Map
{
    public class RobotStep
    {
        public Point Location { get; }
        public int Visibility { get; }
        public int AddedVisibility { get; }

        public RobotStep(Point location, int visibility, int addedVisibility)
        {
            Location = location;
            Visibility = visibility;
            AddedVisibility = addedVisibility;
        }

    }
}
