using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.IO;

public abstract class GridPath
{
	#region Protected Variables
	//Since this is an abstract class these will be passed down to all inheritors
	protected Grid grid;
	protected Node[,] meshCopy;
	protected Vector3 startPos;
	protected Vector3 targetPos;
	protected ManualResetEvent doneEvent;
	protected int pathId;
    protected object threadHandle;
	protected float totalMs;

    protected System.Diagnostics.Stopwatch stopWatch = new System.Diagnostics.Stopwatch();
	#endregion

	#region Public Variables
	public Action<Vector3[], bool> callback;
	public Path path;
	#endregion

	#region Public Fields
	//Is the thread event done?  Wait 0ms and get result bool
	public bool GetEventDone
	{get{return doneEvent.WaitOne(0);}}

    public Grid GetGridRef
    { get { return grid; } }
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="GridPath"/> class.
	/// </summary>
	/// <param name="_grid">_grid.</param>
	/// <param name="_meshCopy">_mesh copy.</param>
	/// <param name="_startPos">_start position.</param>
	/// <param name="_targetPos">_target position.</param>
	/// <param name="_callback">_callback.</param>
	/// <param name="_pathId">_path identifier.</param>
	public GridPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId, float _totalMs)
	{
		grid = _grid;
		meshCopy = _meshCopy;
		startPos = _startPos;
		targetPos = _targetPos;
		callback = _callback;
		totalMs = _totalMs;

		//Sets the threading manual reset event to false
		doneEvent = new ManualResetEvent(false);

		//Makes a new path object from id
        path = new Path(_pathId);
	}
	#endregion

	#region ThreadPoolCallback
	/// <summary>
	/// Starts a new thread from ThreadPool
	/// </summary>
	/// <param name="threadContext">Thread ID</param>
	public virtual void ThreadPoolCallback(System.Object threadContext)
	{
        threadHandle = threadContext;
		FindPath();
	}
	#endregion

	#region FindPath (Abstract DEF)
	/// <summary>
	/// (!!!This is abstract and must be implemented by inheritors!!!)
	/// Finds the path.
	/// </summary>
	public abstract void FindPath();
	#endregion

	#region WriteResults
	/// <summary>
	/// Writes the results to the test file
	/// </summary>
	/// <param name="time">Time.</param>
	/// <param name="pathType">Path type.</param>
	/// <param name="numExpanded">Number nodes expanded.</param>
	/// <param name="numWaypoints">Number waypoints in path</param>
    public virtual void WriteResults(float pathTime, float totalTime, string pathType, int numExpanded, int numWaypoints, float distance)
    {
        string results = string.Empty;

        results += ("Expanded: " + numExpanded + " Path Length: " + numWaypoints + " Path Type: " + pathType + " Path Time: " + pathTime + "ms " + " Total Time: " +totalMs + " Straight Line Dist: " +distance);
        TestFileRecord.WriteToThatFile(results, threadHandle);
    }
	#endregion

	#region RetracePath
	/// <summary>
	/// Retraces the path starting with the end node and working
	/// its way backwards through each nodes parent
	/// </summary>
	/// <returns>The path.</returns>
	/// <param name="startNode">Start node.</param>
	/// <param name="endNode">End node.</param>
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
	#endregion

	#region GetDistance
	/// <summary>
	/// Gets the distance factor from one node to a next.
	/// Used in computing A* fcost for the node
	/// </summary>
	/// <returns>The distance.</returns>
	/// <param name="nodeA">Node a.</param>
	/// <param name="nodeB">Node b.</param>
	public virtual int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);
		
		if(distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);
		
	}
	#endregion
}

#region PathClass
//Used to store path information after processing has occured
public class Path
{
	public Vector3[] waypoints;
	public bool pathSuccess;
	public float pathTime;
	public float totalMs;
	public string pathType;
	public int totalNodes;

	private int pathId;

	public int PathID
	{get{return pathId;}}

	public Path(int _pathId)
	{
		pathId = _pathId;
        waypoints = new Vector3[0];
	}

	public Path(Vector3[] _waypoints, bool _pathSuccess, int _pathId)
	{
		waypoints = _waypoints;
		pathSuccess = _pathSuccess;
		pathId = _pathId;
	}
}
#endregion
