using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TargetArea {

	private List<Node> searchList;

	List<Node> nodeList;
	Grid grid;
	Vector3 center;
	float radius;
	double area;

	public List<Node> SearchList
	{
		get{return searchList;}
	}

	public TargetArea(Grid _grid, Vector3 _center, float _radius)
	{
		center = _center;
		radius = _radius;
		grid = _grid;

		this.area = Mathf.PI * Mathf.Pow(radius, 2);
		this.nodeList = new List<Node>();
	}

	private void AddNodeFromWorldPoint(Vector3 worldPoint)
	{
		Node node = grid.NodeFromWorldPoint(worldPoint);

		if(node.walkable && nodeList.Contains(node) == false)
			nodeList.Add(node);
	}

	public List<Node> GenerateCheckingPath(int numPoints)
	{
		CreatePossibleTargetList();
		searchList = new List<Node>();

		for(int i = 0; i < numPoints; i++)
		{
			int index = Random.Range(0, nodeList.Count-1);
			searchList.Add(nodeList[index]);
			nodeList.RemoveAt(index);
		}

		return searchList;
	}

	private void CreatePossibleTargetList()
	{
		for(float x = (center.x - radius); x <= center.x; x++)
		{
			for(float y = (center.y - radius); y <= center.y; y++)
			{
				if((x - center.x)*(x - center.x) + (y - center.y)*(y-center.y) <= radius * radius)
				{
					float xSym = (int)(center.x - (x - center.x));
					float ySym = (int)(center.y - (y - center.y));

					AddNodeFromWorldPoint(new Vector3(x, 0, y));
					AddNodeFromWorldPoint(new Vector3(x, 0, ySym));
					AddNodeFromWorldPoint(new Vector3(xSym, 0, y));
					AddNodeFromWorldPoint(new Vector3(xSym, 0, ySym));
				}
			}
		}
	}
}
