using System;
using System.Collections.Generic;

using LocalRepairAStar.DataStructures;
using LocalRepairAStar.Navigation;
using UnityEngine;

namespace LocalRepairAStar.Algorithms
{
    /**
	 * https://en.wikipedia.org/wiki/A*_search_algorithm
	 * http://theory.stanford.edu/~amitp/GameProgramming/Heuristics.html
	 */
    public class AStar
    {
        /**
         * <summary>
         * The result paths
         * </summary>
         */
        private List<NavigationNode> paths;

        public List<NavigationNode> Paths
        {
            set
            {
                this.paths = value;
            }

            get
            {
                return this.paths;
            }
        }

        /**
         * <summary>
         * A list of explored navigation nodes
         * </summary>
         */
        private List<NavigationNode> explored;

        public List<NavigationNode> Explored
        {
            set
            {
                this.explored = value;
            }

            get
            {
                return this.explored;
            }
        }

        /**
         * <summary>
         * Priority queue
         * </summary>
         */
        private BinaryMinHeap priorityQueue;

        public BinaryMinHeap PriorityQueue {
            set
            {
                this.priorityQueue = value;
            }

            get
            {
                return this.priorityQueue;
            }
        }

        public AStar()
        {

        }

        /**
         * <summary>
         * Search for the paths between the start node and the goal node
         * </summary>
         *
         * <param name="start"></param>
         * <param name="goal"></param>
         *
         * <returns>
         * List<NavigationNode>
         * </returns>
         */
        public List<NavigationNode> Search (NavigationNode start, NavigationNode goal)
        {
            this.PriorityQueue = new BinaryMinHeap();
            this.Explored = new List<NavigationNode>();
            this.Paths = new List<NavigationNode>();

            this.PriorityQueue.Insert(start);

            start.CameFrom = null;
            start.GScore = 0;
            start.FScore = this.Heuristic(start, goal);

            while (this.PriorityQueue.NotEmpty())
            {
                NavigationNode current = this.PriorityQueue.Extract();

                if (current.ID == goal.ID)
                {
                    return this.Paths = this.ReconstructPath(current);
                }

                for (int i = 0; i < current.Neighbours.Count; i++)
                {
                    NavigationNode neighbour = current.Neighbours[i];


                    if (
                        neighbour == null ||
                        neighbour.OccupiedBy != null
                    )
                    {
                        continue;
                    }

                    var tentativeGScore = current.GScore + this.MovementCost(current, neighbour);

                    if (tentativeGScore < neighbour.GScore)
                    {
                        neighbour.CameFrom = current;
                        neighbour.GScore = tentativeGScore;
                        neighbour.FScore = neighbour.GScore + this.Heuristic(neighbour, goal);

                        if (!neighbour.Explored)
                        {
                            neighbour.Explored = true;
                            this.Explored.Add(neighbour);

                            this.PriorityQueue.Insert(neighbour);
                        }
                    }
                }
                // Set the explored flag
                current.Explored = true;
                this.Explored.Add(current);
            }

            this.ResetFlags();

            return this.Paths;
        }

        /**
         * <summary>
         * Hueristic - Manhattan
         * Hueristic function is used to guide the A* exploration. The better hueristic
         * function the better the algorithm will perform.
         * </summary>
         *
         * <param name="current"></param>
         * <param name="next"></param>
         *
         * <returns>
         * float
         * </returns>
         */
        public float Heuristic (NavigationNode current, NavigationNode next)
        {
            float dx = Math.Abs(current.X - next.X);
            float dy = Math.Abs(current.Z - next.Z);

            return dx + dy;
        }

        /**
         * <summary>
         * The cost to move to next node.
         * For example, if the tile is a hill type.
         * The movement cost will be higher. The agent may instead move around the hill to get across.
         * </summary>
         *
         * <param name="current"></param>
         * <param name="next"></param>
         *
         * <returns>
         * float
         * </returns>
         */
        public float MovementCost (NavigationNode current, NavigationNode next)
        {
            return (float) Math.Sqrt(
                (next.X - current.X) * (next.X - current.X) +
                (next.Z - current.Z) * (next.Z - current.Z)
            );
        }

        /**
         * <summary>
         * Reconstruct the paths
         * </summary>
         *
         * <param name="current"></param>
         * 
         * <returns>
         * List<NavigationNode>
         * </returns>
         */
        private List<NavigationNode> ReconstructPath (NavigationNode current)
        {
            List<NavigationNode> paths = new List<NavigationNode>() { current };

            while (current.CameFrom != null)
            {
                current = current.CameFrom;
                paths.Insert(0, current);
            }

            this.ResetFlags();

            return paths;
        }

        /**
         * <summary>
         * Reset the flags so that the navigation will always use new flag values
         * </summary>
         *
         * <returns>
         * void
         * </returns>
         */
        private void ResetFlags()
        {
            foreach (NavigationNode exploredNode in this.Explored)
            {
                exploredNode.FScore = float.PositiveInfinity;
                exploredNode.GScore = float.PositiveInfinity;

                exploredNode.CameFrom = null;
                exploredNode.Explored = false;
                exploredNode.Obstructed = false;
            }
        }
    }
}