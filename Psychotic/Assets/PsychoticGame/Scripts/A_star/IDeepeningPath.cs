using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class IDeepeningPath : GridPath 
{
	public IDeepeningPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)
	{

	}

	public override void FindPath ()
	{
		grid.ResetNodes(meshCopy);
		
		HashSet<Node> visitedHash = new HashSet<Node>();
		Node startNode = grid.NodeFromWorldPoint(meshCopy, startPos);
		Node targetNode = grid.NodeFromWorldPoint(meshCopy, targetPos);
		visitedHash.Add(startNode);

		int depth = 0;
		path.pathSuccess = false;
		
		try
		{
			while(!path.pathSuccess && visitedHash.Count < grid.MaxSize)
			{
				path.pathSuccess = Deepening(startNode, depth, visitedHash, targetNode);
			
				visitedHash.RemoveWhere(n => n.parent != null && n != startNode);
			
				depth++;
			}

			if(path.pathSuccess)
				path.waypoints = RetracePath(startNode, targetNode);
		}
		catch(Exception ex)
		{
			Debug.Log("Error in find path method of IDeepening path");
			path.pathSuccess = false;
		}

		doneEvent.Set();
	}

	bool Deepening(Node root, int depth, HashSet<Node> visitedHash, Node target)
	{
		bool targetFound = false;
		Heap<Node> nodeHeap = new Heap<Node>(8);

		try
		{
			if(depth >= 0)
			{
				if(root == target)
				{
					return true;
				}
				else
				{
					foreach(Node n in grid.GetNeighbors(root))
					{
						if(!visitedHash.Contains(n) && n.walkable)
						{
							n.hCost = GetDistance(n, target);
							nodeHeap.Add(n);
							visitedHash.Add(n);
						}
					}
				
					while(nodeHeap.Count > 0 && targetFound == false)
					{
						Node node = nodeHeap.RemoveFirst();
						node.parent = root;
						targetFound = Deepening(node, depth-1, visitedHash, target);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Debug.Log("Exception in Deepening method of IDeepening Path" +ex);
			return false;
		}
		return targetFound;
	}
}
