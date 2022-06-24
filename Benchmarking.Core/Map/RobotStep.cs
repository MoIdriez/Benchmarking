namespace Benchmarking.Core.Map
{
    public class RobotStep
    {
        public Point Location { get; }
        public int Visibility { get; }
        
        public RobotStep(Point location, int visibility)
        {
            Location = location;
            Visibility = visibility;
        }
    }
}
