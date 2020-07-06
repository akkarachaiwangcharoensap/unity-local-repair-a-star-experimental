using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LocalRepairAStar.Navigation {
    public class NavigationNode
    {
        /**
         * <summary>
         * ID
         * </summary>
         */
        private int id;

        public int ID
        {
            set
            {
                this.id = value;
            }

            get
            {
                return this.id;
            }
        }

        /**
         * <summary>
         * X
         * </summary>
         */
        private int x;

        public int X
        {
            set
            {
                this.x = value;
            }

            get
            {
                return this.x;
            }
        }

        /**
         * <summary>
         * Z
         * </summary>
         */
        private int z;

        public int Z
        {
            set
            {
                this.z = value;
            }

            get
            {
                return this.z;
            }
        }

        /**
         * <summary>
         * World position
         * </summary>
         */
        private Vector3 position;

        public Vector3 Position
        {
            set
            {
                this.position = value;
            }

            get
            {
                return this.position;
            }
        }

        /**
         * <summary>
         * The distance between the node and exploring ndoe
         * </summary>
         */
        private float gScore;

        public float GScore
        {
            set
            {
                this.gScore = value;
            }

            get
            {
                return this.gScore;
            }
        }

        /**
         * <summary>
         * The distance between the node and the target node
         * </summary>
         */
        private float fScore;

        public float FScore
        {
            set
            {
                this.fScore = value;
            }

            get
            {
                return this.fScore;
            }
        }

        /**
         * <summary>
         * The node that it came from, this is used to retrieve
         * the final paths.
         * </summary>
         */
        private NavigationNode cameFrom;

        public NavigationNode CameFrom
        {
            set
            {
                this.cameFrom = value;
            }

            get
            {
                return this.cameFrom;
            }
        }

        /**
         * <summary>
         * Is node explored
         * </summary>
         */
        private bool explored;

        public bool Explored
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
         * Is the navigation node obstructed?
         * </summary>
         */
        private bool obstructed;

        public bool Obstructed
        {
            set
            {
                this.obstructed = value;
            }

            get
            {
                return this.obstructed;
            }
        }

        /**
         * <summary>
         * Neighbouring nodes
         * </summary>
         */
        private List<NavigationNode> neighbours;

        public List<NavigationNode> Neighbours
        {
            set
            {
                this.neighbours = value;
            }

            get
            {
                return this.neighbours;
            }
        }

        // Temporary for testing.
        private GameObject temporaryReference;

        public GameObject TemporaryReference
        {
            set { this.temporaryReference = value; }
            get { return this.temporaryReference; }
        }

        /**
         * <summary>
         * Occupied by which agent
         * </summary>
         */
        private AgentBehaviour occupiedBy;

        public AgentBehaviour OccupiedBy
        {
            set {
                this.occupiedBy = value;

                //if (this.occupiedBy == null)
                //{
                //    // White
                //    GameObject UI = this.TemporaryReference.GetComponent<NavigationNodeBehaviour>().UI;
                //    UI.GetComponent<Image>().color = Color.white;
                //}
                //else
                //{
                //    // Red
                //    GameObject UI = this.TemporaryReference.GetComponent<NavigationNodeBehaviour>().UI;
                //    UI.GetComponent<Image>().color = Color.red;
                //}
            }

            get { return this.occupiedBy; }
        }

        public NavigationNode()
        {
            this.ID = int.MinValue;
            this.X = int.MinValue;
            this.Z = int.MinValue;

            this.FScore = float.PositiveInfinity;
            this.GScore = float.PositiveInfinity;

            this.Position = Vector3.zero;

            this.CameFrom = null;
            this.Explored = false;
            this.Obstructed = false;

            //this.OccupiedBy = null;

            //this.TemporaryReference = null;
            this.Neighbours = new List<NavigationNode>();
        }

        /**
         * <summary>
         * Convert object into a string
         * </summary>
         *
         * <returns>
         * string
         * </returns>
         */
        public override string ToString()
        {
            return "x: " + this.X + " z: " + this.Z;
        }
    }
}