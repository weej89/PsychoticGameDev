#region Using
using UnityEngine;
using System.Collections;
#endregion

public class Node : IHeapItem<Node> {

	#region Public Variables
	public bool walkable;
	public Vector3 worldPosition;
	public int gridX;
	public int gridY;

	public int gCost;
	public int hCost;
	public Node parent;
	public int movementPenalty;
	#endregion

	#region Private Variables
	int heapIndex;
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="Node"/> class.
	/// </summary>
	/// <param name="_walkable">If set to <c>true</c> _walkable.</param>
	/// <param name="_worldPos">_world position.</param>
	/// <param name="_gridX">_grid x.</param>
	/// <param name="_gridY">_grid y.</param>
	public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _penalty)
	{
		walkable=_walkable;
		worldPosition=_worldPos;
		gridX=_gridX;
		gridY=_gridY;
		movementPenalty=_penalty;
	}
	#endregion

	#region fCost
	/// <summary>
	/// Gets the f cost of the node 
	/// </summary>
	/// <value>The f cost.</value>
	public int fCost
	{
		get{return gCost+hCost+movementPenalty;}
	}
	#endregion

	#region HeapIndex
	/// <summary>
	/// Gets or sets the index of the nodes place in the heap
	/// </summary>
	/// <value>The index of the heap.</value>
	public int HeapIndex
	{
		get{return heapIndex;}
		set{heapIndex = value;}
	}
	#endregion

	#region CompareTo
	/// <summary>
	/// Comparator method to use against another nodes
	/// fCost.
	/// </summary>
	/// <returns>The to.</returns>
	/// <param name="nodeToCompare">Node to compare.</param>
	public int CompareTo(Node nodeToCompare)
	{
		int compare = fCost.CompareTo(nodeToCompare.fCost);

		if(compare == 0)
			compare = hCost.CompareTo(nodeToCompare.hCost);

		return -compare;
	}
	#endregion

	public override int GetHashCode ()
	{
		return base.GetHashCode ();
	}

	public override bool Equals (object obj)
	{
		Node nodeToCompare = (Node)obj;

		if(this.gridX == nodeToCompare.gridX && this.gridY == nodeToCompare.gridY)
			return true;
		else
			return false;
	}
}
