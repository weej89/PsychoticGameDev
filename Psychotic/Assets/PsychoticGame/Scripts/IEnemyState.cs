#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public interface IEnemyState 
{
	void UpdateState();

	void OnTriggerEnter(Collider other);

	void ToPatrolState();

	void ToCheckingState(float interval);

	void ToAlertState();

	void ToChaseState();

}
