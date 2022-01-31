namespace Benchmarking.Core.Map
{
    public static class DefaultMaps
    {
        public static List<(int[,] map, Robot robot, Point goal, string mapName)> AllMaps(Random r)
        {
            var (fMap, fRobot, fGoal) = GetFiveBlockMap(r);
            var (bMap, bRobot, bGoal) = GetBugTrapMap(r);
            var (tMap, tRobot, tGoal) = GetTunnelMap(r);
            return new List<(int[,] map, Robot robot, Point goal, string mapName)>
            {
                (fMap, fRobot, fGoal, "five"),
                (bMap, bRobot, bGoal, "bug"),
                (tMap, tRobot, tGoal, "tunnel")
            };
        }

        public static Rectangle[] GetFiveObstacles()
        {
            return new[]
            {
                new Rectangle(60, 80, 120, 160),
                new Rectangle(60, 80, 60, 100),
                new Rectangle(60, 80, 0, 40),
                new Rectangle(120, 140, 10, 70),
                new Rectangle(120, 140, 90, 150)
            };
        }
        public static (int[,] map, Robot robot, Point goal) GetFiveBlockMap(Random r)
        {
            var obstacles = GetFiveObstacles();
            var goalSpawn = new Rectangle(10, 50, 10, 150);
            var robotSpawn = new Rectangle(150, 190, 10, 150);
            return GenerateMap(200, 160, obstacles, goalSpawn, robotSpawn, r);
        }

        public static (int[,] map, Robot robot, Point goal) GetBugTrapMap(Random r)
        {
            var obstacles = new[]
            {
                new Rectangle(50, 150, 50, 60),
                new Rectangle(50, 60, 50, 100),
                new Rectangle(140, 150, 50, 100),
                new Rectangle(50, 80, 90, 100),
                new Rectangle(120, 150, 90, 100),
            };
            var goalSpawn = new Rectangle(61, 139, 61, 89);
            var robotSpawn = new Rectangle(10, 190, 10, 40);
            return GenerateMap(200, 160, obstacles, goalSpawn, robotSpawn, r);
        }

        public static (int[,] map, Robot robot, Point goal) GetTunnelMap(Random r)
        {
            var obstacles = new[]
            {
                new Rectangle(50, 150, 0, 70),
                new Rectangle(50, 150, 90, 160),
                new Rectangle(50, 110, 70, 80),
                new Rectangle(120, 150, 80, 90)
            };
            var goalSpawn = new Rectangle(10, 40, 10, 150);
            var robotSpawn = new Rectangle(160, 190, 10, 150);
            return GenerateMap(200, 160, obstacles, goalSpawn, robotSpawn, r);
        }



        private static (int[,] map, Robot robot, Point goal) GenerateMap(int w, int h, Rectangle[] obstacles,
            Rectangle goalSpawn, Rectangle robotSpawn, Random r)
        {
            var map = MapExt.SetupMapWithBoundary(w, h);
            foreach (var obstacle in obstacles)
                map.FillMap(obstacle, MapExt.WallPoint);

            var goal = map.GetRandomLocation(goalSpawn, r);
            map.FillMap(goalSpawn, MapExt.GoalSpawn);
            var robot = new Robot(map.GetRandomLocation(robotSpawn, r), 30);
            map.FillMap(robotSpawn, MapExt.RobotSpawn);
            return (map, robot, goal);
        }

        private static void FillMap(this int[,] map, Rectangle r, int v)
        {
            for (var x = r.MinX; x < r.MaxX; x++)
            {
                for (var y = r.MinY; y < r.MaxY; y++)
                {
                    map[x, y] = v;
                }
            }
        }
    }
}
