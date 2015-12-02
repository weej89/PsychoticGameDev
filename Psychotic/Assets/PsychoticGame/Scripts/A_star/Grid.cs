#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class Grid : MonoBehaviour {

	#region Public Variables
	public Transform player;
	public LayerMask unwalkableMask;
	public Vector2 gridWorldSize;
	public float nodeRadius;
	public bool drawingGizmos = false;
	public TerrainType [] walkableRegions;
	#endregion

	#region Private Variables
	float nodeDiameter;
	int gridSizeX, gridSizeY;
	Node[,] grid;
	LayerMask walkableMask;
	Dictionary <int, int> walkableRegionsDictionary = new Dictionary<int, int>();
	#endregion

	#region Awake
	/// <summary>
	/// Awake this instance.
	/// Sets a node diameter to make up positions in the grid
	/// Gets both y and x values = to world size / nodeDiameter
	/// Creates the grid
	/// </summary>
	void Awake()
	{
		nodeDiameter=nodeRadius*2;
		gridSizeX=Mathf.RoundToInt(gridWorldSize.x/nodeDiameter);
		gridSizeY=Mathf.RoundToInt(gridWorldSize.y/nodeDiameter);

		foreach(TerrainType region in walkableRegions)
		{
			walkableMask.value = walkableMask | region.terrainMask.value;
			walkableRegionsDictionary.Add((int)Mathf.Log(region.terrainMask.value, 2),region.terrainPenalty);
		}
		CreateGrid();

		HashSet<Node> nodesSeen = new HashSet<Node>();

		for(int r = 0; r<gridSizeX; r++)
		{
			for(int c = 0; c<gridSizeY; c++)
			{
				Node node = grid[r,c];
				DetermineMovementPenalty(nodesSeen, node, 5);
			}

			foreach(Node n in grid)
				if(n.movementPenalty > 0)
					nodesSeen.Add(n);
		}

	}
	#endregion

	#region MaxSize
	/// <summary>
	/// Gets the max size of the Grid
	/// </summary>
	/// <value>The max size of the Grid</value>
	public int MaxSize
	{
		get{return gridSizeX * gridSizeY;}
	}
	#endregion

	#region CreateGrid
	/// <summary>
	/// Creates the grid using nodes
	/// </summary>
	void CreateGrid()
	{
		grid=new Node[gridSizeX,gridSizeY];
		Vector3 worldBottmLeft=transform.position-Vector3.right * gridWorldSize.x/2 - Vector3.forward * gridWorldSize.y / 2;

		for(int x=0; x<gridSizeX; x++)
		{
			for(int y=0; y<gridSizeY; y++)
			{
				Vector3 worldPoint=worldBottmLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
			
				bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));

				int movementPenalty = 0;

				if(walkable)
				{
					Ray ray = new Ray(worldPoint + Vector3.up * 50, Vector3.down);
					RaycastHit hit;
					if(Physics.Raycast(ray, out hit, 100, walkableMask))
					{
						walkableRegionsDictionary.TryGetValue(hit.collider.gameObject.layer, out movementPenalty);
					}
				}
				grid[x,y]=new Node(walkable, worldPoint, x, y, movementPenalty);
			}
		}
	}
	#endregion

	#region GetGridCopy
	/// <summary>
	/// Returns a copy of the 2d node array for use with threads so that they don't
	/// interfere with the overall grid structure and cause race conditions during pathfinding
	/// </summary>
	/// <returns>The grid copy.</returns>
	public Node[,] GetGridCopy()
	{
		Node[,] newGrid = new Node[gridSizeX,gridSizeY];

		for(int r = 0; r < gridSizeX; r++)
		{
			for(int c = 0; c < gridSizeY; c++)
			{
				Node copyNode = grid[r,c];
				newGrid[r,c] = new Node(copyNode.walkable, copyNode.worldPosition, r, c, copyNode.movementPenalty);
			}
		}

		return newGrid;
	}
	#endregion


	#region GetNeighbors
	/// <summary>
	/// Gets the neighbors of the current node being checked
	/// </summary>
	/// <returns>The neighbors.</returns>
	/// <param name="node">Node.</param>
	public List<Node> GetNeighbors(Node node)
	{
		List<Node> neighbors = new List<Node>();

		for(int x=-1; x<=1; x++)
		{
			for(int y=-1; y<=1; y++)
			{
				if(x==0 && y==0)
					continue;

				int checkX = node.gridX + x;
				int checkY = node.gridY + y;

				if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbors.Add(grid[checkX, checkY]);
				}
			}
		}
		
		return neighbors;
	}

	//New Method for copy
	public List<Node> GetNeighbors(Node[,] otherGrid, Node node)
	{
		List<Node> neighbors = new List<Node>();

		for(int x=-1; x<=1; x++)
		{
			for(int y=-1; y<=1; y++)
			{
				if(x==0 && y==0)
					continue;
				
				int checkX = node.gridX + x;
				int checkY = node.gridY + y;
				
				if(checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
				{
					neighbors.Add(otherGrid[checkX, checkY]);
				}
			}
		}
		
		return neighbors;
	}
	#endregion

	#region DetermineMovementPenalty
	/// <summary>
	/// Used for weighting different areas of the graph
	/// Will create a sort of heat map going outwards from unwalkable nodes
	/// </summary>
	/// <param name="seenNodes">Seen nodes.</param>
	/// <param name="node">Node.</param>
	/// <param name="penalty">Penalty.</param>
	public void DetermineMovementPenalty(HashSet<Node> seenNodes, Node node, int penalty)
	{
		if(penalty > 0)
		{
			foreach(Node n in GetNeighbors(node))
			{
				if((!n.walkable || node.movementPenalty > 0) && !seenNodes.Contains(n) )
				{
					node.movementPenalty += penalty;
					DetermineMovementPenalty(seenNodes, n, penalty-1);
				}
			}
		}
	}
	#endregion

	#region NodeFromWorldPoint
	/// <summary>
	/// Returns a Node from a given Vector 3 world point
	/// </summary>
	/// <returns>The Node from world point.</returns>
	/// <param name="worldPosition">World position.</param>
	public Node NodeFromWorldPoint(Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;

		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);

		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

		return grid[x,y];
	}

	//New method for grid copy
	public Node NodeFromWorldPoint(Node[,] otherGrid, Vector3 worldPosition)
	{
		float percentX = (worldPosition.x + gridWorldSize.x/2) / gridWorldSize.x;
		float percentY = (worldPosition.z + gridWorldSize.y/2) / gridWorldSize.y;
		
		percentX = Mathf.Clamp01(percentX);
		percentY = Mathf.Clamp01(percentY);
		
		int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
		int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);
		
		return otherGrid[x,y];
	}
	#endregion

	#region OnDrawGizmos
	/// <summary>
	/// Draws gizmos on screen as wire cubes to 
	/// represent walkable and unwalkable spaces
	/// on the grid.  Walkable are white unwalkable
	/// are red.  This will only happen if drawingGizmos
	/// public bool is true.
	/// </summary>
	void OnDrawGizmos()
	{
		if(drawingGizmos == true)
		{
			Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));
			if(grid!=null)
			{
				foreach(Node n in grid)
				{
					Gizmos.color=(n.walkable)?Color.white:Color.red;
					Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
					/*
					if(!n.walkable)
					{
						Gizmos.color = Color.red;
						Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
					}
					if(n.movementPenalty > 0)
					{
						Gizmos.color = Color.yellow;
						Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - .1f));
					}
					*/
				}
			}
		}
	}
	#endregion

	#region ResetNodes
	public void ResetNodes()
	{
		foreach(Node n in grid)
		{
			n.parent = null;
		}
	}

	public void ResetNodes(Node[,] otherGrid)
	{
		foreach(Node n in otherGrid)
		{
			n.parent = null;
		}
	}
	#endregion

	[System.Serializable]
	public class TerrainType
	{
		public LayerMask terrainMask;
		public int terrainPenalty;
	}
}
