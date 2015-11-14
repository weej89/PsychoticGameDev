#region Using
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
#endregion

public class Pathfinding : MonoBehaviour 
{
	#region Private Variables
	PathRequestManager requestManager;
	Grid grid;

	List<Node> checkedNodes = new List<Node>();
	List<Node> usedNodes = new List<Node>();
	#endregion

	#region Awake
	/// <summary>
	/// Awake this instance.  Gets a PathRequestManager component
	/// as well as a Grid component.
	/// </summary>
	void Awake()
	{
		requestManager = GetComponent<PathRequestManager>();
		grid=GetComponent<Grid>();
	}
	#endregion

	/*
	#region StartFindPath
	/// <summary>
	/// Starts a new Coroutine to Find a path with given
	/// start and end point
	/// </summary>
	/// <param name="startPos">Start position.</param>
	/// <param name="targetPos">Target position.</param>
	public void StartFindPath(Vector3 startPos, Vector3 targetPos, string pathType, bool lineOfSight)
	{
		StartCoroutine(FindPath(startPos, targetPos, pathType, lineOfSight));
	}
	#endregion
*/
	#region A* Pathfinding
	public IEnumerator AStarPathfinding(Vector3 startPos, Vector3 targetPos, bool lineOfSight)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Vector3[] waypoints = new Vector3[0];
		
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		//if (startNode.walkable && targetNode.walkable) {
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);
			
			while (openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();
				closedSet.Add(currentNode);
				
				if (currentNode == targetNode) {
					sw.Stop();
					print ("Path found: " + sw.ElapsedMilliseconds + " ms");
					pathSuccess = true;
					break;
				}
				
				foreach (Node neighbour in grid.GetNeighbors(currentNode)) {
					if (!neighbour.walkable || closedSet.Contains(neighbour)) {
						continue;
					}
					
					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);

					if(lineOfSight == true)
					{
						if (currentNode.parent.gCost + GetDistance(currentNode.parent, neighbour) < neighbour.gCost && !openSet.Contains(neighbour) && currentNode != startNode)
						{
							neighbour.parent = currentNode.parent;
							neighbour.gCost = currentNode.parent.gCost + GetDistance(currentNode.parent, neighbour);
						}
						else if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
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
		//}

		yield return null;

		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}

		requestManager.FinishedProcessingPath(waypoints, pathSuccess);
	}
	#endregion

	#region FringeSearch
	public IEnumerator FringeSearch(Vector3 startPoint, Vector3 targetPos)
	{
		List<Node> fringe = new List<Node>();
		HashSet<Node> cache = new HashSet<Node>();
		bool found = false;
		int flimit;

		Node startNode = grid.NodeFromWorldPoint(startPoint);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		cache.Add(startNode);
		fringe.Add(startNode);
		flimit = startNode.hCost;

		while(!found && fringe.Count > 0)
		{
			int fmin = int.MaxValue;

			for(int i = 0; i < fringe.Count; i++)
			{
				Node n = fringe[i];
				n.hCost = GetDistance(n, targetNode);
				n.parent = fringe[i];
				cache.Add(n);
			}
		}
		
		yield return null;

		if(found)
			RetracePath(startNode, targetNode);

	}
	#endregion

	#region IterativeDeepeningSearch
	public IEnumerator IterativeDeepening(Vector3 startPoint, Vector3 targetPos)
	{
		grid.ResetNodes();

		HashSet<Node> visitedHash = new HashSet<Node>();
		Node startNode = grid.NodeFromWorldPoint(startPoint);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		visitedHash.Add(startNode);

		Vector3[] waypoints = new Vector3[0];
		
		int depth = 0;
		bool targetFound = false;

		Stopwatch sw = new Stopwatch();
		sw.Start();

		while(!targetFound && visitedHash.Count < grid.MaxSize)
		{
			targetFound = Deepening(startNode, depth, visitedHash, targetNode);

			visitedHash.RemoveWhere(n => n.parent != null && n != startNode);

			depth++;
		}

		sw.Stop();
		print ("Path found: " + sw.ElapsedMilliseconds + " ms");

		yield return null;

		if(targetFound)
			waypoints = RetracePath(startNode, targetNode);

		requestManager.FinishedProcessingPath(waypoints, targetFound);
	}

	bool Deepening(Node root, int depth, HashSet<Node> visitedHash, Node target)
	{
		bool targetFound = false;
		Heap<Node> nodeHeap = new Heap<Node>(8);

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
		return targetFound;
	}
	#endregion

	#region NewPathfinding
	public IEnumerator DepthFirstSearch(Vector3 startPoint, Vector3 targetPos)
	{
		Vector3[] waypoints = new Vector3[0];
		Stack<Node> depthStack = new Stack<Node>();
		HashSet<Node> visitedNodes = new HashSet<Node>();
		bool pathFound = false;

		Node startNode = grid.NodeFromWorldPoint(startPoint);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		visitedNodes.Add(startNode);
		depthStack.Push(startNode);

		pathFound = DepthFirst(depthStack, depthStack.Pop(), visitedNodes, targetNode);

		if(pathFound)
		{
			waypoints = RetracePath(grid.NodeFromWorldPoint(startPoint), grid.NodeFromWorldPoint(targetPos));
		}

		yield return null;

		requestManager.FinishedProcessingPath(waypoints, pathFound);
	}

	bool DepthFirst(Stack<Node> depthStack, Node root, HashSet<Node> visitedNodes, Node target)
	{
		bool targetFound = false;

		foreach(Node node in grid.GetNeighbors(root))
		{
			if(node.walkable && !visitedNodes.Contains(node))
			{
				node.parent = root;
				visitedNodes.Add(node);
				depthStack.Push(node);

				if(node == target)
				{
					targetFound = true;
					return targetFound;
				}
			}
		}

		if(depthStack.Count > 0)
			targetFound = DepthFirst(depthStack, depthStack.Pop(), visitedNodes, target);

		return targetFound;
	}
	#endregion

	#region RetracePath
	/// <summary>
	/// Retraces the path through a start and end node
	/// by using parent association and reverses the array
	/// constructed before returning for use by the AI
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
	Vector3[] RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		List<Vector3> points = new List<Vector3>();
		Node currentNode = endNode;

		while(currentNode != startNode)
		{
			path.Add(currentNode);
			points.Add(currentNode.worldPosition);
			currentNode = currentNode.parent;
		}
		//Vector3[] waypoints = SimplifyPath(path);
		Vector3[] waypoints = points.ToArray();
		Array.Reverse(waypoints);

		return waypoints;
	}
	#endregion

	#region SimplifyPath
	/// <summary>
	/// Smooths the path by making sure that there is not an
	/// overabundance of nodes going in same direction all pointing
	/// to each other.  Used for optimization puroposes
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="path">Path.</param>
	Vector3[] SimplifyPath(List<Node> path)
	{
		List<Vector3> waypoints = new List<Vector3>();
		Vector2 directionOld = Vector2.zero;

		for(int i = 1; i<path.Count; i++)
		{
			Vector2 directionNew = new Vector2(path[i-1].gridX - path[i].gridX, path[i-1].gridY - path[i].gridY);
			if(directionNew != directionOld)
			{
				waypoints.Add(path[i].worldPosition);
			}
			directionOld=directionNew;
		}

		return waypoints.ToArray();
	}
	#endregion

	#region GetDistance
	/// <summary>
	/// Gets the distance factor from one node to a next.
	/// Used in computing A* fcost for the node
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="nodeA">Node a.</param>
	/// <param name="nodeB">Node b.</param>
	int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if(distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);

	}
	#endregion


	void OnDrawGizmos()
	{
		if(usedNodes != null && checkedNodes != null)
		{
			foreach(Node node in usedNodes)
			{
				Gizmos.color = Color.yellow;
				Gizmos.DrawCube(node.worldPosition, Vector3.one);
			}
			foreach(Node node in checkedNodes)
			{
				Gizmos.color = Color.green;
				Gizmos.DrawCube(node.worldPosition, Vector3.one);
			}
		}
	}

}
