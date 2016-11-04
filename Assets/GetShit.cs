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
		var jsonResponse = CreateJigsawRequest ("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester");
		var secondJsonResponse = CreateJigsawRequest ("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester&page=2");

		List<JSONNode> listOfTwers = MapToTwers (jsonResponse).ToList ();
		List<JSONNode> secondListOfTwers = MapToTwers (secondJsonResponse).ToList ();

		List<JSONNode> fullList = listOfTwers.Union (secondListOfTwers).ToList();
		fullList.Sort( (p1,p2)=>p1 ["twExperience"].AsFloat.CompareTo(p2 ["twExperience"].AsFloat) );
		float lastPosition = CreateCubes (1.0f, fullList);
	}

	private float CreateCubes (float initialPosition, List<JSONNode> listOfTwers)
	{
		float position = initialPosition;
		float lastTwExperience = 0;
		listOfTwers.ForEach (e => {
			float twExperience = e ["twExperience"].AsFloat;
			float localPosition = (twExperience / 2) + position;
			GameObject person = (GameObject)Instantiate (personSphere, new Vector3 (0, twExperience, localPosition), Quaternion.Euler (new Vector3 (0, 0, 0)));
			person.GetComponent<Person> ().preferredName = e ["preferredName"].Value;
			person.GetComponent<Person> ().experience = e ["twExperience"].Value;
			person.transform.localScale = new Vector3 (twExperience, twExperience, twExperience);

			string picture = e ["picture"] ["url"];
			AttachPictureToPerson (person, picture);

			float force = Random.Range (0.0f, 0.3f);
			float force2 = Random.Range (0.0f, 0.3f);
			//person.GetComponent<Rigidbody> ().AddForce (new Vector3 (force, 0, force2));

			position += twExperience + 0.1f;
		});

		return position;
	}

	private void AttachPictureToPerson (GameObject person, string picture)
	{
		string[] number = picture.Split ('/');
		string url = "http://s3.amazonaws.com/thoughtworks-jigsaw-production/upload/consultants/images/" + number [4] + "/profile/picture.jpg";
		StartCoroutine (ApplyTexture (url, person));
	}

	static string CreateJigsawRequest (string url)
	{
		HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create (url);
		request.Method = WebRequestMethods.Http.Get;
		request.Accept = JSON_CONTENT_TYPE;
		request.Headers ["Authorization"] = AuthorizationToken.Token ();
		var responseStream = request.GetResponse ().GetResponseStream ();
		string jsonResponse = new StreamReader (responseStream).ReadToEnd ();
		logger.Log (jsonResponse);
		return jsonResponse;
	}

	IEnumerator ApplyTexture (string url, GameObject gameObject)
	{
		UnityWebRequest www = UnityWebRequest.GetTexture (url);
		www.SetRequestHeader ("Accept", "image/*");
		Debug.Log ("Downloading...");
		yield return www.Send ();

		while (!www.isDone) {
			Debug.LogError ("Error!");
		}
		if (www.isError) {
			logger.Log (LogType.Error, www.error);
			yield return null;

		} else {
			Texture myTexture = ((DownloadHandlerTexture)www.downloadHandler).texture;
			gameObject.GetComponent<Renderer> ().materials [0].mainTexture = myTexture;
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
