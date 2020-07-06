using UnityEngine;
using System.Collections;
using LocalRepairAStar.Navigation;

public class NavigationNodeBehaviour : MonoBehaviour
{
    /**
     * <summary>
     * Navigation node
     * </summary>
     */
    private NavigationNode node;

    public GameObject UI;

    public NavigationNode Node
    {
        set { this.node = value; }
        get { return this.node; }
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

}
