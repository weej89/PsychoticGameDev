using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float fieldOfViewAngle = 110f;
	public bool playerInSight;
	public float sightRange = 35f;
	public Vector3 offset = new Vector3 (0, .5f, 0);
	public Transform eyes;
	[HideInInspector] public Transform targetLocation;

	private SphereCollider col;

	// Use this for initialization
	void Start () 
	{
		col = GetComponent<SphereCollider>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnTriggerStay (Collider other)
	{
		// If the player has entered the trigger sphere...
		if(other.gameObject.CompareTag("Player"))
		{
			// By default the player is not in sight.
			playerInSight = false;
			
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
						
						// Set the last global sighting is the players current position.
					}
				}
			}
		}
	}

	void OnTriggerExit (Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
			playerInSight = false;
	}
}
