#region Using
using UnityEngine;
using System.Collections;
#endregion

public class FlashlightToggle : MonoBehaviour {

	private FlashlightScript flashLightScript;

	#region Start
	/// <summary>
	/// Initializes private and public fields
	/// </summary>
	void Start () 
	{
		flashLightScript = GetComponent<FlashlightScript> ();
	}
	#endregion

	#region Update
	/// <summary>
	/// Updates the flashlight object when the
	/// registered key is pressed
	/// </summary>
	void Update () {
		if (Input.GetKeyDown ("f")) 
		{
			flashLightScript.FlashlightOn=!flashLightScript.FlashlightOn;
		}
	}
	#endregion
}
