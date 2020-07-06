using UnityEngine;
using System.Collections;
using LocalRepairAStar.Navigation;
using LocalRepairAStar.Algorithms;
using System.Collections.Generic;
using System.Linq;

public class AgentBehaviour : MonoBehaviour
{
    /**
     * <summary>
     * The node the agent is currently on
     * </summary>
     */
    private NavigationNode node;

    /**
     * <summary>
     * The immediate next node that the agent should be
     * moving toward.
     * </summary>
     */
    private NavigationNode nextNode;

    /**
     * <summary>
     * Pathfinder
     * </summary>
     */
    private AStar pathfinder;

    /**
     * <summary>
     * Move Coroutine
     * </summary>
     */
    private IEnumerator moveCoroutine;

    /**
     * <summary>
     * Navigation Game Object
     * </summary>
     */
    public GameObject navigationGameObject;

    /**
     * <summary>
     * Navigation
     * </summary>
     */
    private NavigationBehaviour navigation;

    /**
     * <summary>
     * Paths
     * </summary>
     */
    private List<NavigationNode> paths;

    /**
     * <summary>
     * Path index
     * </summary>
     */
    private int pathIndex = 0;

    /**
     * <summary>
     * Is agent moving
     * </summary>
     */
    private bool moving = false;

    /**
     * <summary>
     * Target node
     * </summary>
     */
    public Vector3 targetNode;

    private void Awake()
    {
        this.navigation = this.navigationGameObject.GetComponent<NavigationBehaviour>();
    }

