using UnityEngine;
using System.Collections;

public class Durability : MonoBehaviour {

	public Rigidbody Body;
	public Collider Col;
	public int Life;
	public float Speed;

	private Vector3 damagePosition;
	private OpeningDoor doorScript;
	private Collider[] colliders;

	// Use this for initialization
	void Start () {
		doorScript = this.GetComponent<OpeningDoor>();
		colliders = this.GetComponents<Collider>();
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Weapon"))
		{
			this.Life -= 10;
			damagePosition = other.transform.position;

			if(Life <= 0)
			{
				doorScript.enabled = false;

				Body.constraints = RigidbodyConstraints.None;
				Vector3 direction = other.gameObject.transform.forward;
				direction.Normalize();
				Body.AddForce(direction * Speed);
			}
		}
	}

}
