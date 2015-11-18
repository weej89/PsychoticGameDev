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

	public override TreeAction MakeDecision (DecisionTreeNode root)
	{
		DecisionTreeNode decision = GetBranch();

		if(decision is TreeAction)
		{
			TreeAction action = (TreeAction) decision;
			return action;
		}

		return MakeDecision(decision);
	}
}
