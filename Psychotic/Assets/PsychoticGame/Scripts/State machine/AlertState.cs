using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlertState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private float searchTimer = 0f;
	private HorrorAI zombie;
	private Vector3 playerPos, zombiePos;
	
	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[4];

	private DecisionTree decisionTree;
	
	public AlertState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		
		MakeDecisionTree();
		SetNodes();

		decisionTree = new DecisionTree(decisions, actions, enemy.AddActionToQueue);
	}
	
	public void UpdateState()
	{
		playerPos = zombie.target.position;
		zombiePos = zombie.transform.position;
		Search();
	}
	
	public void GetStateAction()
	{
		if(decisionTree.EventCompleted)
			decisionTree.StartDecisionProcess();
	}

	public void MakeDecisionTree()
	{
		//Is the search timer up
		decisions[0] = new Decision((object[] args) => {
			if(searchTimer > enemy.searchingDuration)
				return true;
			else
				return false;
		});
		
		//Is Player In Collider
		decisions[1] = new Decision((object[] args) => {
			if(enemy.enemySight.playerInCollider)
				return true;
			else
				return false;
		});
		
		//Is player less than 10 meters away
		decisions[2] = new Decision((object[] args) => {
			if(Vector3.Distance(zombiePos, playerPos) < 10)
				return true;
			else
				return false;
		});
		
		//Is the Player visible?
		decisions[3] = new Decision((object[]args) => {
			if(enemy.enemySight.playerInSight)
				return true;
			else
				return false;
		});
		
		//Is the Player Audible?
		decisions[4] = new Decision((object[] args) => {
			if(enemy.enemySight.PlayerAudible(zombiePos, playerPos))
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

	private void SetNodes()
	{
		decisions[0].SetNodes(decisions[1], decisions[2]);
		decisions[1].SetNodes(null, decisions[3]);
		decisions[2].SetNodes(actions[0], actions[1]);
		decisions[3].SetNodes(decisions[4], actions[2]);
		decisions[4].SetNodes(null, actions[3]);
	}
	
	public void OnStateEnter()
	{
		searchTimer = 0f;
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
