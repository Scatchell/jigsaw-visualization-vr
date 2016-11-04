using UnityEngine;
using System.Collections;

public class Person : VRTK.VRTK_InteractableObject {
	public string preferredName;
	public string experience;
	private TextMesh nameText;

	public override void StartUsing(GameObject usingObject)
	{
		base.StartUsing(usingObject);
		nameText.text = preferredName + " - " + experience;
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
		nameText = gameObject.transform.FindChild ("NameText").gameObject.GetComponent<TextMesh>();
		gameObject.transform.FindChild ("NameText").localPosition = new Vector3(0, 0.6f, 0);
	}
}
