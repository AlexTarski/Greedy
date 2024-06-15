using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DynamicData;
using Greedy.Architecture;
using NUnit.Framework.Interfaces;

namespace Greedy
{
    public class Tree : IEnumerable<Tree>
    {
        public Tree Head;
        public Point? From;
        public Point To;
        public HashSet<Tree> Roots = new ();
        public int TotalEnergy;
        public List<Point> Path = new ();
        public HashSet<Point> VisitedChest;

        public Tree()
        {
            VisitedChest = new HashSet<Point>();
        }

        public Tree(Tree tree, Point chest)
        {
            Head = tree;
            VisitedChest = new HashSet<Point>(tree.VisitedChest) { chest };
        }

        public IEnumerator<Tree> GetEnumerator()
        {
            var stack = new Stack<Tree>();
            stack.Push(this);
            while (stack.Count != 0)
            {
                var current = stack.Pop();
                foreach (var currentRoot in current.Roots)
                {
                    stack.Push(currentRoot);
                    yield return currentRoot;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class NotGreedyPathFinder : IPathFinder
    {
        public List<Point> FindPathToCompleteGoal(State state)
        {
            var pathFinder = new DijkstraPathFinder();
            var tree = new Tree { To = state.Position };
            var startPaths = new Dictionary<(Point from, Point to), PathWithCost>();
            var chestsPaths = new Dictionary<(Point from, Point to), PathWithCost>();
            var bestPath = new List<Point>();
            var availableChests = FindAvailableChests(state, startPaths);

            FindPathsBetweenChests(state, pathFinder, chestsPaths, availableChests);

            foreach (var startPath in startPaths
                         .Where(w => w.Value.Cost <= state.Energy))
            {
                tree.Roots.Add(new Tree(tree, startPath.Key.to)
                {
                    From = startPath.Key.from,
                    To = startPath.Key.to,
                    TotalEnergy = startPath.Value.Cost,
                    Path = startPath.Value.Path
                });
            }

            var stack = new Stack<Tree>();
            foreach (var treeRoot in tree.Roots)
            {
                stack.Push(treeRoot);
                while (stack.Count != 0)
                {
                    var currentRoot = stack.Pop();

                    var nextChests = chestsPaths
                        .Where(c =>
                            c.Key.from == currentRoot.To
                            && !currentRoot.VisitedChest.Contains(c.Key.to)
                            && currentRoot.TotalEnergy + c.Value.Cost <= state.Energy)
                        .ToList();

                    foreach (var chest in nextChests)
                    {
                        var root = new Tree(currentRoot, chest.Key.to)
                        {
                            From = chest.Key.from,
                            To = chest.Key.to,
                            TotalEnergy = currentRoot.TotalEnergy + chest.Value.Cost,
                            Path = chest.Value.Path
                        };

                        if (root.VisitedChest.Count == availableChests.Count)
                        {
                            List<Point> resPath = GeneratePath(root, bestPath);
                            return resPath;
                        }

                        currentRoot.Roots.Add(root);
                        stack.Push(root);
                    }
                }
            }

            var bestTree = tree.OrderByDescending(t => t.VisitedChest.Count).FirstOrDefault();

            if (bestTree == null)
                return new List<Point>();

            List<Point> result = GeneratePath(bestTree, bestPath);

            return result;
        }

        private static List<Point> GeneratePath (Tree bestTree, List<Point> bestPath)
        {
            while (bestTree.Head != null)
            {
                bestTree.Path.Reverse();

                bestPath.AddRange(bestPath.Count == 0 ? bestTree.Path : bestTree.Path.Skip(1));

                bestTree = bestTree.Head;
            }
            bestPath.Reverse();
            return bestPath.Skip(1).ToList();
        }

        private static void FindPathsBetweenChests(State state, DijkstraPathFinder pathFinder, Dictionary<(Point from, Point to), PathWithCost> chestsPaths, List<Point> availableChests)
        {
            foreach(var availableChest in availableChests)
            {
                IEnumerable<PathWithCost> paths = pathFinder.GetPathsByDijkstra(state, availableChest, availableChests.ToArray());
                if (paths != null)
                {
                    foreach (var path in paths)
                    {
                        chestsPaths.Add((availableChest, path.End), path);
                    }
                }
            }
        }

        private List<Point> FindAvailableChests(State state, Dictionary<(Point from, Point to), PathWithCost> startPaths)
        {
            var availableChests = new List<Point>();
            var dijkstra = new DijkstraPathFinder();
            var pathsWithCost = dijkstra.GetPathsByDijkstra(state, state.Position, state.Chests);
            if (pathsWithCost != null)
            {
                foreach (var path in pathsWithCost)
                {
                    availableChests.Add(path.End);
                    startPaths.Add((state.Position, path.End), path);
                }
            }
            return availableChests;
        }
    }
}