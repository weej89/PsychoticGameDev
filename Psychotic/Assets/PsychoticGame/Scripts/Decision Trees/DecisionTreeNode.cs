using UnityEngine;
using System.Collections;

public abstract class DecisionTreeNode
{
	protected DecisionTreeNode trueNode;
	protected DecisionTreeNode falseNode;

	public DecisionTreeNode()
	{

	}

	public DecisionTreeNode (DecisionTreeNode _trueNode, DecisionTreeNode _falseNode)
	{
		this.trueNode = _trueNode;
		this.falseNode = _falseNode;		}
		
	public abstract DecisionTreeNode GetBranch();
		
	public abstract Action MakeDecision(DecisionTreeNode root);
}
