using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using JSONObj;

//Wrapper class for the JSON information on cards
public class Card {
	public string cardName;			//The name of the card
	public List<string> cardNames;	//Only used for split, flip, and double-faced cards.
	public string layout;			//The layout of the card. e.g. normal, split, double-faced, etc.
	public string manaCost;			//Mana cost in a form like {3}{U}{B}.
	public int CMC;					//Converted Mana Cost.
	public List<string> colors;		//All colors the card belongs to.
	public string type;				//Text of the card's type.
	public List<string> supertypes;	//e.g. Basic, Legendary, Snow, etc.
	public List<string> types;		//Type of the card. e.g. Instant, Creature, Land, etc.
	public List<string> subtypes;	//Subtypes of the card. e.g. Arcane, Human, Equipment, etc.
	public string rarity;			//Rarity of the card.
	public string text;				//Text on the card.
	public string flavor;			//Flavortext on the card, if any.
	public string artist;			//Artist of the card.
	public string number;			//Card number. String because some numbers have letters in them.
	public string power;			//Power of the card. String because some cards have powers like "1+*"
	public string toughness;		//Toughness of the card. String because some cards have toughness like "1+*"
	public int loyalty;				//The loyalty of the card. This is only present for planeswalkers.
	public int multiverseID;		//MultiverseID of the card on Wizard's Gatherer web page.
	public List<int> variations;    //If a card has alternate art, this will contain the multiverseIDs of each variation.
	public string watermark;        //The watermark on the card.

	public Card() {
		cardName = "";
		cardNames = new List<string>();
		layout = "";
		manaCost = "";
		CMC = -1;
		colors = new List<string>();
		type = "";
		supertypes = new List<string>();
		types = new List<string>();
		subtypes = new List<string>();
		rarity = "";
		text = "";
		flavor = "";
		artist = "";
		number = "";
		power = "";
		toughness = "";
		loyalty = -1;
		multiverseID = -1;
		variations = new List<int>();
		watermark = "";
    }

	//Turns a JSONObject into a Card object
	public static Card JSONToCard(JSONObject jso) {
		Card returnCard = new Card();

		foreach (string key in jso.keys) {
			//Retrive the part of the JSON referenced by the key
			JSONObject subObject = (JSONObject)jso.GetField(key);

			//Perform the appropriate action for the field
			switch (key) {
				case "name":
					returnCard.cardName = subObject.str.Replace("\\\"","\"");
					break;
				case "names":
					returnCard.cardNames.Clear();
					foreach (JSONObject name in subObject.list) {
						returnCard.cardNames.Add(name.str);
					}
					break;
				case "layout":
					returnCard.layout = subObject.str;
					break;
				case "manaCost":
					returnCard.manaCost = subObject.str;
					break;
				case "cmc":
					returnCard.CMC = (int)subObject.n;
					break;
				case "colors":
					returnCard.colors.Clear();
					foreach (JSONObject color in subObject.list) {
						returnCard.colors.Add(color.str);
					}
					break;
				case "type":
					returnCard.type = subObject.str;
					break;
				case "supertypes":
					returnCard.supertypes.Clear();
					foreach (JSONObject supertype in subObject.list) {
						returnCard.supertypes.Add(supertype.str);
					}
					break;
				case "types":
					returnCard.types.Clear();
					foreach (JSONObject type in subObject.list) {
						returnCard.types.Add(type.str);
					}
					break;
				case "subtypes":
					returnCard.subtypes.Clear();
					foreach (JSONObject subtype in subObject.list) {
						returnCard.subtypes.Add(subtype.str);
					}
					break;
				case "rarity":
					returnCard.rarity = subObject.str;
					break;
				case "text":
					returnCard.text = subObject.str.Replace("\\n","\n").Replace("\\\"","\"");
					break;
				case "flavor":
					returnCard.flavor = subObject.str;
					break;
				case "artist":
					returnCard.artist = subObject.str;
					break;
				case "number":
					returnCard.number = subObject.str;
					break;
				case "power":
					returnCard.power = subObject.str;
					break;
				case "toughness":
					returnCard.toughness = subObject.str;
					break;
				case "loyalty":
					returnCard.loyalty = (int)subObject.n;
					break;
				case "multiverseid":
					returnCard.multiverseID = (int)subObject.n;
					break;
				case "variations":
					returnCard.variations.Clear();
					foreach (JSONObject variation in subObject.list) {
						returnCard.variations.Add((int)variation.n);
					}
					break;
				case "watermark":
					returnCard.watermark = subObject.str;
					break;
			}
		}

		return returnCard;
	}

	//Creates a simple JSONObject containing only name and multiverseid
	public static JSONObject CardToSimpleJSON(Card card) {
		JSONObject cardJSON = new JSONObject(JSONObject.Type.ARRAY);
		cardJSON.AddField("name", card.cardName);
		cardJSON.AddField("multiverseid", card.multiverseID);

		return cardJSON;
	}

	//Overrides C#'s ToString() operator to display card information
	public override string ToString() {
		string retString = "Card Name:\t" + cardName;
		if (manaCost != "") {
			retString += "\nMana Cost:\t" + manaCost;
		}
		if (CMC != -1) {
			retString += "\nCMC:\t\t" + CMC;
		}

		if (colors.Count > 0) {
			retString += "\nColors:\t\t";
			foreach (string color in colors) {
				retString += color;
				if (color != colors[colors.Count-1]) {
					retString += ", ";
				}
			}
		}

		retString += "\nType:\t\t" + type;
		if (rarity != "") {
			retString += "\nRarity:\t\t" + rarity;
		}
		retString += "\nText:\t\t" + text;
		if (flavor != "") {
			retString += "\nFlavor Text:\t" + flavor;
		}

		if (power != "") {
			retString += "\nPower:\t\t" + power;
		}
		if (toughness != "") {
			retString += "\nToughness:\t" + toughness;
		}
		if (loyalty != -1) {
			retString += "\nLoyalty:\t\t" + loyalty;
		}

		if (multiverseID != -1) {
			retString += "\nMultiverseID:\t" + multiverseID;
		}

		return retString;
	}
}
