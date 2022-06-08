namespace Benchmarking.Core.Generation
{
    public class Room
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;

        public Direction CorridorDirection;

        public Corridor c;

        public Room(IntRange roomWidth, IntRange roomHeight, int worldWidth, int worldHeight, Random random)
        {
            Width = roomWidth.Random(random);
            Height = roomHeight.Random(random);

            X = Convert.ToInt32(Math.Round((double)(worldWidth / 2 - Width)));
            Y = Convert.ToInt32(Math.Round((double)(worldHeight / 2 - Height)));
        }

        public Room(IntRange roomWidth, IntRange roomHeight, Corridor corridor, Random random)
        {
            c = corridor;
            Width = roomWidth.Random(random);
            Height = roomHeight.Random(random);
            CorridorDirection = corridor.Direction;

            switch (CorridorDirection)
            {
                case Direction.North:
                    X = corridor.X - Width;
                    Y = corridor.Y;
                    break;
                case Direction.South:
                    X = corridor.X;
                    Y = corridor.Y + corridor.Length;
                    break;
                case Direction.West:
                    X = corridor.X - Width;
                    Y = corridor.Y;
                    break;
                case Direction.East:
                    X = corridor.X + corridor.Length;
                    Y = corridor.Y - Height + corridor.Width;
                    break;
            }
        }
    }
}
