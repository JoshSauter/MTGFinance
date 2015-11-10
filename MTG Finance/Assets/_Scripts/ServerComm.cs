using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JSONObj;

public class ServerComm : MonoBehaviour {
	//URL should be completed with /multiverseID for a specific card
	string cardInfoServerURL = "http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8000/mtg-card-prices/";
	//URL should be completed with /multiverseID.jpg for a specific card
	string cardImageServerURL = "http://ec2-52-10-109-207.us-west-2.compute.amazonaws.com:8080/mtg-card-images/";

	WWW cardPriceGet;
	WWW imageGet;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public JSONObject RequestCardInfo(string cardName) {
        int multiverseID = CardDictionary.Cards[cardName].multiverseID;
        string cardInfoURL = cardInfoServerURL + multiverseID + "/";

        cardPriceGet = new WWW(cardInfoURL);

        JSONObject cardInfo = new JSONObject();

        StartCoroutine(WaitForResponse(cardPriceGet));

        //Must be done so that we don't read info from cardPriceGet before it's completely downloaded
        while (!cardPriceGet.isDone) { } 

        cardInfo = new JSONObject(cardPriceGet.text);

        return cardInfo;
    }

    public Sprite RequestCardImage(string cardName) {
        int multiverseID = CardDictionary.Cards[cardName].multiverseID;
        string cardImageURL = cardImageServerURL + multiverseID + ".jpg";

        imageGet = new WWW(cardImageURL);

        StartCoroutine(WaitForResponse(imageGet));

        //Must be done so that we don't read info from cardPriceGet before it's completely downloaded
        while (!imageGet.isDone) { }

        //Convert WWW.texture to a sprite
        Sprite cardImage = Sprite.Create(imageGet.texture, new Rect(0, 0, imageGet.texture.width, imageGet.texture.height), new Vector2(0.5f, 0.5f));

        return cardImage;
    }

    IEnumerator WaitForResponse(WWW www)
    {
        yield return www;

        if (www.error == null)
        {
            Debug.Log("www OK!: " + www.text);
            
        }
        else
        {
            Debug.Log("www ERROR!: " + www.error);
        }
    }
}
