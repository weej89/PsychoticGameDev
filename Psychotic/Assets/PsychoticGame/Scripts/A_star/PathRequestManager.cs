#region Using
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;
#endregion

public class PathRequestManager : MonoBehaviour {

	#region Private Variables
	public int MAX_WORKING_PATHS = 4;

	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;
	int numWorkingPaths = 0;

	static List<int> enemysInQueue = new List<int>();
	static PathRequestManager instance;
	Pathfinding pathfinding;
	
	bool isProcessingPath = false;
	#endregion

	#region Public Fields
	public static bool IsProcessingPath
	{
		get{return instance.isProcessingPath;}
	}

	public static bool IsInQueue(int id)
	{return enemysInQueue.Contains(id);}
	#endregion

	#region Awake
	/// <summary>
	/// Awake this instance.
	/// Make a new instance of this class for
	/// static method
	/// </summary>
	void Awake() 
	{
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}
	#endregion

	#region RequestPath
	/// <summary>
	/// Requests a path from the manager and adds it to the queue
	/// </summary>
	/// <param name="pathStart">Path start.</param>
	/// <param name="pathEnd">Path end.</param>
	/// <param name="callback">Callback.</param>
	/// <param name="pathType">Path type.</param>
	/// <param name="lineOfSight">If set to <c>true</c> line of sight.</param>
	/// <param name="id">Identifier to make sure that correct thread grabs it path</param>
	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, string pathType, bool lineOfSight, int id) 
	{
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback, pathType, lineOfSight, id);
		enemysInQueue.Add(id);
		instance.pathRequestQueue.Enqueue(newRequest);
		newRequest.sw.Start();
		instance.TryProcessNext();
	}
	#endregion

	#region TryProcessNext
	/// <summary>
	/// Tries to process the next path by checking the current
	/// thread count and allowing another to be started if the count
	/// is less than the max threads limit.  Coroutines are used here
	/// to optimize timings for frame processing
	/// </summary>
	void TryProcessNext() 
	{
		if (numWorkingPaths < MAX_WORKING_PATHS && pathRequestQueue.Count > 0){
			currentPathRequest = pathRequestQueue.Dequeue();
			numWorkingPaths ++;

			currentPathRequest.sw.Stop();

			switch(currentPathRequest.pathfindingType)
			{
				case "A*":
				StartCoroutine(pathfinding.AStarPathfinding(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.lineOfSight, currentPathRequest.callback, currentPathRequest.pathId, currentPathRequest.sw.ElapsedMilliseconds));
				break;
				case "DepthFirst":
				StartCoroutine(pathfinding.DepthFirstSearch(currentPathRequest.pathStart, currentPathRequest.pathEnd));
				break;
				case "IterativeDeepening":
				StartCoroutine(pathfinding.IterativeDeepening(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.callback, currentPathRequest.pathId, currentPathRequest.sw.ElapsedMilliseconds));
				break;
                case "DynamicBiDirectional":
				StartCoroutine(pathfinding.DynamicBiDirectional(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.callback, currentPathRequest.pathId, currentPathRequest.sw.ElapsedMilliseconds));
                break;
                case "Fringe":
				StartCoroutine(pathfinding.FringeSearch (currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.callback, currentPathRequest.pathId, currentPathRequest.sw.ElapsedMilliseconds));
                break;
			}
		}
	}
	#endregion

	#region FinishedProcessingPath
	/// <summary>
	/// Finisheds the processing path and tries to process the next
	/// </summary>
	/// <param name="path">Path.</param>
	/// <param name="success">If set to <c>true</c> success.</param>
	public void FinishedProcessingPath(Vector3[] path, bool success) {
		currentPathRequest.callback(path,success);
		isProcessingPath = false;
		TryProcessNext();
	}

	public void FinishedProcessingPath(GridPath foundPath) 
	{
		if(foundPath.path.pathSuccess)
		{
			foundPath.WriteResults(foundPath.path.pathTime, foundPath.path.pathTime + foundPath.path.totalMs, foundPath.path.pathType, foundPath.path.totalNodes, foundPath.path.waypoints.Length, Vector3.Distance(foundPath.path.waypoints[0], foundPath.path.waypoints[foundPath.path.waypoints.Length-1]));
		}

		foundPath.callback(foundPath.path.waypoints, foundPath.path.pathSuccess);

		enemysInQueue.Remove(foundPath.path.PathID);
		numWorkingPaths --;
		TryProcessNext();
	}
	#endregion

	#region PathRequest (DataStructure)
	//This struct is used for passing path data back and forth between the
	//PathManager and Pathfinding object
	public class PathRequest 
	{
		public bool lineOfSight;
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;
		public string pathfindingType;
		public int pathId;

		public System.Diagnostics.Stopwatch sw = new Stopwatch();
		
		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback, string _pathfindingType, bool _lineOfSight, int _pathId) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			pathfindingType = _pathfindingType;
			lineOfSight = _lineOfSight;
			pathId = _pathId;
		}
	}
	#endregion
}
