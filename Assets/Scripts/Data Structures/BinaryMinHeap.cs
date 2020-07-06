using System;
using System.Collections.Generic;

using LocalRepairAStar.Navigation;

namespace LocalRepairAStar.DataStructures
{
    public class BinaryMinHeap
    {
        /**
         * <summary>
         * Navigation nodes list
         * </summary>
         */
        private List<NavigationNode> queue;

        public List<NavigationNode> Queue
        {
            set
            {
                this.queue = value;
            }

            get
            {
                return this.queue;
            }
        }

        public BinaryMinHeap()
        {
            this.queue = new List<NavigationNode>();
        }

        /**
         * <summary>
         * Sort / rebuild the heap such that the
         * index is the lowest
         * </summary>
         *
         * <param name="index"></param>
         * <returns>
         * void
         * </returns>
         */
        private void MinHeapify(int index)
        {
            int left = index * 2 + 1;
            int right = index * 2 + 2;

            int smallest = index;

            if (left < this.Queue.Count && this.Queue[smallest].FScore > this.Queue[left].FScore)
            {
                smallest = left;
            }

            if (right < this.Queue.Count && this.Queue[smallest].FScore > this.Queue[right].FScore)
            {
                smallest = right;
            }

            if (smallest != index)
            {
                // swap
                NavigationNode temp = this.Queue[smallest];
                this.Queue[smallest] = this.Queue[index];
                this.Queue[index] = temp;

                this.MinHeapify(smallest);
            }
        }

        /**
         * <summary>
         * Insert a new node onto the queue list
         * </summary>
         *
         * <param name="node"></param>
         *
         * <returns>
         * void
         * </returns>
         */
        public void Insert(NavigationNode node)
        {
            this.Queue.Add(node);

            int index = this.Queue.Count - 1;
            int parent = (int)((index - 1) / 2);

            // Keep swapping parent and the new node until
            // the tree is properly sorted
            while (this.Queue[parent] != null && this.Queue[parent].FScore > this.Queue[index].FScore)
            {
                NavigationNode temp = this.Queue[index];
                this.Queue[index] = this.Queue[parent];
                this.Queue[parent] = temp;

                index = parent;
                parent = (int)((index - 1) / 2);
            }
        }

        /**
         * <summary>
         * Extract the smallest FScore
         * </summary>
         *
         * <returns>
         * NavigationNode
         * </returns>
         */
        public NavigationNode Extract()
        {
            if (this.Queue.Count < 1)
            {
                return null;
            }

            NavigationNode smallest = this.Queue[0];

            // Remove the node at index 0 from the list
            this.Queue.RemoveAt(0);

            // Rebuild the heap at index 0
            this.MinHeapify(0);

            return smallest;
        }

        /**
         * <summary>
         * Is the queue empty?
         * </summary>
         *
         * <returns>
         * bool
         * </returns>
         */
        public bool NotEmpty()
        {
            return this.Queue.Count > 0;
        }
    }
}