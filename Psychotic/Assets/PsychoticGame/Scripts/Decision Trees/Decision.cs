#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#endregion

public class Decision : DecisionTreeNode
{
	#region Public Variables
	public object[] args;
	#endregion

	#region Private Variables
	private Func<object[], bool> determining;
	#endregion

	#region ProtectedVariables
	protected string operation = string.Empty;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="Decision"/> class.
	/// </summary>
	/// <param name="_determining">_determining.</param>
	public Decision(Func<object[], bool> _determining)
	{
		this.determining = _determining;
	}

	public Decision(DecisionTreeNode _falseNode, DecisionTreeNode _trueNode, Func<object[], bool> _determining)
		:base(_falseNode, _trueNode)
	{
		this.determining = _determining;
	}
	#endregion

	#region GetBranch
	/// <summary>
	/// (!!!Since this is an abstract class any inhertors must implement this!!!)
	/// Gets the branch.
	/// </summary>
	/// <returns>The branch (True/False)</returns>
	public override DecisionTreeNode GetBranch ()
	{
		if(determining(args))
			return trueNode;
		else
			return falseNode;
	}
	#endregion

	#region SetNodes
	/// <summary>
	/// Sets nodes in the tree to corresponding true or false position
	/// </summary>
	/// <param name="_falseNode">_false node.</param>
	/// <param name="_trueNode">_true node.</param>
	public void SetNodes(DecisionTreeNode _falseNode, DecisionTreeNode _trueNode)
	{
		this.trueNode = _trueNode;
		this.falseNode = _falseNode;
	}
	#endregion

	#region MakeDecision
	/// <summary>
	/// Makes a decision and returns a tree action recursively
	/// </summary>
	/// <returns>The Action.</returns>
	/// <param name="root">Root Node</param>
	public override TreeAction MakeDecision (DecisionTreeNode root)
	{
		if(root == null)
			return null;

		if(root is TreeAction)
		{
			TreeAction action = (TreeAction) root;
			return action;
		}

		return MakeDecision(root.GetBranch());
	}
	#endregion
}