using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JSONObj;

public class CardInfo : MonoBehaviour {
	public Text cardNameText;
    public Text cardPriceText;
    public Image cardImage;

    ServerComm server;
	// Use this for initialization
	void Start () {
        server = GetComponent<ServerComm>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//TODO: Grab pricing information from server and populate
	//appropriate field with this information
	//TODO: Cache prices for a given day so we don't query the server
	//for prices we already know (prices only change daily)
	public void DisplayInformationFor(string cardName) {
        //Get card price info
        JSONObject curCardJSON = server.RequestCardInfo(cardName);
        TCGPlayerInfo curInfo = TCGPlayerInfo.JSONToTCGPlayerInfo(curCardJSON);

        //Get card image
        Image curImage = cardImage;
            
        curImage.sprite = server.RequestCardImage(cardName);

        cardNameText.text = cardName;
        cardPriceText.text = "TCG Player: $" + curInfo.tcg_mid;
        cardImage = curImage;
	}
}
