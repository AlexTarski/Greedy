using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Remote.Protocol.Designer;
using DynamicData;
using Greedy.Architecture;

namespace Greedy;

public class DijkstraPathFinder
{
    class DijkstraData
    {
        public Point? Previous { get; set; }
        public int Price { get; set; }
    }

    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start,
		IEnumerable<Point> targets)
	{
        HashSet<Point> unvisitedTargets = targets.ToHashSet();
        PriorityQueue<Point, int> pointsToVisit = new();
        pointsToVisit.Enqueue(start, 0);
        var track = new Dictionary<Point, DijkstraData>
        {
            [start] = new DijkstraData { Price = 0, Previous = null }
        };

        while (pointsToVisit.Count != 0)
        {
            Point toOpen = pointsToVisit.Dequeue();
            if (unvisitedTargets.Contains(toOpen))
            {
                unvisitedTargets.Remove(toOpen);
                yield return GetPathWithCost(toOpen, track);
            }

            List<Point> nearestPoints = GetAdjacentPoints(state, toOpen);

            foreach (var point in nearestPoints)
            {
                int currentPrice = track[toOpen].Price + state.CellCost[point.X, point.Y];
                if (!track.ContainsKey(point))
                {
                    track[point] = new DijkstraData { Previous = toOpen, Price = currentPrice };
                    pointsToVisit.Enqueue(point, track[point].Price);
                }
                else if (track[point].Price > currentPrice)
                {
                    track[point] = new DijkstraData { Previous = toOpen, Price = currentPrice };
                }
            }
        }
        yield break;
    }

    private static PathWithCost GetPathWithCost (Point target, Dictionary<Point, DijkstraData> track)
    {
        var result = new List<Point>();
        var cost = track[target].Price;
        Point? end = target;
        while (end != null)
        {
            result.Add((Point)end);
            end = track[(Point)end].Previous;
        }
        result.Reverse();
        return new PathWithCost(cost, result.ToArray());
    }

    private static List<Point> GetAdjacentPoints(State state, Point point)
    {
        List<Point> points = new ();

        Point left = new(point.X - 1, point.Y);
        Point right = new(point.X + 1, point.Y);
        Point top = new(point.X, point.Y - 1);
        Point bottom = new(point.X, point.Y + 1);

        if (state.InsideMap(left) && state.CellCost[left.X, left.Y] != 0)
            points.Add(left);
        if (state.InsideMap(right) && state.CellCost[right.X, right.Y] != 0)
            points.Add(right);
        if (state.InsideMap(top) && state.CellCost[top.X, top.Y] != 0)
            points.Add(top);
        if (state.InsideMap(bottom) && state.CellCost[bottom.X, bottom.Y] != 0)
            points.Add(bottom);

        return points;
    }
}