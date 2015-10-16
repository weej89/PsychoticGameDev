using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;

	public PatrolState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
	}

	public void UpdateState()
	{
		Look();
		Patrol();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.CompareTag("Player"))
			ToAlertState();
	}
	
	public void ToPatrolState()
	{
		Debug.Log("Can't transition to same state");
	}

	public void ToCheckingState(float interval, List<Node> nodesToCheck)
	{

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

	void Patrol()
	{
		//Add patrolling code here for pathfinding
		//Add decision tree stuff here for determining behaviors
		if(zombie.targetReached)
		{
			zombie.interval = GetNextRandomInterval(zombie.newPathAvgTime);
		}
	}

	#region GetNextRandomInterval
	/// <summary>
	/// Returns a Random interval of time according to a given
	/// average using the Poisson Distribution Equation
	/// </summary>
	/// <returns>The next random interval.</returns>
	/// <param name="avg">Avg.</param>
	public double GetNextRandomInterval (double avg)
	{
		avg = (1 / avg);
		double interval = -Mathf.Log ((float)(1.0 - Random.value)) / avg;
		return interval;
	}
	#endregion
}
