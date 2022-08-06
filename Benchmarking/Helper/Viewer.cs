using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Point = Benchmarking.Core.Map.Point;
using Rectangle = Benchmarking.Core.Map.Rectangle;

namespace Benchmarking.Helper
{
    internal static class Viewer
    {
        public static void ChapterOne(int[,] map, Robot robot, Point goal)
        {
            var image = new Bitmap(map.Width(), map.Height());
            var grf = Graphics.FromImage(image);
            for (var x = 0; x < map.Width(); x++)
            {
                for (var y = 0; y < map.Height(); y++)
                {
                    if (map[x , y] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    if (map[x, y] == MapExt.DefaultValue)
                        image.SetPixel(x, y, Color.White);
                }
            }

            grf.FillEllipse(new SolidBrush(Color.LightGreen), goal.X, goal.Y, 5, 5);
            grf.FillEllipse(new SolidBrush(Color.LightBlue), robot.Location.X, robot.Location.Y, 5, 5);
            image.Save($"Images/ChapterOne-{Guid.NewGuid()}.png", ImageFormat.Png);
        }


        public static async Task Image(int[,] map, Robot robot, Point goal)
        {
            await Image($"Images/{Guid.NewGuid()}.png", map, robot, goal);
        }
        public static async Task Image(string fileName, int[,] map, Robot robot, Point goal, int scale = 1)
        {
            var image = new Bitmap(map.Width() / scale, map.Height() / scale);
            var grf = Graphics.FromImage(image);
            for (var x = 0; x < map.Width() / scale; x++)
            {
                for (var y = 0; y < map.Height() / scale; y++)
                {
                    if (map[x, y] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    if (map[x, y] == MapExt.DefaultValue)
                        image.SetPixel(x, y, Color.White);
                    //else if (map[x * scale, y * scale] == MapExt.RobotSpawn)
                    //    image.SetPixel(x, y, Color.Blue);
                    //else if (map[x * scale, y * scale] == MapExt.GoalSpawn)
                    //    image.SetPixel(x, y, Color.Green);

                }
            }
            var circleSize = 2;
            grf.FillEllipse(new SolidBrush(Color.LightGreen), goal.X / scale, goal.Y / scale, 5 / scale, 5 / scale);

            foreach (var step in robot.Steps)
            {
                image.SetPixel(step.Location.X, step.Location.Y, Color.LightBlue);
                //grf.FillEllipse(new SolidBrush(Color.LightBlue), step.Location.X / scale, step.Location.Y / scale, circleSize / scale, circleSize / scale);
            }

            await Task.Run(() => image.Save(fileName, ImageFormat.Png));
            Process.Start("cmd", $"/c {fileName}");
        }

        public static async Task Show(int[,] map, Rectangle? robot, Rectangle? goal)
        {
            var image = new Bitmap(map.Width(), map.Height());
            var grf = Graphics.FromImage(image);
            for (var x = 0; x < map.Width(); x++)
            {
                for (var y = 0; y < map.Height(); y++)
                {
                    if (map[x, y] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    else if (map[x, y] == MapExt.RobotSpawn)
                        image.SetPixel(x, y, Color.LightBlue);
                    else if (map[x, y] == MapExt.GoalSpawn)
                        image.SetPixel(x, y, Color.LightGreen);
                    else
                        image.SetPixel(x, y, Color.LightYellow);
                }
            }

            //if (robot != default)
            //{
            //    for (var x = robot.MinX; x <= robot.MaxX; x++)
            //    {
            //        for (var y = robot.MinY; y <= robot.MaxY; y++)
            //        {
            //            image.SetPixel(x, y, Color.LightBlue);
            //        }
            //    }
            //}

            //if (goal != default)
            //{
            //    for (var x = goal.MinX; x <= goal.MaxX; x++)
            //    {
            //        for (var y = goal.MinY; y <= goal.MaxY; y++)
            //        {
            //            image.SetPixel(x, y, Color.LightGreen);
            //        }
            //    }
            //}
            
            var fileName = "test.png";
            await Task.Run(() => image.Save(fileName, ImageFormat.Png));
            Process.Start("cmd", $"/c {fileName}");
        }
    }
}
