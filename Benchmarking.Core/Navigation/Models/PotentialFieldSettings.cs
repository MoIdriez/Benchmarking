namespace Benchmarking.Core.Navigation.Models
{
    public class PotentialFieldSettings
    {
        public int ObstacleRange { get; }
        public double ObstacleConstant { get; }
        public double AttractiveConstant { get; }

        public PotentialFieldSettings(int obstacleRange, double obstacleConstant, double attractiveConstant)
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
