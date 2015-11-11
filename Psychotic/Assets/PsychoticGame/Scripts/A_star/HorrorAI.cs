#region Using
using UnityEngine;
using System.Collections;
#endregion

public class HorrorAI : MonoBehaviour
{

	#region Public Hidden Variables
	[HideInInspector]
	public float DEFAULT_WALKING_SPEED = 2.75f;
	public float DEFUALT_RUNNING_SPEED = 4.0f;
	public double interval;
	#endregion

	#region Public Variables
	public Transform target;
	public float speed = 2;
	public double newPathAvgTime;
	public float rotationSpeed;
	public Transform eyes;
	#endregion

	#region Private Variables
	Vector3[] path;
	int targetIndex;
	private Quaternion lookRotation;
	private Vector3 direction;
	private bool  targetReached = true;
	#endregion

	#region Public Fields
	public int PathCount
	{
		get{return path.Length;}
	}

	public bool TargetReached
	{
		get{return targetReached;}
	}
	#endregion

	#region Start
	/// <summary>
	/// Start this instance.
	/// Requests the first path for the AI
	/// </summary>
	void Start ()
	{
		path = new Vector3[0];
		//PathRequestManager.RequestPath (transform.position, target.position, OnPathFound);
	}
	#endregion

	#region Update
	/// <summary>
	/// Update this instance.
	/// If the target position has been reached
	/// then start new path in random interval
	/// </summary>
	void Update ()
	{

	}
	#endregion

	#region CallForNewPath
	/// <summary>
	/// Calls for new path after a random time interval
	/// has passed
	/// </summary>
	public void CallForNewPath (Vector3 newTarget, string pathType, bool lineOfSight)
	{
		PathRequestManager.RequestPath (transform.position, newTarget, OnPathFound, pathType, lineOfSight);
	}
	#endregion

	#region OnPathFound
	/// <summary>
	/// If a path was obtained using coordinates then
	/// start a new Coroutine that follows the new path
	/// </summary>
	/// <param name="newPath">New path.</param>
	/// <param name="pathSuccessful">If set to <c>true</c> path successful.</param>
	public void OnPathFound (Vector3[] newPath, bool pathSuccessful)
	{
		if (pathSuccessful) 
		{
			path = newPath;
			targetReached = false;
			StopCoroutine ("FollowPath");
			StartCoroutine ("FollowPath");
		}
	}
	#endregion

	#region FollowPath
	/// <summary>
	/// Enumerator used to follow path on a separate thread
	/// </summary>
	/// <returns>The path.</returns>
	IEnumerator FollowPath ()
	{
		//Check for actual path and yield return null if path is empty
		Vector3 currentWaypoint = path [0];

		while (true) {
			if (WithinOne (currentWaypoint)) {
				targetIndex++;
				if (targetIndex >= path.Length) {
					targetReached = true;
					targetIndex = 0;
					yield break;
				}
				currentWaypoint = path [targetIndex];
			}

			PerformRotation(this.transform, currentWaypoint);
			transform.position = Vector3.MoveTowards (transform.position, currentWaypoint, speed * Time.deltaTime);

			yield return null;
		}
	}
	#endregion

	#region OnDrawGizmos
	/// <summary>
	/// Draws black cube gizmos linked by lines
	/// along the current path of the AI
	/// </summary>
	public void OnDrawGizmos ()
	{
		if (path != null) {
			for (int i = targetIndex; i<path.Length; i++) {
				Gizmos.color = Color.black;
				Gizmos.DrawCube (path [i], Vector3.one);

				if (i == targetIndex) {
					Gizmos.DrawLine (transform.position, path [i]);
				} else {
					Gizmos.DrawLine (path [i - 1], path [i]);
				}
			}
		}
	}
	#endregion

	#region WithinOne
	/// <summary>
	/// Returns true is the current transform location is
	/// within 1 unit of the target space.  This is used 
	/// since animation causes transform to never exactly equal
	/// target location.
	/// </summary>
	/// <returns><c>true</c>, if one was withined, <c>false</c> otherwise.</returns>
	/// <param name="target">Target.</param>
	bool WithinOne (Vector3 target)
	{
		if (Mathf.Abs (target.x - transform.position.x) <= 1.5f && Mathf.Abs (target.z - transform.position.z) <= 1.5f)
			return true;
		else
			return false;
	}
	#endregion

	public void PerformRotation(Transform trans, Vector3 currentWaypoint)
	{

		direction = (currentWaypoint - trans.position).normalized;
		lookRotation = Quaternion.LookRotation (direction);
		trans.rotation = Quaternion.Slerp (trans.rotation, lookRotation, Time.deltaTime * rotationSpeed);

	}
}
