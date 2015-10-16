using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private float searchTimer = 0f;
	
	public AlertState(StatePatternEnemy statePatternEnemy)
	{
		this.enemy = statePatternEnemy;
	}

	public void UpdateState()
	{
		Look();
	}
	
	public void OnTriggerEnter(Collider other)
	{

	}
	
	public void ToPatrolState()
	{
		enemy.currentState = enemy.patrolState;
		searchTimer = 0f;
	}

	public void ToCheckingState(float interval, List<Node> nodesToCheck)
	{
		
	}
	
	public void ToAlertState()
	{
		Debug.Log("Can't transition to same state");
	}
	
	public void ToChaseState()
	{
		enemy.currentState = enemy.chaseState;
		searchTimer = 0f;
	}

	private void Look()
	{
		RaycastHit hit;
		
		if(Physics.Raycast(enemy.eyes.transform.position, enemy.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
			enemy.chaseTarget = hit.transform;
			ToChaseState();
		}
	}

	private void Search()
	{
		//Add new pathfinding stuff
		//Rotate enemy towards player
		searchTimer += Time.deltaTime;

		if(searchTimer >= enemy.searchingDuration)
			ToPatrolState();
	}
}
