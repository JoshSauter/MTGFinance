using UnityEngine;
using System.Collections;

public class ServerComm : MonoBehaviour {
	//URL should be completed with /multiverse_id for a specific card
	string cardInfoServerURL = "http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8000/mtg-card-prices/";
	//URL should be completed with /multiverse_id.jpg for a specific card
	string cardImageServerURL = "http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8080/mtg-card-images/";

	WWW cardPriceGet;
	WWW imageGet;

	// Use this for initialization
	IEnumerator Start () {
		cardPriceGet = new WWW(cardInfoServerURL + "");
		yield return cardPriceGet;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
