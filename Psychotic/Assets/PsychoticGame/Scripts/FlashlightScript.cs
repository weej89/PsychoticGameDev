#region Using
using UnityEngine;
using System.Collections;
#endregion

public class FlashlightScript : MonoBehaviour
{

	#region Public Fields
	public bool FlashlightOn;
	#endregion

	#region Private Fields
	private float FlashlightBattery;
	private Light flashLight;
	#endregion

	#region Start
	/// <summary>
	/// Initializes public and private fields
	/// </summary>
	void Start ()
	{
		FlashlightOn = false;
		FlashlightBattery = 100f;
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
		if (FlashlightOn && FlashlightBattery > 0) 
		{
			flashLight.intensity = 1;
			FlashlightBattery -= .1f;
		} 
		else 
		{
			flashLight.intensity = 0;
		}
	}
	#endregion
}