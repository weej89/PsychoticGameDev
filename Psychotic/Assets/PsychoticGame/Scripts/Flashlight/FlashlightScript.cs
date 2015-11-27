#region Using
using UnityEngine;
using System.Collections;
#endregion

public class FlashlightScript : MonoBehaviour
{

	#region Public Fields
	public bool FlashlightOn;
	public bool flashlightBatteryEnabled = false;

	public float FlashlightBattery
	{
		get{return flashlightBattery;}
	}

	#endregion

	#region Private Fields
	private float flashlightBattery;
	private Light flashLight;
	#endregion

	#region Start
	/// <summary>
	/// Initializes public and private fields
	/// </summary>
	void Start ()
	{
		FlashlightOn = false;
		flashlightBattery = 100f;
		flashLight = this.GetComponent<Light>();
	}
	#endregion

	#region Update
	/// <summary>
	/// Update this instance.
	/// Tells the Game Component whether or not
	/// the Light in the flashlight should be
	/// turned on and if the battery is charged
	/// </summary>
	void Update ()
	{
		if (FlashlightOn && flashlightBattery > 0) 
		{
			flashLight.intensity = 1;
			if(flashlightBatteryEnabled)
				flashlightBattery -= .1f;
		} 
		else 
		{
			flashLight.intensity = 0;
		}
	}
	#endregion

	#region AddBatteryLife
	/// <summary>
	/// Adds the battery life to the flashlight from
	/// a battery value using pickup
	/// </summary>
	/// <param name="battery">Battery.</param>
	public void AddBatteryLife(float battery)
	{
		flashlightBattery += battery;
	}
	#endregion
}