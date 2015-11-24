using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public abstract class GridPath
{
	protected Grid grid;
	protected Node[,] meshCopy;
	protected Vector3 startPos;
	protected Vector3 targetPos;
	protected ManualResetEvent doneEvent;
	protected int pathId;

	public Action<Vector3[], bool> callback;
	public Path path;

	public bool GetEventDone
	{get{return doneEvent.WaitOne(0);}}

	public GridPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
	{
		grid = _grid;
		meshCopy = _meshCopy;
		startPos = _startPos;
		targetPos = _targetPos;
		callback = _callback;

		doneEvent = new ManualResetEvent(false);
		path = new Path(_pathId);
	}

	public virtual void ThreadPoolCallback(System.Object threadContext)
	{
		FindPath();
	}

	public abstract void FindPath();

	public virtual Vector3[] RetracePath(Node startNode, Node endNode)
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

		Vector3[] waypoints = points.ToArray();
		Array.Reverse(waypoints);
		
		return waypoints;
	}

	#region GetDistance
	/// <summary>
	/// Gets the distance factor from one node to a next.
	/// Used in computing A* fcost for the node
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="nodeA">Node a.</param>
	/// <param name="nodeB">Node b.</param>
	protected virtual int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if(distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
		
	}
	#endregion
}

public class Path
{
	public Vector3[] waypoints;
	public bool pathSuccess;

	private int pathId;

	public int PathID
	{get{return pathId;}}

	public Path(int _pathId)
	{
		pathId = _pathId;
	}

	public Path(Vector3[] _waypoints, bool _pathSuccess, int _pathId)
	{
		waypoints = _waypoints;
		pathSuccess = _pathSuccess;
		pathId = _pathId;
	}
}
