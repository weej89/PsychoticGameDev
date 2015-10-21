using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	
	public ChaseState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
	}

	public void UpdateState()
	{
		Look();
		Chase ();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		
	}
	
	public void ToPatrolState()
	{
		
	}

	public void ToCheckingState(float interval)
	{
		
	}

	public void ToAlertState()
	{
		enemy.currentState = enemy.alertState;
	}
	
	public void ToChaseState()
	{
		Debug.Log("Can't transition to same state");
	}

	private void Look()
	{
		RaycastHit hit;
		Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
		if(Physics.Raycast(enemy.eyes.transform.position, enemyToTarget, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
			enemy.chaseTarget = hit.transform;
		}
		else
		{
			ToAlertState();
		}
	}

	private void Chase()
	{
		//Set chase target in pathfinding
		if(zombie.targetReached)
			zombie.CallForNewPath(enemy.chaseTarget.transform.position);
	}
}
