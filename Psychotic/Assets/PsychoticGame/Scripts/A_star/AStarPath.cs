#region Using
using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;
#endregion

public class AStarPath : GridPath 
{
	#region Private Varialbes
	private bool isVisible;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="AStarPath"/> class.
	/// </summary>
	/// <param name="_grid">_grid.</param>
	/// <param name="_meshCopy">_mesh copy.</param>
	/// <param name="_startPos">_start position.</param>
	/// <param name="_targetPos">_target position.</param>
	/// <param name="_isVisible">If set to <c>true</c> _is visible.</param>
	/// <param name="_callback">_callback.</param>
	/// <param name="_pathId">_path identifier.</param>
	public AStarPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, bool _isVisible, Action<Vector3[], bool> _callback, int _pathId)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)
	{
		isVisible = _isVisible;
	}
	#endregion

	/// <summary>
	/// (!!!This is abstract and must be implemented by inheritors!!!)
	/// Finds the path.
	/// </summary>
	public override void FindPath ()
	{
		//Starts the stopwatch timer for testing
        stopWatch.Start();

		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(meshCopy, startPos);
		Node targetNode = grid.NodeFromWorldPoint(meshCopy, targetPos);

		try
		{
			if (startNode.walkable && targetNode.walkable) 
			{
				Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
				HashSet<Node> closedSet = new HashSet<Node>();
				openSet.Add(startNode);
			
				while (openSet.Count > 0) 
				{
					Node currentNode = openSet.RemoveFirst();
					closedSet.Add(currentNode);
				
					if (currentNode == targetNode) 
					{
						pathSuccess = true;
						break;
					}
				
					foreach (Node neighbour in grid.GetNeighbors(meshCopy, currentNode)) 
					{
						if (!neighbour.walkable || closedSet.Contains(neighbour)) 
						{
							continue;
						}
					
						int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
					
						if(isVisible == true && currentNode.parent != null)
						{
							if (currentNode.parent.gCost + GetDistance(currentNode.parent, neighbour) < neighbour.gCost && !openSet.Contains(neighbour) && currentNode != startNode)
							{
								neighbour.parent = currentNode.parent;
								neighbour.gCost = currentNode.parent.gCost + GetDistance(currentNode.parent, neighbour);
							}
							else if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) 
							{
								neighbour.gCost = newMovementCostToNeighbour;
								neighbour.hCost = GetDistance(neighbour, targetNode);
								neighbour.parent = currentNode;
							
								if (!openSet.Contains(neighbour))
									openSet.Add(neighbour);
							}
						}
						else
						{
							if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) 
							{
								neighbour.gCost = newMovementCostToNeighbour;
								neighbour.hCost = GetDistance(neighbour, targetNode);
								neighbour.parent = currentNode;
							
								if (!openSet.Contains(neighbour))
									openSet.Add(neighbour);
							}
						}
					}
				}

				//Stops the stopwatch timer when pathfinding completed
            	stopWatch.Stop();

				//Writes the results of pathfinding to test file
           	 	WriteResults(stopWatch.ElapsedMilliseconds, "A*", (openSet.Count + closedSet.Count), closedSet.Count);
			}
		}
		catch(Exception ex)
		{
			Debug.Log("Exception in A* pathfinding " +ex);
		}


		if(pathSuccess)
			path.waypoints = RetracePath(startNode, targetNode);

		path.pathSuccess = pathSuccess;

		//This must be set for threading to be completed
		doneEvent.Set();
	}
}
