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

	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[6];

	public TargetArea targetArea;

	public PatrolState(StatePatternEnemy statePatternEnemy, HorrorAI zombie, Grid grid)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		this.grid = grid;

		MakeDecisionTree();
		SetNodes();
	}

	public void MakeDecisionTree()
	{
		//Is the distance from enemy to player < 50 meters?
		decisions[0] = (new Decision((object[]args) => {
			 if(Vector3.Distance(zombie.transform.position, zombie.target.position) < 50)
				return true;
			else
				return false;
		}) {args = {}});

		//Was the player last seen > 1 minute ago?
		decisions[1] = (new Decision((object[]args) => {
			if(Time.time - enemy.enemySight.playerLastSeenTime > 60)
				return true;
			else
				return false;
		}) {args = {}});

		//Is the player in the collider?
		decisions[2] = (new Decision((object[]args) => {
			if(enemy.enemySight.playerInCollider)
				return true;
			else
				return false;
		}) {args = {}});

		//Is the Player visible?
		decisions[3] = (new Decision((object[]args) => {
			if(enemy.enemySight.playerInSight)
				return true;
			else
				return false;
		}) {args = {}});

		//Is the player audible?
		decisions[4] = (new Decision((object[]args) => {
			if(enemy.enemySight.PlayerAudible())
				return true;
			else
				return false;
		}) {args = {}});
		
		actions[0] = new TreeAction()
		{
			pathFindingMethod = "A*",
			targetState = "Patrol",
			animation = "None"
		};

		actions[1] = new TreeAction()
		{
			action = () => {GetPatrolPoint(enemy.avgPatrolInterval);},	
			args = {}, 
			pathFindingMethod = "A*", 
			targetState = "Patrol", 
			animation = "None"
		};

		actions[2] = new TreeAction()
		{
			pathFindingMethod = "IterativeDeepening",
			targetState = "Patrol",
			animation = "None"
		};

		actions[3] = new TreeAction()
		{
			action = () => {zombie.CallForNewPath(enemy.enemySight.targetLocation.position, "A*", true);},
			pathFindingMethod = "A*",
			targetState = "Chase",
			animation = "None"
		};

		actions[4] = new TreeAction()
		{
			pathFindingMethod = "IterativeDeepening",
			targetState = "Patrol",
			animation = "None"
		};

		actions[5] = new TreeAction()
		{
			action = () => {
				if(zombie.TargetReached)
					zombie.CallForNewPath(enemy.enemySight.targetLocation.position, "A*", false );
			},
			pathFindingMethod = "A*",
			targetState = "Alert",
			animation = "None"
		};
	}

	public void SetNodes()
	{
		decisions[0].SetNodes(decisions[1], decisions[2]);
		decisions[1].SetNodes(actions[0], actions[1]);
		decisions[2].SetNodes(actions[2], decisions[3]);
		decisions[3].SetNodes(decisions[4], actions[3]);
		decisions[4].SetNodes(actions[4], actions[5]);
	
	}

	public void UpdateState()
	{
		Debug.Log("IN PATROL");
		//Look();
		Patrol();
	}

	public void OnStateEnter()
	{
		GetPatrolPoint(enemy.avgPatrolInterval);
		zombie.speed = zombie.DEFAULT_WALKING_SPEED;
	}
	
	public void OnTriggerEnter(Collider other)
	{
		//if(other.gameObject.CompareTag("Player"))
			//ToAlertState();
	}

	public TreeAction GetStateAction()
	{
		return decisions[0].MakeDecision(decisions[0].GetBranch());
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
		/*
		RaycastHit hit;

		if(Physics.Raycast(enemy.eyes.transform.position, enemy.eyes.transform.forward, out hit, enemy.sightRange) && hit.collider.CompareTag("Player"))
		{
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

	void Patrol()
	{
		enemy.meshRendererFlag.material.color = Color.green;
		//Add patrolling code here for pathfinding
		//Add decision tree stuff here for determining behaviors
		if(zombie.TargetReached && currentTime < patrolTime && PathRequestManager.IsProcessingPath == false)
		{
			Vector3 target = targetArea.GenerateCheckingPath();
					
			zombie.CallForNewPath(target, "IterativeDeepening", false);
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

	public string GetString()
	{
		return "Patrol";
	}
}
