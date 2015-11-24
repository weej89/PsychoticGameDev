using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ChaseState : IEnemyState
{
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private float searchTimer = 0f;
	private Vector3 playerPos, zombiePos;



	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[4];
	private DecisionTree decisionTree;
	
	public ChaseState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
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
		Chase ();
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

		//Less than 2 away from player?
		decisions[2] = new Decision((object[] args) => {
			if(Vector3.Distance(zombiePos, playerPos) < 2)
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
			if(enemy.enemySight.PlayerAudible(zombiePos, playerPos))
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
			animation = "attack01",
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
		Debug.Log("Chase State Entered Calling for New Path");
		zombie.CallForNewPath(enemy.enemySight.targetLocation.position, "A*", true);
	}

	public void GetStateAction()
	{
		if(decisionTree.EventCompleted)
			decisionTree.StartDecisionProcess();
	}	

	private void Chase()
	{
		//Set chase target in pathfinding
		enemy.meshRendererFlag.material.color = Color.red;

		searchTimer += Time.deltaTime;

		if(zombie.TargetReached && !PathRequestManager.IsInQueue(zombie.GetInstanceID()) && Vector3.Distance(zombiePos, playerPos) > 2)
		{
			Debug.Log("Zombie ID in chase" +zombie.GetInstanceID());

			zombie.CallForNewPath(enemy.enemySight.targetLocation.position, enemy.pathfindingStrategy, true);
			Debug.Log("Path Request From Chase State");
		}

	}

	public string GetString()
	{
		return "Chase";
	}
}
