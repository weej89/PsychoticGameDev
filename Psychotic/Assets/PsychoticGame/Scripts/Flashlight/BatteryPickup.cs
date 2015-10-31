#region Using
using UnityEngine;
using System.Collections;
#endregion

public class BatteryPickup : MonoBehaviour {

	#region Private Variables
	private FlashlightScript flashlight;
	#endregion

	#region Start
	/// <summary>
	/// Start this instance and Get FlashlightScript
	/// </summary>
	void Start () {
		flashlight = GetComponentInChildren<FlashlightScript> ();
	}
	#endregion

	#region OnTriggerEnter
	/// <summary>
	/// Raises the trigger enter event against another collider.
	/// If collider object tag is a battery then add battery lige
	/// to player flashlight
	/// </summary>
	/// <param name="obj">Object.</param>
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
