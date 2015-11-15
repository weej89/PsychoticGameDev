using UnityEngine;
using System.Collections;

public class Action : DecisionTreeNode
{
	string pathFindingMethod;
	Vector3 targetPos;
	string animation;
	int priority;

	public override Action MakeDecision (DecisionTreeNode root)
	{
		return this;
	}

	public override DecisionTreeNode GetBranch ()
	{
		throw new System.NotImplementedException ();
	}
}
