#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
#endregion

public class StatePatternEnemy : MonoBehaviour
{
	#region Public Variables
	public float searchingTurnSpeed = 120f;
	public float searchingDuration = 4f;
	public float sightRange = 20f;
	public double avgPatrolInterval = 15;
	public Transform[] wayPoints;
	public Transform eyes;
	public Vector3 offset = new Vector3 (0, .5f, 0);
	public MeshRenderer meshRendererFlag;
	public TargetArea targetArea;
	public float targetAreaRadius = 50f;
	public Transform seeker;

	public GameObject pathFinding;
	#endregion

	#region Public Hidden Variables
	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public IEnemyState currentState;
	[HideInInspector] public ChaseState chaseState;
	[HideInInspector] public AlertState alertState;
	[HideInInspector] public PatrolState patrolState;
	[HideInInspector] public AnimationController anim;
	[HideInInspector] public EnemySight enemySight;

	[HideInInspector] public string pathfindingStrategy;
	#endregion

	#region Private Variables
	private HorrorAI enemy;
	private Grid grid;
	private bool playingTriggerAnimation = false;
	private float decisionTime = 0;
	private const float DECISION_COOLDOWN = .1f;
	private Queue<TreeAction> processedActions;
	#endregion

	#region Awake
	/// <summary>
	/// Awake this instance.
	/// </summary>
	private void Awake()
	{
		GameObject pathFinding = GameObject.Find("Pathfinding");
		enemy = GetComponent<HorrorAI>();
		grid = pathFinding.GetComponent<Grid>();
		enemySight = GetComponent<EnemySight> ();

		chaseState = new ChaseState(this, enemy);
		alertState = new AlertState(this, enemy);
		patrolState = new PatrolState(this, enemy, grid);

		pathfindingStrategy = "Fringe";

		processedActions = new Queue<TreeAction>();
	}
	#endregion

	#region Start
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<AnimationController>();
		patrolState.GetPatrolPoint(15.0);
		currentState = patrolState;	
	}
	#endregion

	#region Update
	// Update is called once per frame
	void Update () 
	{
		currentState.UpdateState();

		decisionTime += Time.deltaTime;

		if(decisionTime >= DECISION_COOLDOWN)
		{
			currentState.GetStateAction();
			decisionTime = 0;
		}

		if(processedActions.Count > 0)
		{
			PerformAction(processedActions.Dequeue());
		}
	}
	#endregion

	#region AddActionToQueue
	/// <summary>
	/// Adds the action to queue to be performed by the AI
	/// </summary>
	/// <param name="action">Action.</param>
	public void AddActionToQueue(TreeAction action)
	{
		processedActions.Enqueue(action);
	}
	#endregion

	#region PerformAction
	/// <summary>
	/// Performs the processed action.
	/// </summary>
	/// <param name="action">Action.</param>
	public void PerformAction(TreeAction action)
	{
		if(action != null)
		{
			//Changes pathfinding strategy if necessary
			pathfindingStrategy = action.pathFindingMethod;

			//Tests to see if current state should be changed
			if(currentState.GetString() != action.targetState)
				PerformStateTransition(action.targetState);

			//Dynamically Invokes the actions method to be performed if there is one
			if(action.action != null)
				action.action.DynamicInvoke();

			//Sends animation type to animation controller
			anim.PerformTriggerAnimation(action.animation);
		}
	}
	#endregion

	#region PerformStateTransition
	/// <summary>
	/// Performs the state transition.
	/// </summary>
	/// <param name="state">State.</param>
	private void PerformStateTransition(string state)
	{
		switch(state)
		{
		case "Patrol":
			this.currentState = patrolState;
			break;
		case "Alert":
			this.currentState = alertState;
			break;
		case "Chase":
			this.currentState = chaseState;
			break;
		}

		//Performs the OnStateEnter method just before exiting to reset state
		currentState.OnStateEnter();
	}
	#endregion



	public void OnDrawGizmos()
	{
		if(currentState != null && patrolState != null)
		{
			foreach(Node node in patrolState.targetArea.nodeList)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(node.worldPosition, Vector3.one);
			}
		}
	}

}
