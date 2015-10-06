using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BatteryIndicator : MonoBehaviour {

	private FlashlightScript flashLight;
	public Text batteryText;
	public int batteryLevel;

	// Use this for initialization
	void Start () {
		flashLight = GetComponentInParent<FlashlightScript> ();
	}
	
	// Update is called once per frame
	void Update () {
		batteryLevel = (int)flashLight.FlashlightBattery;
		batteryText.text = "Battery:  " + batteryLevel.ToString ();
	}

	void SetBatteryText()
	{
		batteryText.text = "Battery:  " + batteryLevel.ToString ();
	}
}
