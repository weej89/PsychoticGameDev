#pragma strict

/*
private var guiShow : boolean = false;

var isOpen : boolean = true;

var door : GameObject;

var rayLength = 10;

function Update()
{
	var hit : RaycastHit;
	var fwd = transform.TransformDirection(Vector3.forward);
	
	if(Physics.Raycast(transform.position, fwd, hit, rayLength))
	{
		if(hit.collider.gameObject.tag == "Door")
		{
			guiShow = true;
			if(Input.GetKeyDown("e") && isOpen == false)
			{
				door.GetComponent.<Animation>().Play("DoorOpen");
				isOpen = true;
				guiShow = false;
			}
			
			else if(Input.GetKeyDown("e") && isOpen == true)
			{
				door.GetComponent.<Animation>().Play("DoorClose");
				isOpen = false;
				guiShow = false;
			}
		}
		else
		{
		    guiShow = false;
		}
	}
}

function OnGUI()
{
	if(guiShow == true && isOpen == false)
	{
		GUI.Box(Rect(Screen.width / 2, Screen.height / 2, 250, 25), "Press E to open the door");
	}
}
*/

var smooth = 2.0;
var DoorOpenAngle = 90.0;
public var open : boolean;
private var enter : boolean;

private var defaultRot : Vector3;
private var openRot : Vector3;

function Start()
{
	defaultRot = transform.eulerAngles;
	openRot = new Vector3 (defaultRot.x, defaultRot.y + DoorOpenAngle, defaultRot.z);
}

//Main function
function Update ()
{
	if(open)
	{
		//Open door
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, openRot, Time.deltaTime * smooth);
	}
	else
	{
		//Close door
		transform.eulerAngles = Vector3.Slerp(transform.eulerAngles, defaultRot, Time.deltaTime * smooth);
	}

	if(Input.GetKeyDown("e") && enter)
	{
		open = !open;
	}
}

function OnGUI()
{
	if(enter)
	{
		GUI.Label(new Rect(Screen.width/2 - 75, Screen.height - 100, 150, 30), "Press 'E' to open the door");
	}
}

//Activate the Main function when player is near the door
function OnTriggerEnter (other : Collider)
{
	if (other.gameObject.tag == "Player") 
	{
		enter = true;
	}
}

//Deactivate the Main function when player is go away from door
function OnTriggerExit (other : Collider)
{
	if (other.gameObject.tag == "Player") 
	{
		enter = false;
	}
}