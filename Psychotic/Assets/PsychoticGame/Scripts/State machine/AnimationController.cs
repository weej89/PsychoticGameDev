using UnityEngine;
using System.Collections;

public class AnimationController : MonoBehaviour 
{
	HorrorAI zombie;
	StatePatternEnemy state;
	Animator anim;

	bool playingTriggerAnimation = false;
	string currentlyPlayingAnimation = string.Empty;

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
		if(!playingTriggerAnimation && animation != "None")
		{
			anim.SetTrigger(animation);
			playingTriggerAnimation = true;
			currentlyPlayingAnimation = animation;
		}
		else if(animation == "None")
		{
			anim.ResetTrigger(currentlyPlayingAnimation);
			playingTriggerAnimation = false;
		}
	}
}
