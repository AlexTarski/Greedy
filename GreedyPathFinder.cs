using System;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
	public List<Point> FindPathToCompleteGoal(State state)
	{
		if(state.Goal > state.Chests.Count)
        {
            return new List<Point>();
        }

        HashSet<Point> chests = new (state.Chests);
        List<Point> pathes = new();
        DijkstraPathFinder pathFinder = new();
		int currentEnergy = state.Energy;

        if (currentEnergy > 0)
        {
            PathWithCost currentPosition = pathFinder.GetPathsByDijkstra(state, state.Position, chests).FirstOrDefault();
            if (currentPosition == null)
            {
                return pathes;
            }
            MoveGreedy(state, pathes, chests, currentPosition, ref currentEnergy);
        }

		while(state.Goal != state.Scores)
        {
            if(chests.Count == 0 || currentEnergy <= 0)
            {
                break;
            }
            if(pathes.Count == 0)
            {
                MoveGreedy(state, pathes, chests, pathFinder.GetPathsByDijkstra(state, state.Position, chests).First(), ref currentEnergy);
            }
            MoveGreedy(state, pathes, chests, pathFinder.GetPathsByDijkstra(state, pathes[^1], chests).First(), ref currentEnergy);
        }
        return pathes;
	}

    private static void MoveGreedy(State state, List<Point> pathes, HashSet<Point> chests, PathWithCost currentPosition, ref int currentEnergy)
    {
        if (currentEnergy - currentPosition.Cost >= 0)
        {
            if (currentPosition.Cost == 0)
            {
                chests.Remove(currentPosition.Start);
                state.Scores += 1;
            }
            else
            {
                for (int i = 1; i < currentPosition.Path.Count; i++)
                {
                    pathes.Add(currentPosition.Path[i]);
                }
                chests.Remove(pathes[^1]);
                state.Scores += 1;
            }
        }
        currentEnergy -= currentPosition.Cost;
    }
}