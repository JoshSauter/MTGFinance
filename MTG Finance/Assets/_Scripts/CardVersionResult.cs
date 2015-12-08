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

	public void SetVersion(string setName, JSONObject versionInfo) {
        buttonText.text = setName;

		//Make the button perform an action when clicked
		string setNameTemp = setName;
		button.onClick.RemoveAllListeners();
		button.onClick.AddListener(delegate {
			MenuManager.S.DisplayCardInfo(setNameTemp, versionInfo);
			SearchBar.S.DeleteSearchResults();
			SearchBar.S.ClearSearchTerms();
		});
	}
}
