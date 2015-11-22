using UnityEngine;
using System.Collections;

public class EnemyAliveScript : MonoBehaviour 
{
	GameObject[] enemy;
	GameObject audioObject;
	AudioSource sound;

	void Awake()
	{
		enemy = GameObject.FindGameObjectsWithTag("Enemy");
		audioObject = GameObject.FindGameObjectWithTag("Zombie_Echo");
		sound = audioObject.GetComponent<AudioSource>();

		foreach(GameObject obj in enemy)
			obj.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
		{
			foreach(GameObject obj in enemy)
				obj.SetActive(true);

			sound.Play();
			this.gameObject.SetActive(false);
		}
	}


}
