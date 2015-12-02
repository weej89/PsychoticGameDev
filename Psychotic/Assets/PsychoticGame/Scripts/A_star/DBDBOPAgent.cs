using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class DBDBOPAgent
{
	#region Private Variables
    private Node[,] grid;
    private Node startNode;
    private Node targetNode;
    private DynBiDirBeamPath searchMaster;
    private ManualResetEvent doneEvent;
    private int nodesToExpand;
	#endregion

	#region Public Variables
    public Heap<Node> openSet;

    public AgentResult agentResult;
    public HashSet<Node> closedSet;
    
    public enum PathFoundResponse{GOAL, OTHER_AGENT, NOT_FOUND, RETRY};
	#endregion

	#region Public Fields
    public bool EventCompleted
    { get { return doneEvent.WaitOne(0); } }
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="DBDBOPAgent"/> class.
	/// </summary>
	/// <param name="_searchMaster">_search master.</param>
	/// <param name="_grid">_grid.</param>
	/// <param name="_startNode">_start node.</param>
	/// <param name="_targetNode">_target node.</param>
	/// <param name="_nodesToExpand">_nodes to expand.</param>
    public DBDBOPAgent(DynBiDirBeamPath _searchMaster, Node[,] _grid, Node _startNode, Node _targetNode, int _nodesToExpand)
    {
        searchMaster = _searchMaster;
        grid = _grid;
        startNode = _startNode;
        targetNode = _targetNode;
        nodesToExpand = _nodesToExpand;
        openSet = new Heap<Node>(grid.GetLength(0) * grid.GetLength(1));
        closedSet = new HashSet<Node>();
        agentResult = new AgentResult(false);
        doneEvent = new ManualResetEvent(false);
    }
	#endregion

	#region DoSearch
	/// <summary>
	/// Dos the search.
	/// </summary>
	/// <returns>A response object to what the search has found</returns>
	/// <param name="oppositeAgentSet">Opposite agent set.</param>
    public PathFoundResponse DoSearch(HashSet<Node> oppositeAgentSet)
    {
        agentResult.pathFound = false;
        int nodesExpanded = 0;

        openSet.Add(startNode);

        while (openSet.Count > 0 && nodesExpanded < nodesToExpand)
        {
            Node currentNode = openSet.RemoveFirst();
            closedSet.Add(currentNode);
            agentResult.lastNodeInSet = currentNode;
            nodesExpanded++;
            
            //If a complete path is found then return true with a reference to the targetNode
            if(currentNode == targetNode)
            {
                agentResult.pathFound = true;
                agentResult.lastNode = currentNode;
                return PathFoundResponse.GOAL;
            }

            foreach(Node neighbor in searchMaster.GetGridRef.GetNeighbors(grid, currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newMovementCostToNeighbor = currentNode.gCost + searchMaster.GetDistance(currentNode, neighbor);

                //If a node is found that is contained within the other agents search history then return true
                //with the last node being unchanged
                if(oppositeAgentSet.Contains(neighbor))
                {
                    agentResult.pathFound = true;
                    agentResult.lastNodeInSet = currentNode;
                    agentResult.lastNode = neighbor;
                    return PathFoundResponse.OTHER_AGENT;
                }

                neighbor.gCost = newMovementCostToNeighbor;
                neighbor.hCost = searchMaster.GetDistance(neighbor, targetNode);
                neighbor.parent = currentNode;

                if (!openSet.Contains(neighbor))
                    openSet.Add(neighbor);
            }
        }

        if (openSet.Count == 0)
            return PathFoundResponse.NOT_FOUND;
        else
            return PathFoundResponse.RETRY;
    }
}
#endregion

#region Class AgentResult
//Used for storing result of current search
public class AgentResult
{
    public bool pathFound;
    public Node lastNodeInSet;
    public Node lastNode;

    public AgentResult(bool _pathFound)
    {
        pathFound = _pathFound;
        lastNode = null;
        lastNodeInSet = null;
    }
}
#endregion
