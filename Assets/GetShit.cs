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
	public GameObject personTower;
	private static ILogger logger = Debug.logger;

	// Use this for initialization
	void Start ()
	{
		ServicePointManager.ServerCertificateValidationCallback = AlwaysCorrect;
		var jsonResponse = CreateJigsawRequest ("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester");
		var secondJsonResponse = CreateJigsawRequest ("https://jigsaw.thoughtworks.net/api/people\\?staffing_office=Manchester&page=2");

		List<JSONNode> listOfTwers = MapToTwers (jsonResponse).ToList ();
		List<JSONNode> secondListOfTwers = MapToTwers (secondJsonResponse).ToList ();

		List<JSONNode> fullList = listOfTwers.Union (secondListOfTwers).ToList ();
		fullList.Sort ((p1, p2) => p1 ["twExperience"].AsFloat.CompareTo (p2 ["twExperience"].AsFloat));
		float lastPosition = CreateCubes (1.0f, fullList);
	}

	private float CreateCubes (float initialPosition, List<JSONNode> listOfTwers)
	{
		float position = initialPosition;
		float lastTwExperience = 0;
		listOfTwers.ForEach (e => {
			float twExperience = e ["twExperience"].AsFloat;
			float totalExperience = e ["totalExperience"].AsFloat;
			float localPosition = (twExperience / 2) + position;

			GameObject tower = (GameObject) Instantiate(personTower, new Vector3(0, totalExperience, localPosition), Quaternion.identity);
			tower.transform.localScale = Vector3.Scale(tower.transform.localScale, new Vector3(1, totalExperience, 1));


			GameObject personCube = (GameObject)Instantiate (personSphere, new Vector3 (0, tower.transform.position.y * 2, localPosition), Quaternion.identity);


			Person person = personCube.GetComponent<Person> ();
			person.preferredName = e ["preferredName"].Value;
			person.experience = e ["twExperience"].Value;
			person.totalExperience = e["totalExperience"].Value;

			person.transform.localScale = new Vector3 (twExperience, twExperience, twExperience);

			string picture = e ["picture"] ["url"];
			AttachPictureToPerson (personCube, picture);

			float force = Random.Range (0.0f, 0.3f);
			float force2 = Random.Range (0.0f, 0.3f);
			//person.GetComponent<Rigidbody> ().AddForce (new Vector3 (force, 0, force2));

			position += twExperience;
		});

		return position;
	}

	private void AttachPictureToPerson (GameObject person, string picture)
	{
		string[] number = picture.Split ('/');
		string url = "http://s3.amazonaws.com/thoughtworks-jigsaw-production/upload/consultants/images/" + number [4] + "/profile/picture.jpg";
		StartCoroutine (ApplyTexture (url, person, 0));
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

	IEnumerator ApplyTexture (string url, GameObject gameObject, int attempts)
	{
		UnityWebRequest www = UnityWebRequest.GetTexture (url);
		www.SetRequestHeader ("Accept", "image/*");
		//Debug.Log ("Downloading...");
		//Debug.Log (url);
		yield return www.Send ();

		while (!www.isDone) {
			Debug.LogError ("Error!");
		}
		if (www.isError) {
			logger.Log (LogType.Error, www.error);
			yield return null;

		} else {
			long forbidden = 403;
			long responseCode = www.responseCode;
			if (responseCode == forbidden && attempts == 0) {
				StartCoroutine (ApplyTexture (url.Replace (".jpg", ".png"), gameObject, 1));
				yield return null;
			} else if (responseCode == forbidden && attempts == 1) {
				StartCoroutine (ApplyTexture (url.Replace (".png", ".JPG"), gameObject, 2));
				yield return null;
			} else if (responseCode == forbidden && attempts == 2) {
				StartCoroutine (ApplyTexture (url.Replace (".JPG", ".jpeg"), gameObject, 3));
				yield return null;
			} else if (responseCode == forbidden && attempts == 3) {
				StartCoroutine (ApplyTexture (url.Replace (".jpeg", ".JPEG"), gameObject, 4));
				yield return null;
			} else if (responseCode == forbidden && attempts == 4) {
				StartCoroutine (ApplyTexture (url.Replace (".JPEG", ".PNG"), gameObject, 5));
				yield return null;
			}
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
