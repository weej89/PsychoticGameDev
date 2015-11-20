using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Decision : DecisionTreeNode
{
	public object[] args;
	private Func<object[], bool> determining;
	protected TestCondition testCondition;
	protected string operation = string.Empty;
	public enum TestCondition{LESS_THAN, EQUAL_TO, GREATER_THAN, NOT_EQUAL}

	public Decision(Func<object[], bool> _determining)
	{
		this.determining = _determining;
	}

	public Decision(DecisionTreeNode _falseNode, DecisionTreeNode _trueNode, Func<object[], bool> _determining)
		:base(_falseNode, _trueNode)
	{
		this.determining = _determining;
	}

	public override DecisionTreeNode GetBranch ()
	{
		if(determining(args))
			return trueNode;
		else
			return falseNode;
	}

	public void SetNodes(DecisionTreeNode _falseNode, DecisionTreeNode _trueNode)
	{
		this.trueNode = _trueNode;
		this.falseNode = _falseNode;
	}

	public override TreeAction MakeDecision (DecisionTreeNode root)
	{
		//DecisionTreeNode decision = GetBranch();

		if(root == null)
			return null;

		if(root is TreeAction)
		{
			TreeAction action = (TreeAction) root;
			return action;
		}

		return MakeDecision(root.GetBranch());
	}
}