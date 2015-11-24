using UnityEngine;
using System.Collections.Generic;
using System;
using System.Collections;

public class AStarPath : GridPath 
{
	private bool isVisible;

	public AStarPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, bool _isVisible, Action<Vector3[], bool> _callback, int _pathId)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)
	{
		isVisible = _isVisible;
	}

	public override void FindPath ()
	{
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
						if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
							neighbour.gCost = newMovementCostToNeighbour;
							neighbour.hCost = GetDistance(neighbour, targetNode);
							neighbour.parent = currentNode;
							
							if (!openSet.Contains(neighbour))
								openSet.Add(neighbour);
						}
					}
				}
			}
		}
		}
		catch(Exception ex)
		{
			Debug.Log("Exception in A* pathfinding " +ex);
		}

		if(pathSuccess)
			path.waypoints = RetracePath(startNode, targetNode);

		path.pathSuccess = pathSuccess;
		doneEvent.Set();
	}
}
