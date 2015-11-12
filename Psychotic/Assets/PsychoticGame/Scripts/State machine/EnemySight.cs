using UnityEngine;
using System.Collections;

public class EnemySight : MonoBehaviour 
{
	public float fieldOfViewAngle = 110f;
	public bool playerInSight;
	public float sightRange = 20f;
	public Transform targetLocation;


	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.CompareTag("Player")) 
		{
			playerInSight = false;
			targetLocation = null;

			Vector3 direction = other.transform.position - transform.position;
			float angle = Vector3.Angle(direction, transform.forward);

			if(angle < fieldOfViewAngle * 0.5f)
			{
				RaycastHit hit;

				if(Physics.Raycast(transform.position + transform.up, direction.normalized, out hit, sightRange) && hit.collider.CompareTag("Player"))
				{
					playerInSight = true;
					targetLocation = hit.transform;
				}
			}
		}
	}
}
