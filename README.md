# Greedy

This project illustrates the workings of the Dijkstra, Greedy and Not-Greedy algorithms.

We have a maze filled with cells.
Cells could be walls (you cannot go through them), chests (you should collect them),
or empty (however, every empty cell has a difficulty rate).

Your hero has energy. Every time he pass through the empty cell, his energy drains according
to the cell difficulty rate.
Goal of your hero: collect as many chests as possible.
He will do that automatically in accordance with the implemented algorithm (Greedy or Not-Greedy).

GetPathsByDijkstra() method in DijkstraPathFinder class looks for all available paths from starting point to chests.
It takes into account the difficulty of every path and returns them from the easiest one to the most difficult,
working as a lazy method.

FindPathToCompleteGoal() method in the GreedyPathFinder class returns the path of your hero.
It takes the easiest of the available chests that your hero could reach from his current position,
then moves your hero to that cell, repeats the search one more time from the new position, and repeats it
until there are no available paths, or your hero doesn't reach the Goal, or there is no more energy.
After that, it connects all routes into the final path.
That method looks for paths using the GetPathsByDijkstra() method.
Also, it takes into account the current energy level of your hero.
If the method can't find any path, it returns an empty path as the result.

FindPathToCompleteGoal() method in the NotGreedyPathFinder class is doing similar things, with one exception:
it looks for the best possible path that can help your hero collect as many chests as possible.
That method illustrates that using of the greedy algorithm could be inaccurate in some situations.






