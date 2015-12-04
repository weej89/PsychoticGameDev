#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class ChaseState : IEnemyState
{
	#region Private Variables
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private float searchTimer = 0f;
	private Vector3 playerPos, zombiePos;
	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[4];
	private DecisionTree decisionTree;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="ChaseState"/> class.
	/// </summary>
	/// <param name="statePatternEnemy">State pattern enemy.</param>
	/// <param name="zombie">Zombie.</param>
	public ChaseState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;

		MakeDecisionTree();
		SetNodes();

		decisionTree = new DecisionTree(decisions, actions, enemy.AddActionToQueue);
	}
	#endregion

	#region UpdateState
	/// <summary>
	/// Updates the state.
	/// </summary>
	public void UpdateState()
	{
		playerPos = zombie.target.position;
		zombiePos = zombie.transform.position;
		Chase ();
	}
	#endregion

	#region MakeDecisionTree
	/// <summary>
	/// Makes the decision tree.
	/// </summary>
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
			if(enemy.enemySight.playerAudible)
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
	#endregion

	#region SetNodes
	/// <summary>
	/// Sets the nodes in decision tree
	/// </summary>
	private void SetNodes()
	{
		decisions[0].SetNodes(decisions[1], decisions[2]);
		decisions[1].SetNodes(decisions[3], actions[0]);
		decisions[2].SetNodes(actions[1], actions[2]);
		decisions[3].SetNodes(null, decisions[4]);
		decisions[4].SetNodes(null, actions[3]);
	}
	#endregion

	#region OnStateEnter
	/// <summary>
	/// Raises the state enter event.
	/// </summary>
	public void OnStateEnter()
	{
		searchTimer = 0f;
		zombie.speed = enemy.RUNNING;
		zombie.CallForNewPath(enemy.enemySight.targetLocation.position, enemy.pathfindingStrategy, true);

		if(enemy.walkingClip.isPlaying)
			enemy.walkingClip.Stop();

		enemy.chasingClip.Play();
	}
	#endregion

	#region GetStateAction
	/// <summary>
	/// Gets the state action.
	/// </summary>
	public void GetStateAction()
	{
		if(decisionTree.EventCompleted)
			decisionTree.StartDecisionProcess();
	}	
	#endregion

	#region Chase
	/// <summary>
	/// Perform actions central to chase state
	/// </summary>
	private void Chase()
	{
		//Set chase target in pathfinding
		enemy.meshRendererFlag.material.color = Color.red;

		searchTimer += Time.deltaTime;

		if(zombie.TargetReached && !PathRequestManager.IsInQueue(zombie.GetInstanceID()) && Vector3.Distance(zombiePos, playerPos) > 2)
		{
			zombie.CallForNewPath(enemy.enemySight.targetLocation.position, enemy.pathfindingStrategy, true);
		}
	}
	#endregion

	#region GetString
	/// <summary>
	/// Returns string representation of this state
	/// </summary>
	/// <returns>The string.</returns>
	public string GetString()
	{
		return "Chase";
	}
	#endregion
}
