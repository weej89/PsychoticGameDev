#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public class TargetArea 
{
	#region Public Variables
	public List<Node> nodeList;
	#endregion

	#region Private Variables
	private Grid grid;
	private Vector3 center;
	private float radius;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="TargetArea"/> class.
	/// </summary>
	/// <param name="_grid">_grid.</param>
	/// <param name="_center">_center.</param>
	/// <param name="_radius">_radius.</param>
	public TargetArea(Grid _grid, Vector3 _center, float _radius)
	{
		center = _center;
		radius = _radius;
		grid = _grid;

		this.nodeList = new List<Node>();
		CreatePossibleTargetList();
	}
	#endregion

	#region AddNodeFromWorldPoint
	/// <summary>
	/// Adds the node from world point.
	/// </summary>
	/// <param name="worldPoint">World point.</param>
	private void AddNodeFromWorldPoint(Vector3 worldPoint)
	{
		Node node = grid.NodeFromWorldPoint(worldPoint);

		if(node.walkable && nodeList.Contains(node) == false)
			nodeList.Add(node);
	}
	#endregion

	#region GenerateCheckingPath
	/// <summary>
	/// Generates the checking path.
	/// </summary>
	/// <returns>The checking path.</returns>
	public Vector3 GenerateCheckingPath()
	{
		if(nodeList.Count > 0)
		{
			int index = Random.Range(0, nodeList.Count-1);
			Node node = nodeList[index];
			nodeList.RemoveAt(index);

			return node.worldPosition;
		}
		else
			return new Vector3(-1, -1, -1);
	}
	#endregion

	#region CreatePossibleTargetList
	/// <summary>
	/// Creates the possible target list within a cirlce of nodes
	/// </summary>
	private void CreatePossibleTargetList()
	{
		for(float x = (center.x - radius); x <= center.x; x++)
		{
			for(float y = (center.z - radius); y <= center.z; y++)
			{
				if((x - center.x)*(x - center.x) + (y - center.z)*(y-center.z) <= radius * radius)
				{
					float xSym = (int)(center.x - (x - center.x));
					float ySym = (int)(center.z - (y - center.z));

					AddNodeFromWorldPoint(new Vector3(x, 0, y));
					AddNodeFromWorldPoint(new Vector3(x, 0, ySym));
					AddNodeFromWorldPoint(new Vector3(xSym, 0, y));
					AddNodeFromWorldPoint(new Vector3(xSym, 0, ySym));
				}
			}
		}
	}
	#endregion
}
