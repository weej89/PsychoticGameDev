#region Using
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
#endregion

public class PathRequestManager : MonoBehaviour {

	#region Private Variables
	Queue<PathRequest> pathRequestQueue = new Queue<PathRequest>();
	PathRequest currentPathRequest;
	
	static PathRequestManager instance;
	Pathfinding pathfinding;
	
	bool isProcessingPath;
	#endregion

	public static bool IsProcessingPath
	{
		get{return instance.isProcessingPath;}
	}

	#region Awake
	/// <summary>
	/// Awake this instance.
	/// Make a new instance of this class for
	/// static method
	/// </summary>
	void Awake() {
		instance = this;
		pathfinding = GetComponent<Pathfinding>();
	}
	#endregion

	#region RequestPath
	/// <summary>
	/// Static method to request a path from the PathRequest Instance
	/// </summary>
	/// <param name="pathStart">Path start.</param>
	/// <param name="pathEnd">Path end.</param>
	/// <param name="callback">Callback.</param>
	public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback, string pathType, bool lineOfSight) {
		PathRequest newRequest = new PathRequest(pathStart,pathEnd,callback, pathType, lineOfSight);
		instance.pathRequestQueue.Enqueue(newRequest);
		instance.TryProcessNext();
	}
	#endregion

	#region TryProcessNext
	/// <summary>
	/// Processes the next path request if the current one
	/// is finished and another one remains
	/// </summary>
	void TryProcessNext() {
		if (!isProcessingPath && pathRequestQueue.Count > 0) {
			currentPathRequest = pathRequestQueue.Dequeue();
			isProcessingPath = true;
			pathfinding.StartFindPath(currentPathRequest.pathStart, currentPathRequest.pathEnd, currentPathRequest.pathfindingType, currentPathRequest.lineOfSight);
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
	#endregion

	#region PathRequest (DataStructure)
	struct PathRequest {
		public bool lineOfSight;
		public Vector3 pathStart;
		public Vector3 pathEnd;
		public Action<Vector3[], bool> callback;
		public string pathfindingType;
		
		public PathRequest(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback, string _pathfindingType, bool _lineOfSight) {
			pathStart = _start;
			pathEnd = _end;
			callback = _callback;
			pathfindingType = _pathfindingType;
			lineOfSight = _lineOfSight;
		}
	}
	#endregion
}
