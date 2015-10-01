/* using UnityEngine;
using System.Collections;

public class FlashlightScript : MonoBehaviour {


	void Start () {
		bool FlashlightOn = false;
		float FlashlightBattery = 100f;
		gameObject FL = GameObject.Find ("Flashlight");

	}
	

	void Update () {

	if(Input.GetKeyDown (KeyCode.F))
		{
			if (FlashlightOn == true)
				FlashlightOn = false;
			
			else if (FlashlightOn == false && FlashlightBattery > 0)
				FlashlightOn = true;
		}

		while (FlashlightOn == true){
			FlashlightBattery -= 0.1f;
				FL.GetComponent(Light).intensity = 1;

		}
		while (FlashlightOn == false)
			FL.GetComponent(Light).intensity = 0;
		if (FlashlightOn == true && FlashlightScriptBattery < 0.1f)
					FlashlightOn = false;
	}
}
*/