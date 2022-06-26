namespace Benchmarking.Core.Map
{
    public class RobotStep
    {
        public Point Location { get; }
        public double Visibility { get; }
        
        public RobotStep(Point location, double visibility)
        {
            Location = location;
            Visibility = visibility;
        }
    }
}
