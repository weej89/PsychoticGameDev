#region Using
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
#endregion

public interface IEnemyState 
{
	void UpdateState();

	void OnStateEnter();

	string GetString();

	void GetStateAction();
}
