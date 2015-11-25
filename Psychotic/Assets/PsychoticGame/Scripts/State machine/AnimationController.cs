using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour 
{
	HorrorAI zombie;
	StatePatternEnemy state;
	Animator anim;

	// Use this for initialization
	void Start () 
	{
		anim = GetComponent<Animator>();
		zombie = GetComponent<HorrorAI>();
		state = GetComponent<StatePatternEnemy>();

	}
	
	// Update is called once per frame
	void Update () 
	{
		float enemySpeed = zombie.speed;
		anim.SetFloat("Speed", enemySpeed);		
	}

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
}
