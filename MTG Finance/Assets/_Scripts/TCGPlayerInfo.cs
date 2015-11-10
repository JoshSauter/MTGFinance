using UnityEngine;
using System.Collections;
using JSONObj;

public class TCGPlayerInfo {
    public int multiverseID;       //MultiverseID of the card for lookup
    public string cardName;        //Name of the card
    public string setName;         //Set the card came from
    public string lastUpdated;     //Date of the last time pricing info for the card was updated
    public string tcgLow;          //Low price from TCGPlayer
    public string tcgMid;          //Median price from TCGPlayer
    public string tcgAvgFoil;     //Average price for a foil version of the card
    public string tcgLink;         //URL for the card's page on TCGPlayer
    public string gathererLink;    //URL for the card's page on Gatherer

    public TCGPlayerInfo() {
        multiverseID = -1;
        cardName = "";
        setName = "";
        lastUpdated = "";
        tcgLow = "";
        tcgMid = "";
        tcgAvgFoil = "";
        tcgLink = "";
        gathererLink = "";
    }

    //Turns a JSONObject into a TCGPlayerInfo object
    public static TCGPlayerInfo JSONToTCGPlayerInfo(JSONObject jso)
    {
        TCGPlayerInfo returnInfo = new TCGPlayerInfo();

        foreach (string key in jso.keys)
        {
            //Retrive the part of the JSON referenced by the key
            JSONObject subObject = (JSONObject)jso.GetField(key);

            //Perform the appropriate action for the field
            switch (key)
            {
                case "multiverse_id":
                    returnInfo.multiverseID = (int)subObject.n;
                    break;
                case "card_name":
                    returnInfo.cardName = subObject.str;
                    break;
                case "set_name":
                    returnInfo.setName = subObject.str;
                    break;
                case "last_upadated":
                    returnInfo.lastUpdated = subObject.str;
                    break;
                case "tcg_low":
                    returnInfo.tcgLow = subObject.str;
                    break;
                case "tcg_mid":
                    returnInfo.tcgMid = subObject.str;
                    break;
                case "tcg_avg_foil":
                    returnInfo.tcgAvgFoil = subObject.str;
                    break;
                case "tcg_link":
                    returnInfo.tcgLink = subObject.str;
                    break;
                case "gatherer_link":
                    returnInfo.gathererLink = subObject.str;
                    break;
            }
        }

        return returnInfo;
    }

    // Use this for initialization
    void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
