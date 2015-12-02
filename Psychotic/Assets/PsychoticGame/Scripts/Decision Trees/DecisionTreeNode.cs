using UnityEngine;
using System.Collections;

public abstract class DecisionTreeNode
{
	#region Protected Variables
	protected DecisionTreeNode trueNode;
	protected DecisionTreeNode falseNode;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionTreeNode"/> class.
	/// </summary>
	public DecisionTreeNode()
	{

	}

	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionTreeNode"/> class.
	/// </summary>
	/// <param name="_falseNode">_false node.</param>
	/// <param name="_trueNode">_true node.</param>
	public DecisionTreeNode (DecisionTreeNode _falseNode, DecisionTreeNode _trueNode)
	{
		this.trueNode = _trueNode;
		this.falseNode = _falseNode;		
	}
	#endregion

	#region GetBranch
	/// <summary>
	/// (!!!Since this is an abstract class any inhertors must implement this!!!)
	/// Gets the branch.
	/// </summary>
	/// <returns>The branch.</returns>
	public abstract DecisionTreeNode GetBranch();
	#endregion

	#region MakeDecision
	/// <summary>
	/// Makes a decision and returns a tree action recursively
	/// </summary>
	/// <returns>The Action.</returns>
	/// <param name="root">Root Node</param>
	public abstract TreeAction MakeDecision(DecisionTreeNode root);
	#endregion
}
