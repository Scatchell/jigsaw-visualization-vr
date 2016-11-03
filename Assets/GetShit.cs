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
	private static ILogger logger = Debug.logger;

	// Use this for initialization
	void Start ()
	{
		ServicePointManager.ServerCertificateValidationCallback = AlwaysCorrect;
		HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create ("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester");

		request.Method = WebRequestMethods.Http.Get;
		request.Accept = JSON_CONTENT_TYPE;
		request.Headers ["Authorization"] = "f59ade505821a90237485b116dd3f519";

		var responseStream = request.GetResponse ().GetResponseStream ();
		string jsonResponse = new StreamReader (responseStream).ReadToEnd ();
		logger.Log (jsonResponse);

		List<JSONNode> listOfTwers = MapToTwers (jsonResponse).ToList ();


		float position = 1f;
		listOfTwers.ForEach (e => {
			GameObject sphere = (GameObject) Instantiate (personSphere, new Vector3 (0, position, 0), Quaternion.Euler (new Vector3 (0, 0, 0)));
			float twExperience = e["twExperience"].AsFloat;
			sphere.transform.localScale = new Vector3 (twExperience, twExperience, twExperience);
//			IEnumerator<Texture> list = GetTexture();
			ServicePointManager.ServerCertificateValidationCallback = AlwaysCorrect;

			string picture = e["picture"]["url"];
			string[] number = picture.Split('/');

			string url = "http://s3.amazonaws.com/thoughtworks-jigsaw-production/upload/consultants/images/" + number[4] + "/profile/picture.jpg";
			logger.Log(url);
			StartCoroutine(GetTexture(url, sphere));

			float force = Random.Range (0.0f, 0.3f);
			float force2 = Random.Range (0.0f, 0.3f);
			sphere.GetComponent<Rigidbody>().AddForce(new Vector3(force, 0, force2));
			position += twExperience + 0.1f;

		});

		//WWW www = new WWW("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester");
	}

	IEnumerator GetTexture(string url, GameObject gameObject) {
		UnityWebRequest www = UnityWebRequest.GetTexture (url);
		www.SetRequestHeader ("Accept", "image/*");
		Debug.Log("Downloading...");
		yield return www.Send();

		while (!www.isDone){
			Debug.LogError(".");
		}
		if(www.isError) {
			logger.Log(LogType.Error,www.error);
			yield return null;

		}
		else {
			Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
			gameObject.GetComponent<Renderer>().materials[0].mainTexture = myTexture;
			yield return null;
		}
	
	}

	bool AlwaysCorrect (object sender, System.Security.Cryptography.X509Certificates.X509Certificate certificate, System.Security.Cryptography.X509Certificates.X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
	{
		return true;
	}

	public IEnumerable<JSONNode> MapToTwers (string peopleJson)
	{
		return JSON.Parse (peopleJson).AsArray.OfType<JSONNode> ().ToList ();

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
}
