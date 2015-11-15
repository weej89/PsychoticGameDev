using UnityEngine;
using System.Collections;

public abstract class Decision : DecisionTreeNode
{
	protected TestCondition testCondition;
	protected string operation = string.Empty;
	public enum TestCondition{LESS_THAN, EQUAL_TO, GREATER_THAN, NOT_EQUAL}

	public Decision(DecisionTreeNode _trueNode, DecisionTreeNode _falseNode, TestCondition _testCondition)
		:base(_trueNode, _falseNode)
	{
		this.testCondition = _testCondition;
	}

	public Decision(DecisionTreeNode _trueNode, DecisionTreeNode _falseNode, TestCondition _testCondition, string operation)
		:base(_trueNode, _falseNode)
	{
		this.testCondition = _testCondition;
		this.operation = operation;
	}

	public override Action MakeDecision (DecisionTreeNode root)
	{
		DecisionTreeNode decision = GetBranch();

		if(decision is Action)
		{
			Action action = (Action) decision;
			return action;
		}

		return MakeDecision(decision);
	}
}
