#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public class FringeSearchPath : GridPath
{
	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="FringeSearchPath"/> class.
	/// </summary>
	/// <param name="_grid">_grid.</param>
	/// <param name="_meshCopy">_mesh copy.</param>
	/// <param name="_startPos">_start position.</param>
	/// <param name="_targetPos">_target position.</param>
	/// <param name="_callback">_callback.</param>
	/// <param name="_pathId">_path identifier.</param>
	public FringeSearchPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)
	{

	}
	#endregion

	#region FindPath
	/// <summary>
	/// (!!!This is abstract and must be implemented by inheritors!!!)
	/// Finds the path.
	/// </summary>
	public override void FindPath()
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		try
		{
			if(startNode.walkable && targetNode.walkable)
			{
				//Starts the stopwatch timer
                stopWatch.Start();

				HashSet<Node> cache = new HashSet<Node>();
                List<Node> nextList = new List<Node>();
				List<Node> currList = new List<Node>();

				//Adds initial node to current processing list
                currList.Add(startNode);

				//Sets the start threshold for distance in tree
				int threshold = GetDistance(startNode, targetNode);

                Debug.Log("Made it to first while loop");
				while(!path.pathSuccess && (currList.Count > 0 || nextList.Count > 0))
				{
					//Resets fMin to infinity (INT_MAX)
                    int fMin = int.MaxValue;

                    while(currList.Count > 0)
                    {
                        Debug.Log("Setting current node");
                        Node currentNode = currList[0];
                        cache.Add(currentNode);
                        currList.RemoveAt(0);
                        Debug.Log("Removing from current list and adding to cache");

                        if(currentNode.fCost > threshold)
                        {
                            Debug.Log("Adding node to next list for later");
                            fMin = Math.Min(currentNode.fCost, fMin);
                            nextList.Add(currentNode);
                            continue;
                        }

                        Debug.Log("Checking For End State");
                        if(currentNode.gridX == targetNode.gridX && currentNode.gridY == targetNode.gridY)
                        {
                            Debug.Log("End State Found");
                            path.pathSuccess = true;
                            break;
                        }

                        Debug.Log("Getting new neighbors");
                        foreach (Node neighbor in grid.GetNeighbors(meshCopy, currentNode))
                        {
                            if (!neighbor.walkable || cache.Contains(neighbor))
                            {
                                continue;
                            }

                            int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);

                            if (!currList.Contains(neighbor) || newMovementCostToNeighbor < neighbor.gCost)
                            {
                                neighbor.gCost = newMovementCostToNeighbor;
                                neighbor.hCost = GetDistance(neighbor, targetNode);
                                neighbor.parent = currentNode;

                                if (!currList.Contains(neighbor))
                                {
                                    currList.Add(neighbor);
                                }
                            }
                        }
                    }

                    Debug.Log("Setting current list to next list");
                    threshold = fMin;
                    List<Node> tmp = currList;
                    currList = nextList;
                    nextList = new List<Node>();
				}

                stopWatch.Stop();

                Debug.Log("Pathfinding Completed");

                if (path.pathSuccess)
                    path.waypoints = RetracePath(startNode, targetNode);

				//Write the test results of pathfinding to the test file
                WriteResults(stopWatch.ElapsedMilliseconds, "FringeSearch", cache.Count, path.waypoints.Length);
			}
		}
		catch(Exception ex)
		{
			Debug.Log("Exception in Fringe Search " +ex);
		}

		//This must be done to notify of thread completion
		doneEvent.Set();
	}
	#endregion
}
