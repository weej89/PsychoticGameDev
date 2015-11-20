using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private float searchTimer = 0f;
	private HorrorAI zombie;
	
	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[4];
	
	public AlertState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		
		MakeDecisionTree();
	}
	
	public void UpdateState()
	{
		Debug.Log("IN ALERT");
		//Look();
		Search();
	}
	
	public TreeAction GetStateAction()
	{
		return decisions[0].MakeDecision(decisions[0].GetBranch());
	}
	
	public void OnTriggerEnter(Collider other)
	{
		
	}
	
	public void MakeDecisionTree()
	{
		//Is the search timer up
		decisions[0] = new Decision(decisions[1], decisions[2], (object[] args) => {
			if(searchTimer > enemy.searchingDuration)
				return true;
			else
				return false;
		});
		
		//Is Player In Collider
		decisions[1] = new Decision(null, decisions[3], (object[] args) => {
			if(enemy.enemySight.playerInCollider)
				return true;
			else
				return false;
		});
		
		//Is player less than 10 meters away
		decisions[2] = new Decision(actions[0], actions[1], (object[] args) => {
			if(Vector3.Distance(zombie.transform.position, zombie.target.position) < 10)
				return true;
			else
				return false;
		});
		
		//Is the Player visible?
		decisions[3] = new Decision(decisions[4], actions[2], (object[]args) => {
			if(enemy.enemySight.playerInSight)
				return true;
			else
				return false;
		});
		
		//Is the Player Audible?
		decisions[4] = new Decision(null, actions[3], (object[] args) => {
			if(enemy.enemySight.PlayerAudible())
				return true;
			else
				return false;
		});
		
		actions[0] = new TreeAction()
		{
			action = () => {enemy.patrolState.GetPatrolPoint(enemy.avgPatrolInterval);},
			targetState = "Patrol",
			pathFindingMethod = "A*",
			animation = "None"
		};
		
		actions[1] = new TreeAction()
		{
			action = () => {enemy.patrolState.GetPatrolPoint(enemy.avgPatrolInterval);},
			targetState = "Patrol",
			pathFindingMethod = "IterativeDeepening",
			animation = "None"
		};
		
		actions[2] = new TreeAction()
		{
			targetState = "Chase",
			pathFindingMethod = "A*",
			animation = "None"
		};
		
		actions[3] = new TreeAction()
		{
			targetState = "Alert",
			pathFindingMethod = "A*",
			animation = "None",
			action = () => {
				if(zombie.TargetReached)
				zombie.CallForNewPath(enemy.enemySight.targetLocation.position, "A*", false );
			}
		};
		
	}
	
	public void OnStateEnter()
	{
		searchTimer = 0f;
	}
	
	
	public void ToPatrolState()
	{
		enemy.patrolState.GetPatrolPoint(enemy.patrolState.GetNextRandomInterval(enemy.avgPatrolInterval));
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
		/*
		RaycastHit hit;
		
		if(Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
			Debug.DrawLine(enemy.eyes.transform.position, new Vector3(enemy.eyes.transform.forward.x, enemy.eyes.transform.forward.y, enemy.eyes.transform.forward.z + enemy.sightRange));

			enemy.chaseTarget = hit.transform;
			zombie.CallForNewPath(enemy.chaseTarget.transform.position, "A*", true);

			ToChaseState();
		}
		*/
		if(enemy.enemySight.playerInSight)
		{
			enemy.chaseTarget = enemy.enemySight.targetLocation;
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
		
		/*
		if(searchTimer >= enemy.searchingDuration)
			ToPatrolState();
			*/
	}
	
	public string GetString()
	{
		return "Alert";
	}
}
