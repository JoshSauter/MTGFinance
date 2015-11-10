using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using JSONObj;

public class CardDictionary : MonoBehaviour {
	public static List<string> CardNames = new List<string>();
	public static Dictionary<string, Card> Cards = new Dictionary<string, Card>();

	// Use this for initialization
	void Start () {
		BuildDictionary();

		//After dictionary is done building, bring in the search panel
		StartCoroutine(MenuManager.S.PanelInCoroutine(MenuManager.S.searchPanel));

		string test = "Æther";
		print (test.Replace ("Æ", "AE"));
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void BuildDictionary() {
		TextAsset fileTextAsset = Resources.Load<TextAsset>("AllSets");
		JSONObject allSetsJso = new JSONObject(fileTextAsset.text);
		foreach (var setJso in allSetsJso.list) {
			foreach (var cardJso in setJso.GetField("cards").list) {
				Card newCard = Card.JSONToCard(cardJso);
				if (CardNames.Contains(newCard.cardName)){
					continue;
				}
				CardNames.Add(newCard.cardName);
				Cards[newCard.cardName] = newCard;
				//if (Random.Range(0,100) < 1) {
				//	print(newCard.ToString());
				//}
			}
		}
		Resources.UnloadAsset(fileTextAsset);

		CardNames.Sort();
	}

	//Returns all card names matching a particular search
	public static List<string> AllCardsContaining(string search) {
		//Non-case-sensitive search
		search = search.ToLower();
		string[] searchWords = search.Split(' ');
		List<string> returnList = new List<string>();

		foreach (string fullname in CardNames) {
			string lowerFullname = fullname.Replace("æ", "ae").Replace("Æ", "AE").ToLower();
			string[] nameWords = lowerFullname.Split(' ');

			//Assume a word is valid until proven otherwise
			bool addWord = true;
			for (int i = 0; i < searchWords.Length; i++) {
				//If we've searched for more than just a card's name, don't add it
				if (i >= nameWords.Length) {
					addWord = false;
					break;
				}
				//Look for mismatches for each individual word in the search
				if (searchWords[i] != nameWords[i]) {
					//Allow mismatch only if the last word & search is on the right track (incomplete search word)
					if (i == searchWords.Length - 1) {

						//Check to see if search matches beginning of card name at this word
						for (int j = 0; j < searchWords[i].Length; j++) {
							//If the partial search term is longer than the card name, don't add this card
							if (j >= nameWords[i].Length) {
								addWord = false;
								break;
							}
							//If there's a character mismatch, don't add this card
							if (searchWords[i][j] != nameWords[i][j]) {
								addWord = false;
								break;
							}
						}
						break;
					}
					//Else if there's a mismatch, don't add the card name
					addWord = false;
				}
			}
			//If we haven't decided that this card DOESN'T match, then return it -- it's a match
			if (addWord) {
				returnList.Add(fullname);
			}
		}
		return returnList;
	}
}
