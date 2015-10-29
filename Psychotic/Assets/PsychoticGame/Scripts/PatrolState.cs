using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PatrolState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private double patrolTime;
	private double currentTime = 0;
	private Grid grid;

	public TargetArea targetArea;

	public PatrolState(StatePatternEnemy statePatternEnemy, HorrorAI zombie, Grid grid)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		this.grid = grid;
	}

	public void UpdateState()
	{
		Debug.Log("IN PATROL");
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
	
	public void ToAlertState()
	{
		enemy.currentState = enemy.alertState;
		zombie.speed = 0;
	}
	
	public void ToChaseState()
	{
		enemy.currentState = enemy.chaseState;
		zombie.speed = zombie.DEFUALT_RUNNING_SPEED;
	}

	public void GetPatrolPoint(double avgInterval)
	{
		patrolTime = GetNextRandomInterval(avgInterval) * avgInterval;
		targetArea = new TargetArea(grid, zombie.target.position, enemy.targetAreaRadius);
		currentTime = 0;
	}

	private void Look()
	{
		RaycastHit hit;
		
		if(Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
			enemy.chaseTarget = hit.transform;
			zombie.CallForNewPath(enemy.chaseTarget.transform.position);
			ToChaseState();
		}
	}

	void Patrol()
	{
		enemy.meshRendererFlag.material.color = Color.green;
		//Add patrolling code here for pathfinding
		//Add decision tree stuff here for determining behaviors
		if(zombie.TargetReached && currentTime < patrolTime && PathRequestManager.IsProcessingPath == false)
		{
				Vector3 target = targetArea.GenerateCheckingPath();
				
			/*
				PathRequestManager.RequestPath(enemy.transform.position, target, (Vector3[] path, bool success) => {
					if(success == true)
					{
						Debug.Log("Path found!");
						zombie.OnPathFound(path, true);
					}
					else
						Debug.Log("Path Not Found!");
				});
			*/
			PathRequestManager.RequestPath(zombie.transform.position, target, zombie.OnPathFound);
		}
		else if (currentTime > patrolTime)
		{
			GetPatrolPoint(enemy.avgPatrolInterval);
		}
		
		currentTime += Time.deltaTime;
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
