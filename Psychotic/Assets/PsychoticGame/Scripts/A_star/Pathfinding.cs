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

	#region FindPath
	/// <summary>
	/// Enumerator used as Coroutine to find a path to target node
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="startPos">Start position.</param>
	/// <param name="targetPos">Target position.</param>
	IEnumerator FindPath(Vector3 startPos, Vector3 targetPos, string pathfindingType, bool lineOfSight)
	{
		Vector3[] waypoints = new Vector3[0];
		bool pathFound = false;

		switch (pathfindingType)
		{
			case "A*":
			pathFound = AStarPathfinding(startPos, targetPos, ref waypoints, lineOfSight);
			break;
			case "DepthFirst":
			pathFound = FindPathNew(startPos, targetPos, ref waypoints);
			break;
		}

		requestManager.FinishedProcessingPath(waypoints, pathFound);

		yield return null;
	}
	#endregion

	#region A* Pathfinding
	bool AStarPathfinding(Vector3 startPos, Vector3 targetPos, ref Vector3[] waypoints, bool lineOfSight)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);
		
		if (startNode.walkable && targetNode.walkable) {
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
						if (currentNode.parent.gCost + GetDistance(currentNode.parent, neighbour) < neighbour.gCost)
						{
							neighbour.parent = currentNode.parent;
							neighbour.gCost = currentNode.parent.gCost + GetDistance(currentNode.parent, neighbour);
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

		if (pathSuccess) {
			waypoints = RetracePath(startNode,targetNode);
		}

		return pathSuccess;
	}
	/*
	bool AStarPathfinding(Vector3 startPos, Vector3 targetPos, ref Vector3[] waypoints)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();
		
		bool pathSuccess = false;
		
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);
		
		//if(targetNode.walkable)
		//{
			Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
			HashSet<Node> closedSet = new HashSet<Node>();
			
			openSet.Add(startNode);
			
			while (openSet.Count>0)
			{
				Node currentNode=openSet.RemoveFirst();
				closedSet.Add(currentNode);
				usedNodes.Add(currentNode);
				
				if(currentNode==targetNode)
				{
					sw.Stop();
					pathSuccess = true;
					//print("Path found: " +sw.ElapsedMilliseconds+ " ms");
					break;
				}
				
				foreach(Node neighbor in grid.GetNeighbors(currentNode))
				{
					if(!neighbor.walkable || closedSet.Contains(neighbor))
					{
						continue;
					}
					
					int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
					if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
					{
						neighbor.gCost = newMovementCostToNeighbor;
						neighbor.hCost = GetDistance(neighbor, targetNode);
						neighbor.parent = currentNode;
						
						if(!openSet.Contains(neighbor))
						{
							openSet.Add(neighbor);
							checkedNodes.Add(neighbor);
						}
						else
							openSet.UpdateItem(neighbor);
					}
				}
			}
		//}
		//yield return null;
		
		if(pathSuccess)
		{
			waypoints = RetracePath(startNode, targetNode);
		}

		usedNodes.Clear();
		checkedNodes.Clear();

		return pathSuccess;
	}
	*/

	#endregion

	#region NewPathfinding
	bool FindPathNew(Vector3 startPoint, Vector3 targetPos, ref Vector3[] waypoints)
	{
		Stack<Node> depthStack = new Stack<Node>();
		HashSet<Node> visitedNodes = new HashSet<Node>();
		bool pathFound = false;

		Node startNode = grid.NodeFromWorldPoint(startPoint);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		visitedNodes.Add(startNode);
		depthStack.Push(startNode);

		pathFound = DepthFirst(depthStack, depthStack.Pop(), visitedNodes, targetNode);

		//yield return null;

		if(pathFound)
		{
			waypoints = RetracePath(grid.NodeFromWorldPoint(startPoint), grid.NodeFromWorldPoint(targetPos));
		}

		return pathFound;
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
