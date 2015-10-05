using UnityEngine;
using System.Collections;

public class BatteryPickup : MonoBehaviour {

	private FlashlightScript flashlight;

	// Use this for initialization
	void Start () {
		flashlight = GetComponentInChildren<FlashlightScript> ();
	}

	#region OnTriggerEnter
	void OnTriggerEnter(Collider obj) 
	{	
		if (obj.gameObject.CompareTag ("Battery")) 
		{
			BatteryDetails batDetails;
			batDetails = obj.GetComponent <BatteryDetails> ();
			flashlight.AddBatteryLife(batDetails.GetBatteryValue());
			obj.gameObject.SetActive(false);
		}
	}
	#endregion
}
