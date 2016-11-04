using UnityEngine;
using System.Collections;

public class Person : VRTK.VRTK_InteractableObject {
	public string preferredName;
	private TextMesh nameText;

	public override void StartUsing(GameObject usingObject)
	{
		base.StartUsing(usingObject);
		nameText.text = preferredName;
		Debug.Log ("using");
	}

	public override void StopUsing(GameObject usingObject)
	{
		base.StartUsing(usingObject);
		nameText.text = "";
		Debug.Log ("Stop using");
	}

	// Use this for initialization
	void Start () {
		base.Start ();
		nameText = GameObject.Find ("NameText").GetComponent<TextMesh>();
	}
}
