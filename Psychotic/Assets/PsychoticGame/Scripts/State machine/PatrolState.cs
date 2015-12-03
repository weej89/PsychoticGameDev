#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class PatrolState : IEnemyState
{
	#region Private Variables
	private readonly StatePatternEnemy enemy;
	private HorrorAI zombie;
	private double patrolTime;
	private double currentTime = 0;
	private Grid grid;

	private Vector3 playerPos, zombiePos;

	private Decision[] decisions = new Decision[6];
	private TreeAction[] actions = new TreeAction[7];
	private DecisionTree decisionTree;
	#endregion

	#region Public Variables
	public TargetArea targetArea;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="PatrolState"/> class.
	/// </summary>
	/// <param name="statePatternEnemy">State pattern enemy.</param>
	/// <param name="zombie">Zombie.</param>
	/// <param name="grid">Grid.</param>
	public PatrolState(StatePatternEnemy statePatternEnemy, HorrorAI zombie, Grid grid)
	{
		this.enemy = statePatternEnemy;
		this.zombie = zombie;
		this.grid = grid;
		
		MakeDecisionTree();
		SetNodes();

		decisionTree = new DecisionTree(decisions, actions, enemy.AddActionToQueue);
	}
	#endregion

	#region MakeDecisionTree
	/// <summary>
	/// Makes the decision tree.
	/// </summary>
	public void MakeDecisionTree()
	{
		//Is there an object in front of me?
		decisions[0] = (new Decision((object[] args) => {
			if(enemy.enemySight.objectInFront)
				return true;
			else
				return false;
		}));
		//Is the distance from enemy to player < 50 meters?
		decisions[1] = (new Decision((object[]args) => {
			 if(Vector3.Distance(zombiePos, playerPos) < 50)
				return true;
			else
				return false;
		}));

		//Was the player last seen >  30 secs ago?
		decisions[2] = (new Decision((object[]args) => {
			if(enemy.enemySight.playerLastSeenTime > 30)
				return true;
			else
				return false;
		}));

		//Is the player in the collider?
		decisions[3] = (new Decision((object[]args) => {
			if(enemy.enemySight.playerInCollider)
				return true;
			else
				return false;
		}));

		//Is the Player visible?
		decisions[4] = (new Decision((object[]args) => {
			if(enemy.enemySight.playerInSight)
				return true;
			else
				return false;
		}));

		//Is the player audible?
		decisions[5] = (new Decision((object[]args) => {
			if(enemy.enemySight.playerAudible)
				return true;
			else
				return false;
		}));

		actions[0] = new TreeAction()
		{
			pathFindingMethod = "DynamicBiDirectional",
			targetState = "Patrol",
			animation = "attack02"
		};

		actions[1] = new TreeAction()
		{
			pathFindingMethod = "DynamicBiDirectional",
			targetState = "Patrol",
			animation = "None"
		};

		actions[2] = new TreeAction()
		{
			action = () => {GetPatrolPoint(enemy.avgPatrolInterval);},	
			args = {},
			pathFindingMethod = "DynamicBiDirectional", 
			targetState = "Patrol", 
			animation = "None"
		};

		actions[3] = new TreeAction()
		{
			pathFindingMethod = "DynamicBiDirectional",
			targetState = "Patrol",
			animation = "None"
		};

		actions[4] = new TreeAction()
		{
			pathFindingMethod = "DynamicBiDirectional",
			targetState = "Chase",
			animation = "None"
		};

		actions[5] = new TreeAction()
		{
			pathFindingMethod = "DynamicBiDirectional",
			targetState = "Patrol",
			animation = "None"
		};

		actions[6] = new TreeAction()
		{
			action = () => {
				if(zombie.TargetReached)
				zombie.CallForNewPath(enemy.enemySight.targetLocation.position, "DynamicBiDirectional", false );
			},
			pathFindingMethod = "A*",
			targetState = "Alert",
			animation = "None"
		};
	}
	#endregion

	#region SetNodes
	/// <summary>
	/// Sets the nodes for each decision in tree
	/// </summary>
	public void SetNodes()
	{
		decisions[0].SetNodes(decisions[1], actions[0]);
		decisions[1].SetNodes(decisions[2], decisions[3]);
		decisions[2].SetNodes(actions[1], actions[0]);
		decisions[3].SetNodes(actions[3], decisions[4]);
		decisions[4].SetNodes(decisions[5], actions[4]);
		decisions[5].SetNodes(actions[5], actions[6]);
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

		Patrol();
	}
	#endregion

	#region OnStateEnter
	/// <summary>
	/// Raises the state enter event.
	/// </summary>
	public void OnStateEnter()
	{
		GetPatrolPoint(enemy.avgPatrolInterval);
		zombie.speed = zombie.DEFAULT_WALKING_SPEED;
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

	#region GetPatrolPoint
	/// <summary>
	/// Gets the new patrol point.
	/// </summary>
	/// <param name="avgInterval">Avg interval.</param>
	public void GetPatrolPoint(double avgInterval)
	{
		patrolTime = GetNextRandomInterval(avgInterval) * avgInterval;
		targetArea = new TargetArea(grid, zombie.target.position, enemy.targetAreaRadius);
		currentTime = 0;
	}
	#endregion

	#region Patrol
	/// <summary>
	/// Performs the actions specific to patrolling state
	/// </summary>
	void Patrol()
	{
		//Used for keeping track of state
		enemy.meshRendererFlag.material.color = Color.green;

		//If the current target has been reached and patrol time for this current location hasn't ended and AI isn't waiting for path processing
		if(zombie.TargetReached && currentTime < patrolTime && !PathRequestManager.IsInQueue(zombie.GetInstanceID()))
		{
			//Call for a new path from checking area
			Vector3 target = targetArea.GenerateCheckingPath();
					
			zombie.CallForNewPath(target, enemy.pathfindingStrategy, false);

		}
		//Else generate a new checking area
		else if (currentTime > patrolTime)
		{
			GetPatrolPoint(enemy.avgPatrolInterval);
		}

		//Increment the current time for checking area expiration
		currentTime += Time.deltaTime;
	}
	#endregion

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

	#region GetString
	/// <summary>
	/// Gets the current states string representation
	/// </summary>
	/// <returns>The string.</returns>
	public string GetString()
	{
		return "Patrol";
	}
	#endregion
}
