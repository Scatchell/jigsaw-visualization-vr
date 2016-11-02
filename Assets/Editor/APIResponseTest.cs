using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using SimpleJSON;

public class APIResponseTest
{

	[Test]
	public void ShouldMapPlayersToTwExperience ()
	{
		string json = "[{\"preferredName\":\"Adam Fahie\",\"gender\":\"Male\",\"totalExperience\":3.47,\"twExperience\":2.27},{\"preferredName\":\"Adam Fahie\",\"gender\":\"Male\",\"totalExperience\":3.47,\"twExperience\":3.27}]";
		GetShit gs = new GetShit ();

		var expectedList = new float[2] {
			2.27f,
			3.27f
		};

		var actualList = gs.MapToTwExperience (json);
		Assert.AreEqual (expectedList, actualList);
	}
}
