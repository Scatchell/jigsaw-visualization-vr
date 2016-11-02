using UnityEngine;
using System.Collections;
using System.Net;
using System.IO;
using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

public class GetShit : MonoBehaviour
{
	const string JSON_CONTENT_TYPE = "application/json";
	public GameObject personSphere;

	// Use this for initialization
	void Start ()
	{
		
		HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester");

		request.Method = WebRequestMethods.Http.Get;
		request.Accept = JSON_CONTENT_TYPE;
		request.Headers ["Authorization"] = "f59ade505821a90237485b116dd3f519";

		var responseStream = request.GetResponse ().GetResponseStream ();
		string jsonResponse = new StreamReader (responseStream).ReadToEnd ();

		List<float> listOfTwExperience = MapToTwExperience (jsonResponse).ToList ();


		float position = 1f;
		listOfTwExperience.ForEach (e => {
			GameObject sphere = (GameObject) Instantiate (personSphere, new Vector3 (0, position, 0), Quaternion.Euler (new Vector3 (0, 0, 0)));
			sphere.transform.localScale = new Vector3 (e, e, e);
			IEnumerator<Texture> list = GetTexture();



			float force = Random.Range (0.0f, 0.3f);
			float force2 = Random.Range (0.0f, 0.3f);
			sphere.GetComponent<Rigidbody>().AddForce(new Vector3(force, 0, force2));
			position += e + 0.1f;

		});
		

		listOfTwExperience.ForEach (i => Debug.Log (i));

		//WWW www = new WWW("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester");
	}

	IEnumerator<Texture> GetTexture() {
		UnityWebRequest www = UnityWebRequest.GetTexture("http://s3.amazonaws.com/thoughtworks-jigsaw-production/upload/consultants/images/18400/profile/picture.jpg?1432921008");
		yield return www.Send();

		if(www.isError) {
			Debug.Log(www.error);
		}
		else {
			Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
		}
	}

	public IEnumerable<float> MapToTwExperience (string peopleJson)
	{
		IEnumerable<JSONNode> people = JSON.Parse (peopleJson).AsArray.OfType<JSONNode> ().ToList ();
		IEnumerable<float> twExperience = people.Select (p => p ["twExperience"].AsFloat);
		return twExperience;
	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
