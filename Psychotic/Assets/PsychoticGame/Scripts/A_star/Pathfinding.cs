#region Using
using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
#endregion

public class Pathfinding : MonoBehaviour 
{
	#region Private Variables
	PathRequestManager requestManager;
	List<GridPath> paths = new List<GridPath>();
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

        TestFileRecord.CreateFile("TestFile-" + DateTime.Now);
	}
	#endregion

	#region Update
	void Update()
	{
		for(int i = paths.Count - 1; i>=0; i--)
		{
			GridPath path = paths[i];

			if(path.GetEventDone)
			{
				paths.RemoveAt(i);
				requestManager.FinishedProcessingPath(path);
			}
		}
	}
	#endregion
		
	#region A* Pathfinding
	public IEnumerator AStarPathfinding(Vector3 startPos, Vector3 targetPos, bool lineOfSight, Action<Vector3[], bool> callback, int pathId)
	{
		//Added for mesh copy O(n2) heavy
		Node[,] meshCopy = grid.GetGridCopy();

		yield return null;

		UnityEngine.Debug.Log("Going into A* Thread");
		AStarPath aStar = new AStarPath(grid, meshCopy, startPos, targetPos, lineOfSight, callback, pathId);
		paths.Add(aStar);
		ThreadPool.QueueUserWorkItem(aStar.ThreadPoolCallback, paths.Count);
}
	#endregion
	
	#region IterativeDeepeningSearch
	public IEnumerator IterativeDeepening(Vector3 startPos, Vector3 targetPos, Action<Vector3[], bool> callback, int pathId)
	{
		//Added for mesh copy O(n2) heavy
		Node[,] meshCopy = grid.GetGridCopy();
		
		yield return null;
		UnityEngine.Debug.Log("Going into IterativeDeepening Thread");
		IDeepeningPath iDeepening = new IDeepeningPath(grid, meshCopy, startPos, targetPos, callback, pathId);
		paths.Add(iDeepening);
		ThreadPool.QueueUserWorkItem(iDeepening.ThreadPoolCallback, paths.Count);
	}
	#endregion

    public IEnumerator DynamicBiDirectional(Vector3 startPos, Vector3 targetPos, Action<Vector3[], bool> callback, int pathId)
    {
        Node[,] meshCopy = grid.GetGridCopy();

        yield return null;
        UnityEngine.Debug.Log("Going into DynamicBiDirectional Thread");
        DynBiDirBeamPath dynPath = new DynBiDirBeamPath(grid, meshCopy, startPos, targetPos, callback, pathId);
        paths.Add(dynPath);
        ThreadPool.QueueUserWorkItem(dynPath.ThreadPoolCallback, paths.Count);
    }

    public IEnumerator FringeSearch(Vector3 startPos, Vector3 targetPos, Action<Vector3[], bool> callback, int pathId)
    {
        Node[,] meshCopy = grid.GetGridCopy();

        yield return null;
        UnityEngine.Debug.Log("Going into FringeSearch Thread");
        FringeSearchPath fringePath = new FringeSearchPath(grid, meshCopy, startPos, targetPos, callback, pathId);
        paths.Add(fringePath);
        ThreadPool.QueueUserWorkItem(fringePath.ThreadPoolCallback, paths.Count);
    }

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
