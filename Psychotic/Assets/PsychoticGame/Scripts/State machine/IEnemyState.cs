#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public interface IEnemyState 
{
	void UpdateState();

	void OnTriggerEnter(Collider other);

	void OnStateEnter();

	void ToPatrolState();
	
	void ToAlertState();

	void ToChaseState();

	string GetString();
}
