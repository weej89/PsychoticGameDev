using UnityEngine;
using System.Collections;

//When the player (or any designated trigger) walks through this object, it will delete
//the demo text. Other wise, it could be viewed from any where.
public class DeleteDemoText : MonoBehaviour {
	public GameObject TextToDelete;
	void Start () {
		TextToDelete = GameObject.Find("DemoText");
	}
	void OnTriggerEnter(Collider DelText1) {
		DestroyObject (TextToDelete);
	}
}
