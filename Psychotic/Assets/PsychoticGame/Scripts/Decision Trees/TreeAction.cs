#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public class TreeAction : DecisionTreeNode 
{
	#region Public Variables
	public string pathFindingMethod;
	public Transform targetPos;
	public string animation;
	public int priority;
	public string targetState;

	public object[] args;

	//This action will stay null unless there is something that needs to be performed during the action
	public Action action = null;
	#endregion

	#region Constructor
	public TreeAction()
	{
	}

	public TreeAction(ref Transform _targetPos)
	{
		this.targetPos = _targetPos;
	}
	#endregion

	#region MakeDecision
	/// <summary>
	/// Makes the decision.
	/// </summary>
	/// <returns>This defined action</returns>
	/// <param name="root">Root Node</param>
	public override TreeAction MakeDecision (DecisionTreeNode root)
	{
		return this;
	}
	#endregion

	#region GetBranch
	//Not Used In Actions
	public override DecisionTreeNode GetBranch ()
	{
		throw new NotImplementedException ();
	}
	#endregion
}
