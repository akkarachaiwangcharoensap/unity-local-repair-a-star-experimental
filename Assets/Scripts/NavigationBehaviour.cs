using UnityEngine;
using System.Collections;

using LocalRepairAStar.Algorithms;
using LocalRepairAStar.Navigation;
using System.Collections.Generic;
using System.Linq;

public class NavigationBehaviour : MonoBehaviour
{
    /**
     * <summary>
     * Ground game object
     * </summary>
     */
    public GameObject Ground;

    /**
     * <summary>
     * Navigation node dictionary
     * </summary>
     */
    private Dictionary<int, Dictionary<int, NavigationNode>> nodes;

    public Dictionary<int, Dictionary<int, NavigationNode>> Nodes
    {
        set
        {
            this.nodes = value;
        }

        get
        {
            return this.nodes;
        }
    }

    /**
     * <summary>
     * Temporary obstructed positions
     * </summary>
     */
    private List<Vector3> obstructs;

    public List<Vector3> Obstructs
    {
        set
        {
            this.obstructs = value;
        }

        get
        {
            return this.obstructs;
        }
    }

    private void Awake()
    {
        this.Obstructs = new List<Vector3>();

        this.BuildNavigation();
    }

    // Use this for initialization
    private void Start()
    {
        //AStar pathfinder = new AStar();

        //NavigationNode start = this.nodes[-6][-6];
        //NavigationNode end = this.nodes[-6][6];

        //List<NavigationNode> paths = pathfinder.Search(start, end);
        //this.OutputPaths(paths);

        //this.OutputNavigationNodes();
    }

    // Update is called once per frame
    private void Update()
    {

    }

    private void OutputPaths (List<NavigationNode> paths)
    {
        foreach (NavigationNode node in paths)
        {
            Debug.Log(node.ToString());
        }
    }

    /**
     * <summary>
     * Print navigation nodes
     * </summary>
     *
     * <returns>
     * void
     * </returns>
     */
    private void OutputNavigationNodes ()
    {
        foreach (Dictionary<int, NavigationNode> group in this.nodes.Values)
        {
            foreach (NavigationNode node in group.Values)
            {
                Debug.Log(node);
            }
        }
    }

    /**
     * <summary>
     * Build navigation nodes
     * </summary>
     *
     * <returns>
     * void
     * </returns>
     */
    public void BuildNavigation ()
    {
        Bounds bounds = this.Ground.GetComponent<Renderer>().bounds;
        Vector3 bottomLeft = new Vector3(bounds.min.x, bounds.max.y, bounds.min.z);
        Vector3 topRight = new Vector3(bounds.max.x, bounds.max.y, bounds.max.z);

        // Initial new nodes dictionary
        this.nodes = new Dictionary<int, Dictionary<int, NavigationNode>>();

        Debug.Log("Building Navigation");

        int id = 0;

        // Map the mesh
        for (int x = Mathf.RoundToInt(bottomLeft.x); x < Mathf.RoundToInt(topRight.x); x++)
        {
            if (!this.nodes.ContainsKey(x))
            {
                this.nodes[x] = new Dictionary<int, NavigationNode>();
            }

            for (int z = Mathf.RoundToInt(bottomLeft.z); z < Mathf.RoundToInt(topRight.z); z++)
            {
                Vector3 rayPosition = new Vector3(x, bounds.max.y + 1f, z);
                RaycastHit[] hits = Physics.RaycastAll(rayPosition, Vector3.down * Mathf.Abs(bounds.max.y));

                foreach (RaycastHit hit in hits)
                {
                    // Create navigation point
                    if (hit.transform.tag != "Navigation")
                    {
                        this.Obstructs.Add(new Vector3(x, 0, z));
                        continue;
                    }

                    Vector3 navigationPoint = hit.point;

                    GameObject newNavigationNode = MonoBehaviour.Instantiate(
                        Resources.Load("Prefabs/Navigations/Navigation Node") as GameObject
                    );

                    newNavigationNode.transform.position = navigationPoint;

                    // Build the navigation node
                    NavigationNode navigationNode = new NavigationNode();
                    navigationNode.ID = id;
                    navigationNode.X = x;
                    navigationNode.Z = z;

                    navigationNode.Position = navigationPoint;
                    //navigationNode.TemporaryReference = newNavigationNode;

                    this.nodes[x][z] = navigationNode;

                    id++;
                }
            }
        }

        // Build and set neighbouring nodes
        this.BuildNeighbours();

        // Set obstructions
        this.SetObstructions();
    }

    /**
     * <summary>
     * Set obstruction
     * </summary>
     *
     * <returns>
     * void
     * </returns>
     */
    private void SetObstructions ()
    {
        foreach (Vector3 obstruct in this.Obstructs)
        {
            int x = (int)obstruct.x;
            int z = (int)obstruct.z;

            this.nodes[x][z].Obstructed = true;
        }
    }

    /**
     * <summary>
     * Build neighbours
     * </summary>
     *
     * <returns>
     * void
     * </returns>
     */
    private void BuildNeighbours ()
    {
        foreach (Dictionary<int, NavigationNode> group in this.nodes.Values)
        {
            foreach (NavigationNode node in group.Values)
            {

                NavigationNode topLeft = null, top = null, topRight = null,
                    left = null, right = null,
                    bottomLeft = null, bottom = null, bottomRight = null;

                if (this.nodes.ContainsKey(node.X - 1))
                {
                    topLeft = (this.nodes[node.X - 1].ContainsKey(node.Z + 1)) ? this.nodes[node.X - 1][node.Z + 1] : null;
                    bottomLeft = (this.nodes[node.X - 1].ContainsKey(node.Z - 1)) ? this.nodes[node.X - 1][node.Z - 1] : null;
                    left = (this.nodes[node.X - 1].ContainsKey(node.Z)) ? this.nodes[node.X - 1][node.Z] : null;
                }

                if (this.nodes.ContainsKey(node.X + 1))
                {
                    topRight = (this.nodes[node.X + 1].ContainsKey(node.Z + 1)) ? this.nodes[node.X + 1][node.Z + 1] : null;
                    bottomRight = (this.nodes[node.X + 1].ContainsKey(node.Z - 1)) ? this.nodes[node.X + 1][node.Z - 1] : null;
                    right = (this.nodes[node.X + 1].ContainsKey(node.Z)) ? this.nodes[node.X + 1][node.Z] : null;
                }

                top = (this.nodes[node.X].ContainsKey(node.Z + 1)) ? this.nodes[node.X][node.Z + 1] : null;
                bottom = (this.nodes[node.X].ContainsKey(node.Z - 1)) ? this.nodes[node.X][node.Z - 1] : null;

                node.Neighbours = new List<NavigationNode>()
                {
                    topLeft, top, topRight,
                    left, right,
                    bottomLeft, bottomRight
                };
            }
        }
    }
}
