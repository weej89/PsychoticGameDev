#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class AlertState : IEnemyState
{
	#region Private Variables
	private readonly StatePatternEnemy enemy;
	private float searchTimer = 0f;
	private HorrorAI zombie;
	private Vector3 playerPos, zombiePos;
	
	private Decision[] decisions = new Decision[5];
	private TreeAction[] actions = new TreeAction[4];

	private DecisionTree decisionTree;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="AlertState"/> class.
	/// </summary>
	/// <param name="statePatternEnemy">State pattern enemy.</param>
	/// <param name="zombie">Zombie.</param>
	public AlertState(StatePatternEnemy statePatternEnemy, HorrorAI zombie)
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
		Search();
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

	#region MakeDecisionTree
	/// <summary>
	/// Makes the decision tree.
	/// </summary>
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
			if(enemy.enemySight.playerAudible)
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
	#endregion

	#region SetNodes
	/// <summary>
	/// Sets the nodes.
	/// </summary>
	private void SetNodes()
	{
		decisions[0].SetNodes(decisions[1], decisions[2]);
		decisions[1].SetNodes(null, decisions[3]);
		decisions[2].SetNodes(actions[0], actions[1]);
		decisions[3].SetNodes(decisions[4], actions[2]);
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
	}
	#endregion

	#region Search
	/// <summary>
	/// Performs actions specific to the alert state
	/// </summary>
	private void Search()
	{
		enemy.meshRendererFlag.material.color = Color.yellow;
		
		searchTimer += Time.deltaTime;
	}
	#endregion

	#region GetString
	/// <summary>
	/// Returns string representation of this state
	/// </summary>
	/// <returns>The string.</returns>
	public string GetString()
	{
		return "Alert";
	}
	#endregion
}
