using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading.Tasks;
using Benchmarking.Core.Map;
using Point = Benchmarking.Core.Map.Point;

namespace Benchmarking.Helper
{
    internal static class Viewer
    {
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
                    if (map[x * scale, y * scale] == MapExt.WallPoint)
                        image.SetPixel(x, y, Color.Black);
                    if (map[x * scale, y * scale] == MapExt.DefaultValue)
                        image.SetPixel(x, y, Color.DimGray);
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
                grf.FillEllipse(new SolidBrush(Color.LightBlue), step.Location.X / scale, step.Location.Y / scale, circleSize / scale, circleSize / scale);
            }

            await Task.Run(() => image.Save(fileName, ImageFormat.Png));
            Process.Start("cmd", $"/c {fileName}");
        }
    }
}
