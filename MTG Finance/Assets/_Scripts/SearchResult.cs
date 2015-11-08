using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SearchResult : MonoBehaviour {
	Text buttonText;
	Button button;

	void Awake() {
		buttonText = GetComponentInChildren<Text>();
		button = GetComponent<Button>();
	}

	public string resultName {
		get {
			return buttonText.text;
		}
		set {
			//Special case for All Results button
			if (value == "-- All Results --") {
                buttonText.text = value;
				return;
			}
			
			if (!CardDictionary.Cards.ContainsKey(value)) {
				Debug.LogError("No card with name " + value + " found in dictionary.");
				return;
			}
			buttonText.text = value;

			//Make the button perform an action when clicked
			string temp = value;
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate {
				MenuManager.S.DisplayCardInfo(temp);
				SearchBar.S.DeleteSearchResults();
				SearchBar.S.ClearSearchTerms();
			});
		}
	}
}
