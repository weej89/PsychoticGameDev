using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CheckingState : IEnemyState 
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private float checkingTime;
	
	public CheckingState(StatePatternEnemy statePatternEnemy, HorrorAI zombie, float checkingTime)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		this.checkingTime = checkingTime;
	}
	
	public void UpdateState()
	{
		Look();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
			ToAlertState();
	}
	
	public void ToPatrolState()
	{

	}

	public void ToCheckingState(float interval, List<Node> nodesToCheck)
	{
		Debug.Log("Can't transition to same state");
	}
	
	public void ToAlertState()
	{
		enemy.currentState = enemy.alertState;
	}
	
	public void ToChaseState()
	{
		enemy.currentState = enemy.chaseState;
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
}
