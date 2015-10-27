using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetArea {

	public List<Node> nodeList;
	Grid grid;
	Vector3 center;
	float radius;

	public TargetArea(Grid _grid, Vector3 _center, float _radius)
	{
		center = _center;
		radius = _radius;
		grid = _grid;

		this.nodeList = new List<Node>();
		CreatePossibleTargetList();
	}

	private void AddNodeFromWorldPoint(Vector3 worldPoint)
	{
		Node node = grid.NodeFromWorldPoint(worldPoint);

		if(node.walkable && nodeList.Contains(node) == false)
			nodeList.Add(node);
	}

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
}
