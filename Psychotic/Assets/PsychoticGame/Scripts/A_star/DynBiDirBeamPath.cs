using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

public class DynBiDirBeamPath : GridPath
{

    DBDBOPAgent toGoalAgent;
    DBDBOPAgent fromGoalAgent;
    Node startNode, targetNode;
    
    public DynBiDirBeamPath(Grid _grid, Node[,] _meshCopy, Vector3 _startPos, Vector3 _targetPos, Action<Vector3[], bool> _callback, int _pathId)
        :base(_grid, _meshCopy, _startPos, _targetPos, _callback, _pathId)    
    {
        startNode = grid.NodeFromWorldPoint(startPos);
        targetNode = grid.NodeFromWorldPoint(targetPos);

        toGoalAgent = new DBDBOPAgent(this, meshCopy, startNode, targetNode, grid.MaxSize/meshCopy.GetLength(0));
        fromGoalAgent = new DBDBOPAgent(this, meshCopy, targetNode, startNode, grid.MaxSize/meshCopy.GetLength(0));

        //toGoalAgent.SetOppositeAgentHash(ref fromGoalAgent.closedSet);
        //fromGoalAgent.SetOppositeAgentHash(ref toGoalAgent.closedSet);
    }

    public override void FindPath()
    {
       DBDBOPAgent.PathFoundResponse responseFromAgent;

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

       doneEvent.Set();
    }
}
