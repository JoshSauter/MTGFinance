using UnityEngine;
using System.Collections;
using JSONObj;

public class TCGPlayerInfo {
    public int multiverse_id;       //MultiverseID of the card for lookup
    public string card_name;        //Name of the card
    public string set_name;         //Set the card came from
    public string last_updated;     //Date of the last time pricing info for the card was updated
    public string tcg_low;          //Low price from TCGPlayer
    public string tcg_mid;          //Median price from TCGPlayer
    public string tcg_avg_foil;     //Average price for a foil version of the card
    public string tcg_link;         //URL for the card's page on TCGPlayer
    public string gatherer_link;    //URL for the card's page on Gatherer

    public TCGPlayerInfo() {
        multiverse_id = -1;
        card_name = "";
        set_name = "";
        last_updated = "";
        tcg_low = "";
        tcg_mid = "";
        tcg_avg_foil = "";
        tcg_link = "";
        gatherer_link = "";
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
                    returnInfo.multiverse_id = (int)subObject.n;
                    break;
                case "card_name":
                    returnInfo.card_name = subObject.str;
                    break;
                case "set_name":
                    returnInfo.set_name = subObject.str;
                    break;
                case "last_upadated":
                    returnInfo.last_updated = subObject.str;
                    break;
                case "tcg_low":
                    returnInfo.tcg_low = subObject.str;
                    break;
                case "tcg_mid":
                    returnInfo.tcg_mid = subObject.str;
                    break;
                case "tcg_avg_foil":
                    returnInfo.tcg_avg_foil = subObject.str;
                    break;
                case "tcg_link":
                    returnInfo.tcg_link = subObject.str;
                    break;
                case "gatherer_link":
                    returnInfo.gatherer_link = subObject.str;
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
