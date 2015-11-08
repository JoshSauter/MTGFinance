using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardInfo : MonoBehaviour {
	public Text cardNameText;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//TODO: Grab pricing information from server and populate
	//appropriate field with this information
	//TODO: Cache prices for a given day so we don't query the server
	//for prices we already know (prices only change daily)
	public void DisplayInformationFor(string cardName) {
		cardNameText.text = cardName;
	}
}
