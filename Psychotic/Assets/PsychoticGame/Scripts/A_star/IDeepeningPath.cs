#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

//Important to note that any further pathfinding classes must inherit from GridPath
public class IDeepeningPath : GridPath 
{
	#region Constructor
	/// <summary>
	/// Initializes a new instance of the IterativeDeepeningPath object
	/// </summary>
	/// <param name="_grid">_grid.</param>
	/// <param name="_meshCopy">_mesh copy.</param>
	/// <param name="_startPos">_start position.</param>
	/// <param name="_targetPos">_target position.</param>
	/// <param name="_callback">_callback.</param>
	/// <param name="_pathId">_path identifier.</param>
	public IDeepeningPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId, float _totalMs)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId, _totalMs)
	{
		path.pathType = "IterativeDeepening";
	}
	#endregion

	#region FindPath
	/// <summary>
	/// Finds the path using Iterative Deepening method
	/// </summary>
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
			//Starts the stopwatch for timing the algorithm
			stopWatch = System.Diagnostics.Stopwatch.StartNew();
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

        stopWatch.Stop();

		path.totalMs = totalMs + stopWatch.ElapsedMilliseconds;
		path.pathTime = stopWatch.ElapsedMilliseconds;
		path.totalNodes = visitedHash.Count;

		//This must be set to let the Pathfinding object know that thread has completed its job
		doneEvent.Set();
	}
	#endregion

	#region Deepening
	/// <summary>
	/// Recursive method used in performing iterative deepening algorithm 
	/// going down to a specific depth in the tree
	/// </summary>
	/// <param name="root">Root Node</param>
	/// <param name="depth">Depth.</param>
	/// <param name="visitedHash">Visited hash.</param>
	/// <param name="target">Target.</param>
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
	#endregion
}
