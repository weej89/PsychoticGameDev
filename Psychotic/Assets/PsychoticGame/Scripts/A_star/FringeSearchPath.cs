using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FringeSearchPath : GridPath
{


	public FringeSearchPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)
	{

	}

	public override void FindPath()
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		try
		{
			if(startNode.walkable && targetNode.walkable)
			{
                stopWatch.Start();
				HashSet<Node> cache = new HashSet<Node>();
                List<Node> nextList = new List<Node>();
				List<Node> currList = new List<Node>();
                cache.Add(startNode);

				int threshold = GetDistance(startNode, targetNode);

				while(!path.pathSuccess && (currList.Count > 0 || nextList.Count > 0))
				{
                    int fMin = int.MaxValue;
                    
                    while(currList.Count > 0)
                    {
                        Node currentNode = currList[0];
                        currList.RemoveAt(0);

                        if(currentNode.fCost > threshold)
                        {
                            fMin = Math.Min(currentNode.fCost, fMin);
                            nextList.Insert(nextList.Count - 1, currentNode);
                            continue;
                        }

                        if(currentNode == targetNode)
                        {
                            path.pathSuccess = true;
                            break;
                        }

                        foreach(Node neighbor in grid.GetNeighbors(meshCopy, currentNode))
                        {
                            if (!neighbor.walkable || cache.Contains(neighbor))
                                continue;

                            int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                            neighbor.gCost = newMovementCostToNeighbor;
                            neighbor.hCost = GetDistance(neighbor, targetNode);
                            neighbor.parent = currentNode;

                            if (!currList.Contains(neighbor))
                            {
                                currList.Insert(0, neighbor);
                                cache.Add(neighbor);
                            }
                        }
                    }

                    threshold = fMin;
                    List<Node> tmp = currList;
                    currList = nextList;
                    nextList = tmp;
				}

                stopWatch.Stop();

                if (path.pathSuccess)
                    path.waypoints = RetracePath(startNode, targetNode);

                //WriteResults(stopWatch.ElapsedMilliseconds, "FringeSearch", cache.Count, path.waypoints.Length);
			}
		}
		catch(Exception ex)
		{
			Debug.Log("Exception in Fringe Search " +ex);
		}

		doneEvent.Set();
	}
}
