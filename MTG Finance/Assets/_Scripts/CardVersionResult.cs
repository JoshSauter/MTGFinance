using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using JSONObj;

public class CardVersionResult : MonoBehaviour {
	public Text buttonText;
	Button button;

	void Awake() {
		buttonText = GetComponentInChildren<Text>();
		button = GetComponent<Button>();
	}

	public void SetVersion(string cardName, string setName, JSONObject versionInfo) {
        buttonText.text = setName;
		//Special case for View More
		if (setName == "-- Page Down --") {
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate {
				CardVersions.S.PageDown();
			});
			return;
		}
		else if (setName == "-- Page Up --") {
			button.onClick.RemoveAllListeners();
			button.onClick.AddListener(delegate {
				CardVersions.S.PageUp();
			});
			return;
		}

		//Make the button perform an action when clicked
		string setNameTemp = setName;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(delegate {
			MenuManager.S.DisplayCardInfo(cardName, setNameTemp, versionInfo);
			SearchBar.S.DeleteSearchResults();
			SearchBar.S.ClearSearchTerms();
		});
	}
}