    // Use this for initialization
    private void Start()
    {
        this.node = this.GetClosestNavigationNode();
        this.node.OccupiedBy = this;

        this.nextNode = null;

        // Find the closest node base on the current position.
        this.pathfinder = new AStar();

        // x: 0, z: 2
        //NavigationNode goal = this.navigation.Nodes[(int)this.targetNode.x][(int)this.targetNode.z];
        //this.MoveTo(goal);

        StartCoroutine(this.RandomMovement());
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log(this.GetAvailableNavigationNodes().Count);
        }
    }

    /**
     * <summary>
     * Randomly move the agent to new navigation node
     * </summary>
     *
     * <returns>
     * IEnumerator
     * </returns>
     */
    private IEnumerator RandomMovement ()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 6f));
            this.pathIndex = 0;
            this.moving = false;

            NavigationNode goal = this.GetRandomAvailableNavigationNode();
            this.paths = this.pathfinder.Search(this.node, goal);

            yield return StartCoroutine(this.Move(goal));

            yield return null;
        }
    }

    /**
     * <summary>
     * Get random available navigation node
     * </summary>
     *
     * <returns>
     * NavigationNode
     * </returns>
     */
    private NavigationNode GetRandomAvailableNavigationNode ()
    {
        List<NavigationNode> availableNodes = this.GetAvailableNavigationNodes();
        return availableNodes[Random.Range(0, availableNodes.Count - 1)];
    }

    /**
     * <summary>
     * Get available naviagation nodes
     * </summary>
     *
     * <returns>
     * List<NavigationNode>
     * </returns>
     */
    private List<NavigationNode> GetAvailableNavigationNodes ()
    {
        List<NavigationNode> noOccupies = new List<NavigationNode>();
        foreach (Dictionary<int, NavigationNode> group in this.navigation.Nodes.Values)
        {
            foreach (NavigationNode node in group.Values)
            {
                if (node.OccupiedBy == null)
                {
                    noOccupies.Add(node);
                }
            }
        }

        return noOccupies;
    }

    /**
     * <summary>
     * Move the agent to the goal
     * </summary>
     *
     * <returns>
     * void
     * </returns>
     */
    private void MoveTo (NavigationNode goal)
    {
        // Stop previous movement
        if (this.moving)
        {
            this.pathIndex = 0;
            this.moving = false;
            StopCoroutine(this.moveCoroutine);
        }

        this.paths = this.pathfinder.Search(this.node, goal);

        // Start coroutine
        this.moveCoroutine = this.Move(goal);
        StartCoroutine(this.moveCoroutine);
    }

    /**
     * <summary>
     * Move to
     * </summary>
     */
    private IEnumerator Move (NavigationNode goal)
    {
        float speed = 2f;

        bool checkNextPath = true;
        bool nextNodeReached = false;
        bool hasRotated = false;

        while (node.ID != goal.ID && this.pathIndex < this.paths.Count && this.paths.Count > 0)
        {
            NavigationNode nextNode = this.paths[this.pathIndex];
            Vector3 nextPoint = nextNode.Position;
            nextPoint.y = -6.7f;

            /**
             * -------------------
             * | Local Repair A* |
             * -------------------
             * Check next node is not occupied by other agent.
             * If it is, find new paths.
             * If no paths are returned, stop.
             */
            if (checkNextPath)
            {
                if (nextNode.OccupiedBy != null && nextNode.OccupiedBy.GetInstanceID() != this.GetInstanceID())
                {
                    yield return new WaitForSeconds(2f);

                    // Stop and find new 
                    this.paths = this.pathfinder.Search(this.node, goal);

                    if (
                        this.paths == null ||
                        this.paths.Count == 0
                    )
                    {
                        yield break;
                    }

                    // If next node is occupied but we can find a way around it.
                    this.pathIndex = 0;

                    nextNode = this.paths[this.pathIndex];
                    nextNode.OccupiedBy = this;

                    nextNodeReached = false;
                    checkNextPath = false;

                    yield return null;
                }
                // If next node is not occupied by other agent
                else
                {
                    nextNode.OccupiedBy = this;
                    nextNodeReached = false;
                    checkNextPath = false;
                }
            }

            // Rotate to next direction before moving
            // Rotate toward the point
            Vector3 targetDirection = nextPoint - this.transform.position;
            
            Vector3 direction = Vector3.RotateTowards(this.transform.forward, targetDirection, Time.deltaTime * speed, 0f);
            direction.y = 0;

            Quaternion newRotation = Quaternion.LookRotation(direction);
            hasRotated = Quaternion.Angle(this.transform.rotation, newRotation) <= 0f;

            if (!hasRotated)
            {
                this.transform.rotation = newRotation;
            }


            // Check if the agent has reached the destination
            // If it is, move onto to the next node
            if (hasRotated && Vector3.Distance(this.transform.position, nextPoint) < 0.001f)
            {
                // Clear previous node
                this.node.OccupiedBy = null;

                // Set new current node to the next node
                this.node = nextNode;
                this.node.OccupiedBy = this;

                nextNodeReached = true;
                checkNextPath = true;

                this.pathIndex++;
            }

            // Has the agent rotated completely and
            // if agent has not reached, keep moving.
            if (hasRotated && !nextNodeReached)
            {
                this.transform.position = Vector3.MoveTowards(
                    this.transform.position,
                    nextPoint,
                    Time.deltaTime * speed
                );

                this.moving = true;
            }

            yield return null;
        }
    }

    /**
     * <summary>
     * Get closest navigation point based on where the agent is
     * located
     * </summary>
     *
     * <returns>
     * NavigationNode
     * </returns>
     */
    private NavigationNode GetClosestNavigationNode ()
    {
        Vector3 position = this.transform.position;

        int x = Mathf.RoundToInt(position.x);
        int z = Mathf.RoundToInt(position.z);

        return (this.navigation.Nodes.ContainsKey(x) && this.navigation.Nodes[x].ContainsKey(z) ? this.navigation.Nodes[x][z] : null);
    }

    /**
     * <summary>
     * Get closest navigation point based on where the agent is
     * located
     * </summary>
     *
     * <param name="x"></param>
     * <param name="z"></param>
     * 
     * <returns>
     * NavigationNode
     * </returns>
     */
    private NavigationNode GetClosestNavigationNode(float x, float z)
    {
        Vector3 position = this.transform.position;

        int nx = Mathf.RoundToInt(x);
        int nz = Mathf.RoundToInt(z);

        return (this.navigation.Nodes.ContainsKey(nx) && this.navigation.Nodes[nx].ContainsKey(nz) ? this.navigation.Nodes[nx][nz] : null);
    }
}
