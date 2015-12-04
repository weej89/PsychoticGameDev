using UnityEngine;
using System.Collections;

public class Trigger : MonoBehaviour 
{
	public string[] TriggerTags; 
	public AudioSource Sound = null;
	public string TagRef = null;
	public int NumActivations = 0;
	public bool StopOnExit = false;
	public bool LoopSound = false;

	public enum Trigger_Type{ON_ENTER, ON_STAY, ON_AWAKE}
	public Trigger_Type TriggerType;
	
	protected GameObject[] enemy;
	protected int activationCount = 0;

	// Use this for initialization
	void Start () 
	{
		InitGameObjects();

		if(TriggerType != Trigger_Type.ON_AWAKE)
		{
			foreach(GameObject obj in enemy)
				obj.SetActive(false);
		}
		else
		{
			foreach(GameObject obj in enemy)
				obj.SetActive(true);

			Sound.Play();
		}
	}
	
	public void OnTriggerEnter(Collider other)
	{
		if(TriggerType == Trigger_Type.ON_ENTER)
		{
			foreach(string tag in TriggerTags)
			{
				if(other.CompareTag(tag))
				{
					SetTriggerActive(true, true);
				}
			}
		}
	}

	public void OnTriggerStay(Collider other)
	{
		if(TriggerType == Trigger_Type.ON_STAY)
		{
			foreach(string tag in TriggerTags)
			{
				if(other.CompareTag(tag))
				{
					SetTriggerActive(true, true);
				}
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if(StopOnExit)
		{
			foreach(string tag in TriggerTags)
			{
				if(other.CompareTag(tag))
				{
					SetTriggerActive(false, false);
				}
			}
		}
	}

	private void InitGameObjects()
	{
		if(TagRef != null)
		{
			enemy = GameObject.FindGameObjectsWithTag(TagRef);
		}

		if(Sound != null)
			Sound.loop = LoopSound;
	}

	private void SetTriggerActive(bool playSound, bool setActiveObjects)
	{
		foreach(GameObject obj in enemy)
			obj.SetActive(setActiveObjects);

		if(Sound != null)
		{
			if(playSound && !Sound.isPlaying)
				Sound.Play();
			else
				Sound.Stop();
		}

		activationCount++;

		if(NumActivations > 0 && activationCount >= NumActivations)
		{
			this.gameObject.SetActive(false);
		}
	}
	
}
