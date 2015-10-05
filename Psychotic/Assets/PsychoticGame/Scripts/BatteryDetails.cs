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
	// Use this for initialization
	void Start () 
	{
		randMin = 25;
		randMax = 75;

		batteryValue = Random.Range (randMin, randMax);
	}
	#endregion

	#region GetBatteryValue
	public float GetBatteryValue()
	{
		return batteryValue;
	}
	#endregion

}
