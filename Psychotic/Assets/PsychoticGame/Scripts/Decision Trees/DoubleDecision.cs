using UnityEngine;
using System.Collections;

public class DoubleDecision : Decision 
{
	private double testValue;
	private double arg1, arg2;

	public DoubleDecision(DecisionTreeNode _trueNode, DecisionTreeNode _falseNode, TestCondition _testCondition, double arg1, double testValue)
		:base(_trueNode, _falseNode, _testCondition)
	{
		this.testValue = testValue;
		this.arg1 = arg1;
	}

	public DoubleDecision(DecisionTreeNode _trueNode, DecisionTreeNode _falseNode, TestCondition _testCondition, double testValue, double arg1, double arg2, string operation)
		:base(_trueNode, _falseNode, _testCondition)
	{
		this.testValue = testValue;
		this.arg1 = arg1;
		this.arg2 = arg2;
		this.operation = operation;
	}

	public override DecisionTreeNode GetBranch ()
	{
		bool nodeType;

		if(operation != string.Empty)
		{
			double result = PerformOperation();
			nodeType = PerformTest(result);
		}
		else
		{
			nodeType = PerformTest(arg1);
		}

		if(nodeType)
			return trueNode;
		else
			return falseNode;
	}

	private double PerformOperation()
	{
		double result;

		switch(operation)
		{
		case "-":
			result = arg1 - arg2;
			return result;
		case "*":
			result = arg1 * arg2;
			return result;
		case "+":
			result = arg1 + arg2;
			return result;
		case "/":
			result = arg1 / arg2;
			return result;
		default:
			Debug.Log("Operation Not Performed In DoubleDecision");
			return 0.0f;
		}
	}
	
	private bool PerformTest(double result)
	{
		switch(testCondition)
		{
		case TestCondition.EQUAL_TO:
			if(result == testValue)
				return true;
			break;
		case TestCondition.GREATER_THAN:
			if(result > testValue)
				return true;
			break;
		case TestCondition.LESS_THAN:
			if(result < testValue)
				return true;
			break;
		case TestCondition.NOT_EQUAL:
			if(result != testValue)
				return true;
			break;
		}
		
		return false;
	}
}
