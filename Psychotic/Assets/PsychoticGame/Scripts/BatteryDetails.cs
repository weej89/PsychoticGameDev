#region Using
using UnityEngine;
using System.Collections;
#endregion

public class BatteryDetails : MonoBehaviour {

	#region Private Fields
	private Random rand;
	private float randMin;
	private float randMax;
	private float batteryValue;
	#endregion

	#region Start
	/// <summary>
	/// Start this instance and get a battery charge value
	/// </summary>
	void Start () 
	{
		randMin = 25;
		randMax = 75;

		batteryValue = Random.Range (randMin, randMax);
	}
	#endregion

	#region GetBatteryValue
	/// <summary>
	/// Gets the battery value.
	/// </summary>
	/// <returns>The battery value.</returns>
	public float GetBatteryValue()
	{
		return batteryValue;
	}
	#endregion

}
