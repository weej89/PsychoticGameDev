﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

public class Pathfinding : MonoBehaviour {
	Grid grid;
	public Transform seeker;
	public Transform target;

	void Awake()
	{
		grid=GetComponent<Grid>();
	}

	void Update()
	{
		if(Input.GetKeyDown("space") || Input.GetKeyDown("a") || Input.GetKeyDown("s")|| Input.GetKeyDown("d")|| Input.GetKeyDown("w"))
			FindPath(seeker.position, target.position);
	}

	void FindPath(Vector3 startPos, Vector3 targetPos)
	{
		Stopwatch sw = new Stopwatch();
		sw.Start();

		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint (targetPos);

		Heap<Node> openSet = new Heap<Node>(grid.MaxSize);
		HashSet<Node> closedSet = new HashSet<Node>();
		openSet.Add(startNode);

		while (openSet.Count>0)
		{
			Node currentNode=openSet.RemoveFirst();
			closedSet.Add(currentNode);

			if(currentNode==targetNode)
			{
				sw.Stop();
				print("Path found: " +sw.ElapsedMilliseconds+ " ms");
				RetracePath(startNode, targetNode);
				return;
			}

			foreach(Node neighbor in grid.GetNeighbors(currentNode))
			{
				if(!neighbor.walkable || closedSet.Contains(neighbor))
				{
					continue;
				}

				int newMovementCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
				if(newMovementCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
				{
					neighbor.gCost = newMovementCostToNeighbor;
					neighbor.hCost = GetDistance(neighbor, targetNode);
					neighbor.parent = currentNode;

					if(!openSet.Contains(neighbor))
						openSet.Add(neighbor);
				}
			}
		}
	}

	void RetracePath(Node startNode, Node endNode)
	{
		List<Node> path = new List<Node>();
		Node currentNode = endNode;

		while(currentNode != startNode)
		{
			path.Add(currentNode);
			currentNode = currentNode.parent;
		}
		path.Reverse();

		grid.path = path;
	}

	int GetDistance(Node nodeA, Node nodeB)
	{
		int distX = Mathf.Abs(nodeA.gridX - nodeB.gridX);
		int distY = Mathf.Abs(nodeA.gridY - nodeB.gridY);

		if(distX > distY)
			return 14 * distY + 10 * (distX - distY);
		return 14 * distX + 10 * (distY - distX);

	}
}
