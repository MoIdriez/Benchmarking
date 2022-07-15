using System.Data;
using Benchmarking.Core.Generation;

namespace Benchmarking.Core.Map
{
    public class MapGenerator
    {
        private readonly int _width;
        private readonly int _height;

        public bool HasObstacles;
        public bool HasDungeon;

        // obstacles related
        public IntRange ObstacleNumbers { get; set; }
        public IntRange ObstacleSizes { get; set; }

        // dungeon related
        public IntRange RoomNumbers { get; set; }
        public IntRange RoomWidth { get; set; }
        public IntRange RoomHeight { get; set; }

        public IntRange CorridorLength { get; set; }
        public IntRange CorridorWidth { get; set; }

        public MapGenerator(int width, int height, IntRange obstacles, IntRange sizes)
        {
            _width = width;
            _height = height;
            HasObstacles = true;
            ObstacleNumbers = obstacles;
            ObstacleSizes = sizes;
        }

        public MapGenerator(int width, int height, IntRange numberOfRooms, IntRange roomWidth, IntRange roomHeight,
            IntRange corridorLength, IntRange corridorWidth)
        {
            _width = width;
            _height = height;
            HasDungeon = true;
            RoomNumbers = numberOfRooms;
            RoomHeight = roomHeight;
            RoomWidth = roomWidth;

            CorridorLength = corridorLength;
            CorridorWidth = corridorWidth;
        }

        public int[,] GetInstance()
        {
            var random = new Random();
            var map = HasDungeon ? new int[_width, _height] : MapExt.SetupMapWithBoundary(_width, _height);

            if (HasDungeon)
            {
                GenerateDungeon(map, random);
                map.InverseMap();
            }

            if (HasObstacles)
            {
                GenerateObstacles(map, random);
            }

            return map;
        }

        private void GenerateDungeon(int[,] map, Random random)
        {
            var room = new Room(RoomWidth, RoomHeight, _width, _height, random);
            DrawRoom(map, room);

            var rooms = RoomNumbers.Random(random);

            for (var i = 0; i < rooms; i++)
            {
                Direction direction;

                var list = new List<Direction>();
                do
                {
                    if (list.Count == 4)
                        throw new InvalidConstraintException("Unable to find a valid direction to move towards");

                    direction = (Direction)random.Next(0, 4);

                    if (!list.Contains(direction))
                        list.Add(direction);

                } while (!DirectionAllowed(room, direction));

                var c = new Corridor(room, CorridorLength, CorridorWidth, direction, random);
                room = new Room(RoomWidth, RoomHeight, c, random);

                DrawCorridor(map, c);
                DrawRoom(map, room);
            }
        }

        private void GenerateObstacles(int[,] map, Random random)
        {
            var obstacles = ObstacleNumbers.Random(random);

            for (var i = 0; i < obstacles; i++)
            {
                var size = ObstacleSizes.Random(random);

                var point = GetEmptyLocation(map, random, LocationType.General);

                var points = point.GetSurround(_width, _height, true, size);

                points.ForEach(p => map[p.X, p.Y] = MapExt.WallPoint);
            }
        }

        private void DrawRoom(int[,] map, Room room)
        {
            for (var x = room.X; x < room.X + room.Width; x++)
            {
                for (var y = room.Y; y < room.Y + room.Height; y++)
                {
                    map[x, y] = MapExt.WallPoint;
                }
            }
        }

        private void DrawCorridor(int[,] map, Corridor corridor)
        {
            if (corridor.Direction == Direction.North || corridor.Direction == Direction.South)
            {
                for (var x = corridor.X; x < corridor.X + corridor.Width; x++)
                {
                    for (var y = corridor.Y; y < corridor.Y + corridor.Length; y++)
                    {
                        map[x, y] = MapExt.WallPoint;
                    }
                }
            }
            else
            {
                for (var x = corridor.X; x < corridor.X + corridor.Length; x++)
                {
                    for (var y = corridor.Y; y < corridor.Y + corridor.Width; y++)
                    {
                        map[x, y] = MapExt.WallPoint;
                    }
                }
            }
        }



        private bool DirectionAllowed(Room room, Direction direction)
        {
            switch (direction)
            {
                case Direction.North:
                    return CheckNorth(room) && CheckWest(room);
                case Direction.West:
                    return CheckWest(room) && CheckSouth(room);
                case Direction.South:
                    return CheckSouth(room) && CheckEast(room);
                default:
                    return CheckEast(room) && CheckNorth(room);
            }
        }
        private bool CheckNorth(Room room)
        {
            return room.Y - CorridorLength.Max - RoomHeight.Max > 0;
        }

        private bool CheckWest(Room room)
        {
            return room.X - CorridorLength.Max - RoomWidth.Max > 0;
        }

        private bool CheckSouth(Room room)
        {
            return room.Y + room.Height + CorridorLength.Max + RoomHeight.Max < _height;
        }

        private bool CheckEast(Room room)
        {
            return room.X + room.Width + CorridorLength.Max + RoomWidth.Max < _width;
        }

        public enum LocationType { Robot, Goal, General }
        public static Point GetEmptyLocation(int[,] map, Random random, LocationType locationType)
        {
            Point location = null;
            var i = 0;
            do
            {
                i++;
                if (i > 360000)
                    return new Point(0, 0);

                int x;
                int y;
                switch (locationType)
                {
                    case LocationType.Robot:
                        x = random.Next(0, map.Width() / 2);
                        y = random.Next(0, map.Height() / 2);
                        break;
                    case LocationType.Goal:
                        x = random.Next(map.Width() / 2, map.Width());
                        y = random.Next(map.Height() / 2, map.Height());
                        break;
                    case LocationType.General:
                        x = random.Next(0, map.Width());
                        y = random.Next(0, map.Height());
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(locationType), locationType, null);
                }

                if (map[x, y] < MapExt.WallPoint)
                    location = new Point(x, y);

            } while (location == null);

            return location;
        }
    }
}
