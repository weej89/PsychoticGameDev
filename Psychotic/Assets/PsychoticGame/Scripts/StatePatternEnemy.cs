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
	
	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public IEnemyState currentState;
	[HideInInspector] public ChaseState chaseState;
	[HideInInspector] public AlertState alertState;
	[HideInInspector] public PatrolState patrolState;

	private HorrorAI enemy;
	private Grid grid;

	private void Awake()
	{
		GameObject pathFinding = GameObject.Find("A*");
		enemy = GetComponent<HorrorAI>();
		grid = pathFinding.GetComponent<Grid>();

		chaseState = new ChaseState(this, enemy);
		alertState = new AlertState(this, enemy);
		patrolState = new PatrolState(this, enemy, grid);

	}
	// Use this for initialization
	void Start () 
	{
		patrolState.GetPatrolPoint(15.0);
		currentState = patrolState;	
	}
	
	// Update is called once per frame
	void Update () 
	{
		currentState.UpdateState();
		Debug.DrawRay(eyes.transform.position, eyes.transform.forward, Color.white, 1.0f);
	}

	private void OnTriggerEnter(Collider other)
	{
		currentState.OnTriggerEnter(other);
	}

	/*
	public void OnDrawGizmos()
	{
		if(targetArea != null)
		{
			foreach(Node node in targetArea.nodeList)
			{
				Gizmos.color = Color.blue;
				Gizmos.DrawCube(node.worldPosition, Vector3.one);
			}
		}
	}
	*/
}
