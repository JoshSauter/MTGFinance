using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldMobile : MonoBehaviour {
	public static InputFieldMobile S;
	string lastText = "";
	Text inputFieldText;
	Text placeholderText;
	TouchScreenKeyboard keyboard;

	void Awake() {
		S = this;
	}

	// Use this for initialization
	void Start () {
		inputFieldText = transform.FindChild("Text").GetComponent<Text>();
		placeholderText = transform.FindChild("Placeholder").GetComponent<Text>();
	}
	
	public void OpenKeyboard() {
		if (keyboard != null && keyboard.active)
			return;

		keyboard = TouchScreenKeyboard.Open(inputFieldText.text);
	}

	// Update is called once per frame
	void Update () {
		if (keyboard != null && keyboard.active && ValueChanged()) {
			lastText = keyboard.text;
			SearchBar.S.SearchFor();
			inputFieldText.text = keyboard.text;
			if (keyboard.text != "") {
				placeholderText.text = "";
			}
		}
		else if (keyboard != null && keyboard.done) {
			lastText = keyboard.text;
			if (keyboard.text == "") {
				ResetSearch();
			}
		}
	}

	public void ResetSearch() {
		inputFieldText.text = "";
		placeholderText.text = "Search for card...";
	}

	bool ValueChanged() {
		return (lastText != keyboard.text);
	}
}
