using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private float searchTimer = 0f;


	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[4];
	
	public ChaseState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;

		MakeDecisionTree();
		SetNodes();
	}

	public void UpdateState()
	{
		Debug.Log("IN CHASE");
		//Look();
		Chase ();
	}
	
	public void OnTriggerEnter(Collider other)
	{
		
	}

	public void MakeDecisionTree()
	{
		//Is enemy in sight?
		decisions[0] = new Decision((object[] args)=>{
			if(enemy.enemySight.playerInSight)
				return true;
			else
				return false;
		});

		//Is search timer up?
		decisions[1] = new Decision((object[] args) => {
			if(searchTimer >= enemy.searchingDuration)
				return true;
			else
				return false;
		});

		//Less than 1 away from player?
		decisions[2] = new Decision((object[] args) => {
			Debug.Log("Distance:  " +Vector3.Distance(zombie.transform.position, zombie.target.position));
			if(Vector3.Distance(zombie.transform.position, zombie.target.position) < 2)
				return true;
			else
				return false;
		});

		//Is the enemy in the collider?
		decisions[3] = new Decision((object[] args) => {
			if(enemy.enemySight.playerInCollider)
				return true;
			else
				return false;
		});

		//Is the enemy audible?
		decisions[4] = new Decision((object[] args) => {
			if(enemy.enemySight.PlayerAudible())
				return true;
			else
				return false;
		});

		actions[0] = new TreeAction()
		{
			pathFindingMethod = "A*",
			targetState = "Alert",
			animation = "None"
		};

		actions[1] = new TreeAction()
		{
			action = () => {searchTimer = 0f;},
			targetState = "Chase",
			animation = "None",
			pathFindingMethod = "A*"
		};

		actions[2] = new TreeAction()
		{
			action = () => {searchTimer = 0f;},
			targetState = "Chase",
			animation = "Attack",
			pathFindingMethod = "A*"
		};

		actions[3] = new TreeAction()
		{
			action = () => {
				if(zombie.TargetReached)
				zombie.CallForNewPath(enemy.enemySight.targetLocation.position, "A*", false );
			},
			targetState = "Chase",
			animation = "None",
			pathFindingMethod = "A*"
		};
	}

	private void SetNodes()
	{
		decisions[0].SetNodes(decisions[1], decisions[2]);
		decisions[1].SetNodes(decisions[3], actions[0]);
		decisions[2].SetNodes(actions[1], actions[2]);
		decisions[3].SetNodes(null, decisions[4]);
		decisions[4].SetNodes(null, actions[3]);
	}

	public void OnStateEnter()
	{
		searchTimer = 0f;
		zombie.speed = zombie.DEFUALT_RUNNING_SPEED;
	}

	public TreeAction GetStateAction()
	{
		return decisions[0].MakeDecision(decisions[0].GetBranch());
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
		/*RaycastHit hit;
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
		*/
		if (enemy.enemySight.playerInSight) 
		{
			enemy.chaseTarget = enemy.enemySight.targetLocation;
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

		searchTimer += Time.deltaTime;

		if(zombie.TargetReached && PathRequestManager.IsProcessingPath == false)
			zombie.CallForNewPath(enemy.enemySight.targetLocation.position, enemy.pathfindingStrategy, true);

	}

	public string GetString()
	{
		return "Chase";
	}
}
