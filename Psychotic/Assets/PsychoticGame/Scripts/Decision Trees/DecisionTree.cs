using UnityEngine;
using System;
using System.Collections;
using System.Threading;

public class DecisionTree
{
	public Action<TreeAction> callback;
	TreeAction[] actions;
	Decision[] decisions;
	int timesRun = 0;

	ManualResetEvent doneEvent;

	public bool EventCompleted
	{get{return doneEvent.WaitOne(0);}}

	public DecisionTree(Decision[] _decisions, TreeAction[] _actions, Action<TreeAction> _callback)
	{
		decisions = _decisions;
		actions = _actions;
		callback = _callback;

		doneEvent = new ManualResetEvent(true);
	}

	public void StartDecisionProcess()
	{
		ThreadPool.QueueUserWorkItem(ThreadPoolCallback, timesRun++);
	}

	private void ThreadPoolCallback(System.Object threadContext)
	{
		doneEvent.Reset();
		MakeDecision();
	}

	private void MakeDecision()
	{
		TreeAction action = decisions[0].MakeDecision(decisions[0].GetBranch());
		doneEvent.Set();
		callback.DynamicInvoke(action);
	}
}
