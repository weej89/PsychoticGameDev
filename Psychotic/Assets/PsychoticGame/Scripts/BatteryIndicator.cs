#region Using
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
#endregion

public class BatteryIndicator : MonoBehaviour {

	#region Private Variables
	private FlashlightScript flashLight;
	#endregion

	#region Public Variables
	public Text batteryText;
	public int batteryLevel;
	#endregion

	#region Start
	/// <summary>
	/// Start this instance and get flashlight script component
	/// </summary>
	void Start () 
	{
		flashLight = GetComponentInParent<FlashlightScript> ();
	}
	#endregion

	#region Update
	/// <summary>
	/// Update this instance and current battery life
	/// </summary>
	void Update () 
	{
		batteryLevel = (int)flashLight.FlashlightBattery;
		batteryText.text = "Battery:  " + batteryLevel.ToString ();
	}
	#endregion

	#region SetBatteryText
	/// <summary>
	/// Sets the battery text.
	/// </summary>
	void SetBatteryText()
	{
		batteryText.text = "Battery:  " + batteryLevel.ToString ();
	}
	#endregion
}
