using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class TreeAction : DecisionTreeNode {

	public string pathFindingMethod;
	public Transform targetPos;
	public string animation;
	public int priority;
	public string targetState;


	public object[] args;
	public Action action = null;

	public TreeAction()
	{
	}

	public TreeAction(ref Transform _targetPos)
	{
		this.targetPos = _targetPos;
	}
	
	public override TreeAction MakeDecision (DecisionTreeNode root)
	{
		return this;
	}
	
	public override DecisionTreeNode GetBranch ()
	{
		throw new NotImplementedException ();
	}
}
