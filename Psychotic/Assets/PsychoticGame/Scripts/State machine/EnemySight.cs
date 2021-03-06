using UnityEngine;
using System;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float fieldOfViewAngle = 110f;
	public float sightRange = 35f;
	public double AudibleRange;
	public Vector3 offset = new Vector3 (0, .5f, 0);
	public Transform eyes;
	
	[HideInInspector] public Transform targetLocation;
	[HideInInspector] public bool playerInSight;
	[HideInInspector] public bool playerInCollider;
	[HideInInspector] public float playerLastSeenTime;
	[HideInInspector] public Vector3 playerLastHearLocation;
	[HideInInspector] public bool playerAudible = false;
	[HideInInspector]public bool objectInFront = false;

	private SphereCollider col;
	private HorrorAI ai;
	private float timeSinceAudibleCheck = 0;
	private const int AUDIBLE_COOLDOWN = 4000;
	private float objectRange = 2.0f;

	// Use this for initialization
	void Start () 
	{
		col = GetComponent<SphereCollider>();
		ai = GetComponent<HorrorAI>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeSinceAudibleCheck+=Time.deltaTime;
		playerLastSeenTime += Time.deltaTime;

	}

	void OnTriggerStay (Collider other)
	{
		objectInFront = CheckForObjectInPath(other);

		// If the player has entered the trigger sphere...
		if(other.gameObject.CompareTag("Player"))
		{
			CheckForPlayerSight(other);

			if(timeSinceAudibleCheck < AUDIBLE_COOLDOWN)
			{
				PlayerAudible(ai.transform.position, ai.target.position);
			}
		}
	}

	private void CheckForPlayerSight(Collider other)
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
					return;
				}
			}
		}
	}

	private void PlayerAudible(Vector3 position, Vector3 targetPosition)
	{
		timeSinceAudibleCheck = 0;

		if(!playerInCollider || PathRequestManager.IsInQueue(GetInstanceID()))
			return;
		else
		{
			Debug.Log("Path Request From Enemy View");

			PathRequestManager.RequestPath(position, targetPosition, (Vector3[] waypoints, bool pathFound) => {
				StartCoroutine(GetPathDistance(waypoints, pathFound));
			}, "A*", false, GetInstanceID());
		}
	}

	private IEnumerator GetPathDistance(Vector3[] waypoints, bool pathFound)
	{
		playerAudible = false;

		if(pathFound)
		{
			double totalDistance = 0;

			for(int i = 0; i < waypoints.Length - 1; i++)
				totalDistance += Vector3.Distance(waypoints[i], waypoints[i+1]);

			yield return null;

			if(totalDistance <= AudibleRange)
			{
				playerLastHearLocation = waypoints[waypoints.Length-1];
				playerAudible = true;
			}
		}

	}

	void OnTriggerExit (Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
		{
			playerInCollider = false;
			playerInSight = false;
			playerAudible = false;
		}

		if(other.gameObject.CompareTag("Door"))
		{

		}
	}

	bool CheckForObjectInPath(Collider obj)
	{
		if(obj.CompareTag("Door"))
		{
			Vector3 direction = obj.transform.position - transform.position;

			RaycastHit hit;

			if(Physics.Raycast(transform.position + transform.up, direction - transform.up, out hit, objectRange))
			{
				return true;
			}
		}

		return false;
	}
}
