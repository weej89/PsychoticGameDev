#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
#endregion

public class DynBiDirBeamPath : GridPath
{
	#region PrivateVariables
    private DBDBOPAgent toGoalAgent;
	private DBDBOPAgent fromGoalAgent;
	private Node startNode, targetNode;
    #endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="DynBiDirBeamPath"/> class.
	/// </summary>
	/// <param name="_grid">_grid.</param>
	/// <param name="_meshCopy">_mesh copy.</param>
	/// <param name="_startPos">_start position.</param>
	/// <param name="_targetPos">_target position.</param>
	/// <param name="_callback">_callback.</param>
	/// <param name="_pathId">_path identifier.</param>
    public DynBiDirBeamPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
        :base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)    
    {
        startNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        toGoalAgent = new DBDBOPAgent(this, meshCopy, startNode, targetNode, grid.MaxSize/meshCopy.GetLength(0));
        fromGoalAgent = new DBDBOPAgent(this, meshCopy, targetNode, startNode, grid.MaxSize/meshCopy.GetLength(0));
    }
	#endregion

	#region FindPath
	/// <summary>
	/// (!!!This is abstract and must be implemented by inheritors!!!)
	/// Finds the path.
	/// </summary>
	public override void FindPath()
	{
    	DBDBOPAgent.PathFoundResponse responseFromAgent;

		//Starts the stopwatch
      	stopWatch.Start();

       	while(path.pathSuccess == false)
       	{
        	   responseFromAgent = toGoalAgent.DoSearch(fromGoalAgent.closedSet);
           	if (responseFromAgent == DBDBOPAgent.PathFoundResponse.GOAL) //Goal found
           	{
            	path.pathSuccess = true;
               	path.waypoints = RetracePath(startNode, targetNode);
               	break;
           	}
           	else if (responseFromAgent == DBDBOPAgent.PathFoundResponse.OTHER_AGENT) //Other agent found
           	{
               	path.pathSuccess = true;
               	Vector3[] toArray = RetracePath(startNode, toGoalAgent.agentResult.lastNodeInSet);
               	Vector3[] fromArray = RetracePath(targetNode, toGoalAgent.agentResult.lastNode);
               	Array.Reverse(fromArray);
               	path.waypoints = new Vector3[toArray.Length + fromArray.Length];
               	Array.Copy(toArray, path.waypoints, toArray.Length);
               	Array.Copy(fromArray, 0, path.waypoints, toArray.Length, fromArray.Length);
               	break;
           	}
           	else if (responseFromAgent == DBDBOPAgent.PathFoundResponse.NOT_FOUND) //Not found giving up
               	break;

           	responseFromAgent = fromGoalAgent.DoSearch(toGoalAgent.closedSet);
           	if (responseFromAgent == DBDBOPAgent.PathFoundResponse.GOAL) //Goal found
           	{
               	path.pathSuccess = true;
               	path.waypoints = RetracePath(startNode, targetNode);
               	Array.Reverse(path.waypoints);
               	break;
           	}
           	else if (responseFromAgent == DBDBOPAgent.PathFoundResponse.OTHER_AGENT) //Other agent found
           	{
               	path.pathSuccess = true;
               	Vector3[] toArray = RetracePath(startNode, fromGoalAgent.agentResult.lastNode);
               	Vector3[] fromArray = RetracePath(targetNode, fromGoalAgent.agentResult.lastNodeInSet);
               	Array.Reverse(fromArray);

               	path.waypoints = new Vector3[toArray.Length + fromArray.Length];
               	Array.Copy(toArray, path.waypoints, toArray.Length);
               	Array.Copy(fromArray, 0, path.waypoints, toArray.Length, fromArray.Length);
               	break;
           	}
           	else if (responseFromAgent == DBDBOPAgent.PathFoundResponse.NOT_FOUND) //Not found giving up
               	break;
       	}

		//Stops the stopwatch timer
       	stopWatch.Stop();

		//Writes the results of pathfinding to test file
       	WriteResults(stopWatch.ElapsedMilliseconds, "BDBOP", (toGoalAgent.closedSet.Count + toGoalAgent.openSet.Count + fromGoalAgent.closedSet.Count + fromGoalAgent.openSet.Count), path.waypoints.Length);

		//This must be done to notify of when thread has completed
       	doneEvent.Set();
    }
	#endregion
}
