using UnityEngine;
using System;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float fieldOfViewAngle = 110f;
	public float sightRange = 35f;
	public Vector3 offset = new Vector3 (0, .5f, 0);
	public Transform eyes;

	[HideInInspector] public Transform targetLocation;
	[HideInInspector] public bool playerInSight;
	[HideInInspector] public bool playerInCollider;
	[HideInInspector] public float playerLastSeenTime;

	private SphereCollider col;
	private float timeSinceAudibleCheck = 0;
	private const int AUDIBLE_COOLDOWN = 4000;

	// Use this for initialization
	void Start () 
	{
		col = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeSinceAudibleCheck+=Time.deltaTime;
		playerLastSeenTime += Time.deltaTime;
	}

	void OnTriggerStay (Collider other)
	{
		// If the player has entered the trigger sphere...
		if(other.gameObject.CompareTag("Player"))
		{
			// By default the player is not in sight or collider
			playerInSight = false;
			playerInCollider = true;
			targetLocation = other.transform;

			// Create a vector from the enemy to the player and store the angle between it and forward.
			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);
			
			// If the angle between forward and where the player is, is less than half the angle of view...
			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;
				Debug.DrawRay(transform.position + transform.up, direction - transform.up, Color.red);
				
				// ... and if a raycast towards the player hits something...
				if(Physics.Raycast(transform.position + transform.up, direction - transform.up, out hit, sightRange))
				{
					// ... and if the raycast hits the player...
					if(hit.collider.CompareTag("Player"))
					{
						// ... the player is in sight.
						playerInSight = true;
						targetLocation = hit.transform;

						playerLastSeenTime = 0;
						// Set the last global sighting is the players current position.
					}
				}
			}
		}
	}

	public bool PlayerAudible(Vector3 position, Vector3 targetPosition)
	{
		if(!playerInCollider || PathRequestManager.IsInQueue(GetInstanceID()) || timeSinceAudibleCheck < AUDIBLE_COOLDOWN)
			return false;
		else
		{
			float totalDistance = int.MaxValue;
			Debug.Log("Path Request From Enemy View");
			PathRequestManager.RequestPath(position, targetPosition, (Vector3[] waypoints, bool pathFound) => {
				if(pathFound)
				{
					totalDistance = 0;
					for(int i = 0; i < waypoints.Length - 1; i++)
						totalDistance += Vector3.Distance(waypoints[i], waypoints[i+1]);
				}
			}, "A*", false, GetInstanceID());

			timeSinceAudibleCheck = 0;

			if(totalDistance < col.radius)
				return true;
			else
				return false;
		}
	}

	void OnTriggerExit (Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			playerInCollider = false;
			playerInSight = false;
		}
	}
}
