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
	public void DisplayInformationFor(string setName, JSONObject versionInfo) {
        ////Get card price info
        //TCGPlayerInfo curInfo = SpecificCardDictionary.cardListDict[setName];
        print("WHAAAAAT: " + setName);
        //Get card image
        Image curImage = cardImage;
            
        curImage.sprite = server.RequestCardImage(versionInfo["card_image_name"].str);

		//Set up image to open up Gatherer page upon click
		string tempGathererLink = versionInfo["gatherer_link"].str;
		Button imageButton = curImage.gameObject.GetComponent<Button>();
		imageButton.onClick.RemoveAllListeners();
		imageButton.onClick.AddListener(delegate {
			Application.OpenURL(tempGathererLink);
		});

        cardNameText.text = versionInfo["card_name"].str;
        cardPriceText.text = "TCG Player: $" + versionInfo["tcg_mid"].str;

		//Set up price text to open up TCGPlayer page upon click
		string tempTCGLink = versionInfo["tcg_link"].str;
		Button priceButton = cardPriceText.gameObject.GetComponent<Button>();
		priceButton.onClick.RemoveAllListeners();
		priceButton.onClick.AddListener(delegate {
			Application.OpenURL(tempTCGLink);
		});
        cardImage = curImage;
	}
}
