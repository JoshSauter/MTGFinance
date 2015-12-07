using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CardVersionResult : MonoBehaviour {
	Text buttonText;
	Button button;
	public int multiverseID = -1;

	void Awake() {
		buttonText = GetComponentInChildren<Text>();
		button = GetComponent<Button>();
	}

	public void SetVersion(int multiverseID, string cardName, string setName) {
		buttonText.text = setName;

		//Make the button perform an action when clicked
		string cardNameTemp = cardName;
		int multiverseIDTemp = multiverseID;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(delegate {
			MenuManager.S.DisplayCardInfo(cardNameTemp, multiverseIDTemp);
			SearchBar.S.DeleteSearchResults();
			SearchBar.S.ClearSearchTerms();
		});
	}
}
