#region Using
using UnityEngine;
using System.Collections;
#endregion

public class AnimationController : MonoBehaviour 
{
	#region Private Variables
	private HorrorAI zombie;
	private StatePatternEnemy state;
	private Animator anim;
	#endregion

	#region Start
	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		zombie = GetComponent<HorrorAI>();
		state = GetComponent<StatePatternEnemy>();
	}
	#endregion

	#region Update
	// Update is called once per frame
	void Update () 
	{
		float enemySpeed = zombie.speed;
		anim.SetFloat("Speed", enemySpeed);		
	}
	#endregion

	#region PerformTriggerAnimation
	/// <summary>
	/// Performs the trigger animation.
	/// </summary>
	/// <param name="animation">Animation.</param>
	public void PerformTriggerAnimation(string animation)
	{	
		switch(animation)
		{
			case "attack01":
			anim.SetTrigger("Attack01");
			break;
			case "attack02":
			anim.SetTrigger("Attack02");
			break;
		}
	}
	#endregion
}
