#region Using
using UnityEngine;
using System;
using System.Collections;
using System.Threading;
#endregion

public class DecisionTree
{
	#region Private Variables
	private TreeAction[] actions;
	private Decision[] decisions;
	private int timesRun = 0;	
	private ManualResetEvent doneEvent;
	#endregion

	#region Public Variables
	public Action<TreeAction> callback;
	#endregion

	#region Public Fields
	public bool EventCompleted
	{get{return doneEvent.WaitOne(0);}}
	#endregion

	#region Constructor
	/// <summary>
	/// Initializes a new instance of the <see cref="DecisionTree"/> class.
	/// </summary>
	/// <param name="_decisions">_decisions.</param>
	/// <param name="_actions">_actions.</param>
	/// <param name="_callback">_callback.</param>
	public DecisionTree(Decision[] _decisions, TreeAction[] _actions, Action<TreeAction> _callback)
	{
		decisions = _decisions;
		actions = _actions;
		callback = _callback;

		doneEvent = new ManualResetEvent(true);
	}
	#endregion

	#region StartDecisionProcess
	public void StartDecisionProcess()
	{
		ThreadPool.QueueUserWorkItem(ThreadPoolCallback, timesRun++);
	}
	#endregion

	#region ThreadPoolCallback
	/// <summary>
	/// Starts new thread of making decision from decision tree
	/// </summary>
	/// <param name="threadContext">Thread context.</param>
	private void ThreadPoolCallback(System.Object threadContext)
	{
		doneEvent.Reset();
		MakeDecision();
	}
	#endregion

	#region MakeDecision
	/// <summary>
	/// Makes the decision.
	/// </summary>
	private void MakeDecision()
	{
		TreeAction action = decisions[0].MakeDecision(decisions[0].GetBranch());
		doneEvent.Set();

		//Performs the callback event passing the action that is to be performed from decision making process
		callback.DynamicInvoke(action);
	}
	#endregion
}
