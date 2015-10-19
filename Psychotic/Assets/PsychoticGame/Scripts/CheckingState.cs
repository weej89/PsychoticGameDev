using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckingState : IEnemyState 
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private float checkingTime;
	private List<Node> nodesToCheck;
	private int pathIndex = 1;
	
	public CheckingState(StatePatternEnemy statePatternEnemy, HorrorAI zombie, float checkingTime, List<Node> nodesToCheck)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		this.checkingTime = checkingTime;
		this.nodesToCheck = nodesToCheck;
	}
	
	public void UpdateState()
	{
		Debug.Log(nodesToCheck.Count);
		Checking();
		Look();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
			ToAlertState();
	}
	
	public void ToPatrolState()
	{
		enemy.GeneratePatrolPath();
		enemy.currentState = enemy.patrolState;
		pathIndex = 1;
	}

	public void ToCheckingState(float interval)
	{
		Debug.Log("Can't transition to same state");
	}
	
	public void ToAlertState()
	{
		enemy.currentState = enemy.alertState;
		pathIndex = 1;
	}
	
	public void ToChaseState()
	{
		enemy.currentState = enemy.chaseState;
		pathIndex = 1;
	}
	
	private void Look()
	{
		RaycastHit hit;
		
		if(Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
			enemy.chaseTarget = hit.transform;
			ToChaseState();
		}
	}

	public void Checking()
	{
		if(zombie.targetReached && pathIndex < nodesToCheck.Count)
		{
			zombie.CallForNewPath(nodesToCheck[pathIndex].worldPosition);
			pathIndex++;
		}
		else if(zombie.targetReached && pathIndex >= nodesToCheck.Count)
		{
			ToPatrolState();
		}
	}

	public void OnDrawGizmos ()
	{
		foreach(Node node in nodesToCheck)
		{
			Gizmos.color = Color.yellow;
			Gizmos.DrawCube(node.worldPosition, Vector3.one);
		}
	}
}
