using UnityEngine;
using System.Collections;

public class HorrorAI : MonoBehaviour {

	public Transform target;
	float speed = 2;
	Vector3[] path;

	int targetIndex;

	void Awake()
	{
		print("Zombie Transform : " +transform.position.x+","+transform.position.y +","+transform.position.z);
		print("Target Transform : " +target.position.x+","+target.position.y +","+target.position.z);
	
		PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
	}

	public void OnPathFound(Vector3[] newPath, bool pathSuccessful)
	{
		if(pathSuccessful)
		{
			path = newPath;
			print ("New path " +path[0].x);
			StopCoroutine("FollowPath");
			StartCoroutine("FollowPath");
		}
	}

	IEnumerator FollowPath()
	{
		Vector3 currentWaypoint = path[0];
		print ("Path at 0 = " +path[0].x);

		while(true)
		{
			if(transform.position == currentWaypoint)
			{
				targetIndex++;
				if(targetIndex >= path.Length)
					yield break;
				currentWaypoint = path[targetIndex];
			}
			transform.position = Vector3.MoveTowards(transform.position, currentWaypoint, speed);
			yield return null;
		}
	}
}
