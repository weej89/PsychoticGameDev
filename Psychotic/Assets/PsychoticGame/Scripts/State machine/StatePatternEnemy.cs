using UnityEngine;
using System.Collections;

public class StatePatternEnemy : MonoBehaviour
{
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


	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public IEnemyState currentState;
	[HideInInspector] public ChaseState chaseState;
	[HideInInspector] public AlertState alertState;
	[HideInInspector] public PatrolState patrolState;
	[HideInInspector] public Animator anim;
	[HideInInspector] public EnemySight enemySight;

	[HideInInspector] public string pathfindingStrategy;


	/*
	private HorrorAI enemy;
	private Grid grid;
	*/

	public HorrorAI enemy;
	public Grid grid;
	private void Awake()
	{
		//GameObject pathFinding = GameObject.Find("A*");
		//enemy = GetComponent<HorrorAI>();
		//grid = pathFinding.GetComponent<Grid>();
		enemySight = GetComponent<EnemySight> ();
		chaseState = new ChaseState(this, enemy);
		alertState = new AlertState(this, enemy);
		patrolState = new PatrolState(this, enemy, grid);

	}
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		patrolState.GetPatrolPoint(15.0);
		currentState = patrolState;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentState.UpdateState();
	}

	private void OnTriggerEnter(Collider other)
	{
		currentState.OnTriggerEnter(other);
	}

	public void PerformAction(TreeAction action)
	{
		pathfindingStrategy = action.pathFindingMethod;
		enemy.target = action.targetPos;

		if(currentState.GetString() != action.targetState)
			PerformStateTransition(action.targetState);
	}

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

		currentState.OnStateEnter();
	}

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
