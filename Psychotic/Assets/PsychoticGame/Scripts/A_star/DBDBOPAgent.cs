using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class DBDBOPAgent
{
    private Node[,] grid;
    private Node startNode;
    private Node targetNode;
    private DynBiDirBeamPath searchMaster;
    //private HashSet<Node> oppositeAgentSet;
    private ManualResetEvent doneEvent;
    private int nodesToExpand;
    private Heap<Node> openSet;

    public AgentResult agentResult;
    public HashSet<Node> closedSet;
    
    public enum PathFoundResponse{GOAL, OTHER_AGENT, NOT_FOUND, RETRY};

    public bool EventCompleted
    { get { return doneEvent.WaitOne(0); } }

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
    
    /*
    public void SetOppositeAgentHash(ref HashSet<Node> oppositeSet)
    {
        oppositeAgentSet = oppositeSet;
    }
    */

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
