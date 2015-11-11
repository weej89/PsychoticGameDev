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
		Debug.Log("IN CHASE");
		Look();
		Chase ();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		
	}
	
	public void ToPatrolState()
	{
		
	}

	public void ToAlertState()
	{
		enemy.currentState = enemy.alertState;
		zombie.speed = 0;
	}
	
	public void ToChaseState()
	{
		Debug.Log("Can't transition to same state");
	}

	private void Look()
	{
		RaycastHit hit;
		Vector3 enemyToTarget = (enemy.chaseTarget.position + enemy.offset) - enemy.eyes.transform.position;
		Debug.DrawRay(enemy.eyes.transform.position, enemyToTarget, Color.red);
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
		enemy.meshRendererFlag.material.color = Color.red;

		if(zombie.TargetReached && PathRequestManager.IsProcessingPath == false)
			zombie.CallForNewPath(enemy.chaseTarget.transform.position, "A*", true);

	}
}
