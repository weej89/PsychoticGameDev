using UnityEngine;
using System.Collections;

//Increases battery life
public class BatteryPlus : MonoBehaviour {

	GameObject obj = GameObject.Find("Flashlight");

	void OnTriggerEnter(Collider Battery) {
		FlashlightScript sc = (FlashlightScript)obj.GetComponent (typeof(FlashlightScript))
		FlashlightScript.FlashlightBattery = 100f;
		DestroyObject (gameObject);
	}
}
