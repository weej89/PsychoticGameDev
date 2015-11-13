using UnityEngine;
using System.Collections;

public class EnemyAliveScript : MonoBehaviour 
{
	GameObject enemy;

	void Awake()
	{
		enemy = GameObject.FindGameObjectWithTag("Enemy");
		enemy.SetActive(false);
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.CompareTag("Player"))
			enemy.SetActive(true);
	}


}
