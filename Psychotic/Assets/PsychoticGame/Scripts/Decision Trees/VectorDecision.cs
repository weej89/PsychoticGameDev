using UnityEngine;
using System.Collections;

public class VectorDecision : Decision
{
	private Vector3 testValue;
	private Vector3 arg1, arg2;
	
	public VectorDecision(DecisionTreeNode _trueNode, DecisionTreeNode _falseNode, TestCondition _testCondition, Vector3 arg1, Vector3 testValue)
		:base(_trueNode, _falseNode, _testCondition)
	{
		this.testValue = testValue;
		this.arg1 = arg1;
	}
	
	public VectorDecision(DecisionTreeNode _trueNode, DecisionTreeNode _falseNode, TestCondition _testCondition, Vector3 testValue, Vector3 arg1, Vector3 arg2, string operation)
		:base(_trueNode, _falseNode, _testCondition)
	{
		this.testValue = testValue;
		this.arg1 = arg1;
		this.arg2 = arg2;
		this.operation = operation;
	}

	public override DecisionTreeNode GetBranch ()
	{
		throw new System.NotImplementedException ();
	}
}
