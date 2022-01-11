namespace Benchmarking.Core.Navigation.Models
{
    public class PfSettings
    {
        public int ObstacleRange { get; }
        public double ObstacleConstant { get; }
        public double AttractiveConstant { get; }

        public PfSettings(int obstacleRange, double obstacleConstant, double attractiveConstant)
        {
            ObstacleRange = obstacleRange;
            ObstacleConstant = obstacleConstant;
            AttractiveConstant = attractiveConstant;
        }

        public override string ToString()
        {
            return $"{ObstacleRange},{ObstacleConstant},{AttractiveConstant}";
        }
    }
}
