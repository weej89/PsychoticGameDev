using UnityEngine;
using System.Collections;

public class HorrorAI : MonoBehaviour {

	#region Public Variables
	public Transform target;
	public float speed = 2;
	public double newPathAvgTime;
	public float rotationSpeed;
	#endregion

	#region Private Variables
	Vector3[] path;
	int targetIndex;
	double currentTime = 0;
	double interval;
	bool targetReached = false;

	private Quaternion lookRotation;
	private Vector3 direction;
	#endregion

	void Start()
	{
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	}

	void Update()
	{
		if(targetReached == true)
			CallForNewPath();
	}

	void CallForNewPath()
	{
		if(currentTime < interval)
			currentTime += Time.deltaTime;
		else
		{
			targetReached = false;
			currentTime = 0;
			PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
		}
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if(pathSuccessful)
		{
			path = newPath;
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];

		while(true)
		{
			if(WithinOne(currentWaypoint))
			{
				targetIndex++;
				if(targetIndex >= path.Length)
				{
					targetReached = true;
					interval = GetNextRandomInterval(newPathAvgTime);
					targetIndex=0;
					yield break;
				}
				currentWaypoint = path[targetIndex];
			}

			direction = (currentWaypoint - transform.position).normalized;
			lookRotation = Quaternion.LookRotation(direction);
			transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed * Time.deltaTime);
			yield return null;
		}
	}

	public void OnDrawGizmos()
	{
		if(path!=null)
		{
			for(int i = targetIndex; i<path.Length; i++)
			{
				Gizmos.color = Color.black;
				Gizmos.DrawCube(path[i], Vector3.one);

				if(i == targetIndex)
				{
					Gizmos.DrawLine(transform.position, path[i]);
				}
				else
				{
					Gizmos.DrawLine(path[i-1], path[i]);
				}
			}
		}
	}

	bool WithinOne(Vector3 target)
	{
		if(Mathf.Abs(target.x - transform.position.x) < 1 && Mathf.Abs(target.z - transform.position.z) < 1)
			return true;
		else
			return false;
	}

	public double GetNextRandomInterval(double avg)
	{
		avg = (1/avg);
		double interval = -Mathf.Log((float)(1.0 - Random.value)) / avg;
		print ("Interval: "+interval);
		return interval;
	}
}
