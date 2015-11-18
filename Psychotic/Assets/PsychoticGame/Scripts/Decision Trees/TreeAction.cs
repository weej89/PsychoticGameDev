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
	private Func<object[], int> function;

	public TreeAction()
	{
		this.function = null;
	}

	public TreeAction(Func<object[] ,int> function)
	{
		this.function = function;
	}

	public TreeAction(ref Transform _targetPos)
	{
		this.function = null;
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
