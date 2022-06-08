namespace Benchmarking.Core.Generation
{
    public enum Direction
    {
        North, East, South, West,
    }

    public class Corridor
    {
        public int X;
        public int Y;
        public int Width;
        public int Length;
        public Direction Direction;

        public Corridor(Room room, IntRange length, IntRange width, Direction direction, Random random)
        {
            Width = width.Random(random);
            Direction = direction;
            Length = length.Random(random);

            switch (Direction)
            {
                case Direction.North:
                    X = new IntRange(room.X, room.X + room.Width - Width).Random(random);
                    Y = room.Y - Length;
                    break;
                case Direction.West:
                    X = room.X - Length;
                    Y = new IntRange(room.Y, room.Y + room.Height - Width).Random(random);
                    break;
                case Direction.South:
                    X = new IntRange(room.X, room.X + room.Width - Width).Random(random);
                    Y = room.Y + room.Height;
                    break;
                case Direction.East:
                    X = room.X + room.Width;
                    Y = new IntRange(room.Y, room.Y + room.Height - Width).Random(random);
                    break;
            }

        }
    }
}
