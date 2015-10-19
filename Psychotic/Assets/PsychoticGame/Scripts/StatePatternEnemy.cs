using UnityEngine;
using System.Collections;

public class StatePatternEnemy : MonoBehaviour
{
	public float searchingTurnSpeed = 120f;
	public float searchingDuration = 4f;
	public float sightRange = 20f;
	public Transform[] wayPoints;
	public Transform eyes;
	public Vector3 offset = new Vector3 (0, .5f, 0);
	public MeshRenderer meshRendererFlag;
	public TargetArea targetArea;
	public float targetAreaRadius = 50f;

	[HideInInspector] public Transform chaseTarget;
	[HideInInspector] public IEnemyState currentState;
	[HideInInspector] public ChaseState chaseState;
	[HideInInspector] public AlertState alertState;
	[HideInInspector] public PatrolState patrolState;
	[HideInInspector] public CheckingState checkingState;

	private HorrorAI enemy;
	private Grid grid;

	private void Awake()
	{
		GameObject pathFinding = GameObject.Find("A*");
		enemy = GetComponent<HorrorAI>();
		grid = pathFinding.GetComponent<Grid>();

		if(grid == null)
		{
			print("This sucks");
		}

		chaseState = new ChaseState(this, enemy);
		alertState = new AlertState(this);
		patrolState = new PatrolState(this, enemy);
	}
	// Use this for initialization
	void Start () 
	{
		GeneratePatrolPath();
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

	public void GeneratePatrolPath()
	{
		targetArea = new TargetArea(grid, enemy.target.transform.position, targetAreaRadius);
		targetArea.GenerateCheckingPath(Random.Range(2, 5));
		enemy.CallForNewPath(targetArea.SearchList[0].worldPosition);
	}

	public void UpdateCheckingState(float interval)
	{
		checkingState = new CheckingState(this, enemy, interval, targetArea.SearchList);
	}
}
