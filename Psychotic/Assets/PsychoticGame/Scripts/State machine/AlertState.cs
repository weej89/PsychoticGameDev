using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private float searchTimer = 0f;
	private HorrorAI zombie;
	
	public AlertState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
	}

	public void UpdateState()
	{
		Debug.Log("IN ALERT");
		Look();
		Search();
	}
	
	public void OnTriggerEnter(Collider other)
	{

	}
	
	public void ToPatrolState()
	{
		enemy.patrolState.GetNextRandomInterval(enemy.avgPatrolInterval);
		enemy.currentState = enemy.patrolState;
		searchTimer = 0f;
		zombie.speed = zombie.DEFAULT_WALKING_SPEED;
	}
	
	public void ToAlertState()
	{
		Debug.Log("Can't transition to same state");
	}
	
	public void ToChaseState()
	{
		enemy.currentState = enemy.chaseState;
		searchTimer = 0f;
		zombie.speed = zombie.DEFUALT_RUNNING_SPEED;
	}

	private void Look()
	{
		RaycastHit hit;
		
		if(Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
			Debug.DrawLine(enemy.eyes.transform.position, new Vector3(enemy.eyes.transform.forward.x, enemy.eyes.transform.forward.y, enemy.eyes.transform.forward.z + enemy.sightRange));

			enemy.chaseTarget = hit.transform;
			zombie.CallForNewPath(enemy.chaseTarget.transform.position, "A*", true);

			ToChaseState();
		}
	}

	private void Search()
	{
		//Add new pathfinding stuff
		//Rotate enemy towards player
		enemy.meshRendererFlag.material.color = Color.yellow;

		searchTimer += Time.deltaTime;

		if(searchTimer >= enemy.searchingDuration)
			ToPatrolState();
	}
}
