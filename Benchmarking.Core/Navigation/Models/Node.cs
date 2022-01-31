using Benchmarking.Core.Map;

namespace Benchmarking.Core.Navigation.Models
{
    public class Node
    {
        public Node(Point p, Node? parent)
        {
            Parent = parent;
            Location = p;
            CostFromStart = parent == null ? 0 : parent.CostFromStart + parent.Location.DistanceTo(p);
        }

        public Node(Point p, Node? parent, double start, double end)
        {
            Parent = parent;
            Location = p;
            CostFromStart = start;
            Cost = start + end;
        }

        public override int GetHashCode()
        {
            return Location.GetHashCode();
        }

        public double Cost { get; set; }

        public double CostFromStart { get; set; }

        public void SetNewParent(Node parent)
        {
            Parent = parent;
            //var pts = GetAncestors();

            //var sum = 0.0;
            //for (var i = 0; i < pts.Count - 1; i++)
            //    sum += pts[i].DistanceTo(pts[i + 1]);

            CostFromStart = parent.CostFromStart + parent.Location.DistanceTo(Location);
        }

        public List<Point> GetAncestors()
        {
            var points = GetNextSteps(this).ToList();
            points.Reverse();
            return points;
        }
        
        public static IEnumerable<Point> GetNextSteps(Node successor)
        {
            yield return successor.Location;

            if (successor.Parent == null) yield break;
            foreach (var parent in GetNextSteps(successor.Parent))
                yield return parent;
        }

        public Node? Parent { get; private set; }
        public Point Location { get; }
    }
}
