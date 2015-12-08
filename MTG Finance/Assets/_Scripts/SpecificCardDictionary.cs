//using UnityEngine;
//using System.Collections;
//using System.IO;
//using System.Collections.Generic;
//using JSONObj;

//public class SpecificCardDictionary : MonoBehaviour {
//	public static Dictionary<string, TCGPlayerInfo> cardListDict = new Dictionary<string, TCGPlayerInfo>();

//	//Number of cards to process each frame
//	int numCardsPerFrame = 400;

//	// Use this for initialization
//	void Start () {

//	}
	
//	// Update is called once per frame
//	void Update () {
	
//	}

//    public static void BuildDictionary(JSONObject cardList) {
//        JSONObject curCardList = cardList["cards"];
//        print("Current Card List: " + curCardList.Print());
//        foreach (var cardJso in curCardList.list)
//        {
//            TCGPlayerInfo newCard = TCGPlayerInfo.JSONToTCGPlayerInfo(cardJso);
//            cardListDict[newCard.setName] = newCard;
//        }
//    }
//}
