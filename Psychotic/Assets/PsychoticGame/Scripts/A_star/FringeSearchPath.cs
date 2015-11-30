using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class FringeSearchPath : GridPath
{


	public FringeSearchPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
		:base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)
	{

	}

	private void FindPath()
	{
		Node startNode = grid.NodeFromWorldPoint(startPos);
		Node targetNode = grid.NodeFromWorldPoint(targetPos);

		try
		{
			if(startNode.walkable && targetNode.walkable)
			{
				HashSet<Node> closedList = new HashSet<Node>();
				Heap<Node> openList = new Heap<Node>(grid.MaxSize);

				int threshold = GetDistance(startNode, targetNode);
				int fMin = int.MaxValue;

				while(openList.Count > 0)
				{


				}
			}
		}
		catch(Exception ex)
		{
			Debug.Log("Exception in Fringe Search " +ex);
		}

		if(path.pathSuccess)
			path.waypoints = RetracePath(startNode, targetNode);

		doneEvent.Set();
	}
}
