using UnityEngine;
using System.Collections;

public class Person : VRTK.VRTK_InteractableObject {
	public string preferredName;
	public string experience;
	public string totalExperience;

	private TextMesh nameText;

	public override void StartUsing(GameObject usingObject)
	{
		base.StartUsing(usingObject);
		nameText.text = preferredName + "\n" + experience + " Yrs @ ThoughtWorks\n" + totalExperience + " Yrs Total";
	}

	public override void StopUsing(GameObject usingObject)
	{
		base.StartUsing(usingObject);
		nameText.text = "";
	}

	// Use this for initialization
	void Start () {
		base.Start ();

		GameObject nameTextGameObject = VRTK.VRTK_DeviceFinder.GetControllerRightHand().transform.FindChild("NameText").gameObject;

		nameText = nameTextGameObject.GetComponent<TextMesh>();

		nameTextGameObject.transform.localPosition = new Vector3(0, 0.1f, 0);
	}
}
