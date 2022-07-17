﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Core.Map
{
    public static class MathExt
    {
        public static int ToInt(this double v)
        {
            return Convert.ToInt32(v);
        }
        public static int Round(this double v)
        {
            return (int)Math.Round(v);
        }
        public static double ToRad(this double degree)
        {
            return degree * Math.PI / 180;
        }

        public static double ToDeg(this double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static Vector DirectionTo(this Point start, Point end)
        {
            return new Vector(end.X - start.X, end.Y - start.Y);
        }

        public static double DistanceTo(this Point p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static double GetAngleBetween(this Point p1, Point p2)
        {
            return Math.Atan2(p2.Y - p1.Y, p2.X - p1.X).ToDeg();
        }

        public static double GetPathSmoothness(this LineSegment[] segments)
        {
            var sum = 0.0;
            for (var i = 0; i < segments.Length - 1; i++)
            {
                sum += Math.Pow(segments[i].GetAngleBetween(segments[i + 1]), 2);
            }

            return 1 / (double)segments.Length * sum;
        }

        public static IEnumerable<LineSegment> ToSegments(this Point[] points)
        {
            for (var i = 0; i < points.Length - 1; i++)
            {
                yield return new LineSegment(points[i], points[i + 1]);
            }
        }

        public static Point GetEndPoint(this Point p, double angle, double length)
        {
            return new Point(
                (p.X + length * Math.Cos(angle.ToRad())).ToInt(),
                (p.Y + length * Math.Sin(angle.ToRad())).ToInt()
            );
        }

        public static double Percentage(this long v, long m)
        {
            return v / m;
        }

        public static double Percentage(this double v, double m)
        {
            return v / m;
        }

        public static double Percentage(this int v, int m)
        {
            return (double)v / m;
        }

        public static double DiagonalOfRectangle(int w, int h)
        {
            return Math.Sqrt(Math.Pow(w, 2) + Math.Pow(h, 2));
        }

        public static double PercentageFromRange(this double i, double mn, double mx)
        {
            return (i - mn) / (mx - mn);
        }

        public static double KeepRange(this double i, int min, int max)
        {
            return Math.Max(min, Math.Min(i, max));
        }

        public static double StandardDeviation(this IEnumerable<int> values)
        {
            var avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static double StandardDeviation(this IEnumerable<double> values)
        {
            var avg = values.Average();
            return Math.Sqrt(values.Average(v => Math.Pow(v - avg, 2)));
        }

        public static double Percentile(this IEnumerable<int> sequence, double excelPercentile)
        {
            return sequence.Select(s => (double) s).ToArray().Percentile(excelPercentile);
        }

        public static double Percentile(this IEnumerable<double> seq, double excelPercentile)
        {
            var sequence = seq.ToArray();
            return sequence.Percentile(excelPercentile);
        }

        public static double Percentile(this double[] sequence, double excelPercentile)
        {
            Array.Sort(sequence);
            int N = sequence.Length;
            double n = (N - 1) * excelPercentile + 1;
            // Another method: double n = (N + 1) * excelPercentile;
            if (n == 1d) return sequence[0];
            else if (n == N) return sequence[N - 1];
            else
            {
                int k = (int)n;
                double d = n - k;
                return sequence[k - 1] + d * (sequence[k] - sequence[k - 1]);
            }
        }
    }
}
