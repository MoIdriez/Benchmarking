namespace Benchmarking.Core.Generation
{
    public class IntRange
    {
        public int Min;
        public int Max;

        public IntRange(int min, int max)
        {
            Min = min;
            Max = max;
        }

        public int Random(Random r)
        {
            return r.Next(Min, Max);
        }

        public override string ToString()
        {
            return $"IntRange: {Min}->{Max}";
        }
    }
}
