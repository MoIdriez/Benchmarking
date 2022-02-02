using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchmarking.Helper
{
    public static class TestResult
    {
        public static string RRT => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\RRT\RRT-All.txt";
        public static string AStar1 => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\AStar\AStar-1.txt";
        public static string AStar2 => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\AStar\AStar-2.txt";
        public static string AStar3 => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\AStar\AStar-3.txt";
        public static string PotentialFieldInitial => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Initial.txt";
        public static string PotentialFieldExtensive => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Extensive.txt";
        public static string PotentialFieldExtensive2 => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\PotentialFieldParameter\PotentialField-Extensive-pt2.txt";
        public static string PheromonePotentialFieldExtensive => @"D:\Research\Repos\Benchmarking\Benchmarking\Results\PheromonePotentialFieldParameter\PheromonePotentialField-Extensive.txt";
        //tunnel,False,True,374,41,1000,1,1,1

        public static List<RRTResult> GetRRT()
        {
            return File.ReadAllLines(RRT).Select(l => new RRTResult(l)).ToList();
        }
        public static List<AStarResult> GetAStar()
        {
            var pt1 = File.ReadAllLines(AStar1);
            var pt2 = File.ReadAllLines(AStar2);
            var pt3 = File.ReadAllLines(AStar3);
            return pt1.Concat(pt2).Concat(pt3).Select(l => new AStarResult(l)).ToList();
        }

        public static List<PotentialFieldResult> GetPotentialFieldExtensive()
        {
            var pt1 = File.ReadAllLines(PotentialFieldExtensive);
            var pt2 = File.ReadAllLines(PotentialFieldExtensive2);
            return pt1.Concat(pt2).Select(l => new PotentialFieldResult(l)).ToList();
        }
        public static List<PheromoneResults> GetPheromonePotentialFieldExtensive()
        {
            var pt = File.ReadAllLines(PheromonePotentialFieldExtensive);
            return pt.Select(l => new PheromoneResults(l)).ToList();
        }
        public static List<PotentialFieldResult> Read(string filename)
        {
            return File.ReadAllLines(filename).Select(l => new PotentialFieldResult(l)).ToList();
        }
        public static List<PheromoneResults> PfRead(string filename)
        {
            return File.ReadAllLines(filename).Select(l => new PheromoneResults(l)).ToList();
        }

        public class PheromoneResults : PotentialFieldResult
        {
            public PheromoneResults(string line) : base(line)
            {
                var split = line.Split(',');
                Constant = double.Parse(split[9]);
                StrengthIncrease = double.Parse(split[10]);
                Range = int.Parse(split[11]);
            }
            public double Constant { get; }
            public double StrengthIncrease { get; }
            public int Range { get; }
            public override string Line => base.Line + $",{Constant},{StrengthIncrease},{Range}";
        }

        public class PotentialFieldResult : Result
        {
            public PotentialFieldResult(string line) : base(line)
            {
                var split = line.Split(',');
                ObstacleRange = int.Parse(split[6]);
                ObstacleConstant = int.Parse(split[7]);
                AttractiveConstant = int.Parse(split[8]);
            }
            public int ObstacleRange { get; set; }
            public int ObstacleConstant { get; set; }
            public int AttractiveConstant { get; set; }

            public override string Line => base.Line + $",{ObstacleRange},{ObstacleConstant},{AttractiveConstant}";
        }

        public class RRTResult
        {
            public RRTResult(string line)
            {
                var split = line.Split(',');
                Map = split[0] == "five" ? MapType.Five : split[0] == "bug" ? MapType.Bug : MapType.Tunnel;
                Success = bool.Parse(split[1]);
                IsStuck = bool.Parse(split[2]);
                Time = int.Parse(split[3]);
                Steps = int.Parse(split[4]);
                MaxIterations = int.Parse(split[5]);
                NewPlanCount = int.Parse(split[6]);
                GrowthSize = int.Parse(split[7]);
                GoalDistance = int.Parse(split[8]);
            }
            public MapType Map { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }

            public int Time { get; set; }
            public int Steps { get; set; }
            public int MaxIterations { get; set; }
            public int NewPlanCount { get; set; }
            public int GrowthSize { get; set; }
            public int GoalDistance { get; set; }
            public virtual string Line => $"{(int)Map}" +
                                          $",{(Success ? 1 : 0)}" +
                                          $",{Time},{Steps},{MaxIterations},{NewPlanCount},{GrowthSize},{GoalDistance}";
        }

        public class AStarResult
        {
            public AStarResult(string line)
            {
                var split = line.Split(',');
                Map = split[0] == "five" ? MapType.Five : split[0] == "bug" ? MapType.Bug : MapType.Tunnel;
                Success = bool.Parse(split[1]);
                Time = int.Parse(split[2]);
                Steps = int.Parse(split[3]);
                MaxIterations = int.Parse(split[4]);
                NewPlanCount = int.Parse(split[5]);
            }
            public MapType Map { get; set; }
            public bool Success { get; set; }
            public int Time { get; set; }
            public int Steps { get; set; }
            public int MaxIterations { get; set; }
            public int NewPlanCount { get; set; }
            public virtual string Line => $"{(int)Map}" +
                                          $",{(Success ? 1 : 0)}" +
                                          $",{Time},{Steps},{MaxIterations},{NewPlanCount}";
        }

        public class Result
        {
            public Result(string line)
            {
                var split = line.Split(',');
                Map = split[0] == "five" ? MapType.Five : split[0] == "bug" ? MapType.Bug : MapType.Tunnel;
                Success = bool.Parse(split[1]);
                IsStuck = bool.Parse(split[2]);
                //Success = int.Parse(split[1]) == 1;
                //IsStuck = int.Parse(split[2]) == 1;
                Time = int.Parse(split[3]);
                Steps = int.Parse(split[4]);
                MaxIterations = int.Parse(split[5]);
            }
            public MapType Map { get; set; }
            public bool Success { get; set; }
            public bool IsStuck { get; set; }
            public int Time { get; set; }
            public int Steps { get; set; }
            public int MaxIterations { get; set; }

            public virtual string Line => $"{(int)Map}" +
                                          $",{(Success ? 1 : 0)}" +
                                          $",{(IsStuck ? 1 : 0)}" +
                                          $",{Time},{Steps},{MaxIterations}";
        }

        public enum MapType { Five, Bug, Tunnel }
    }
}
