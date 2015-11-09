using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputFieldMobile : MonoBehaviour {
	string lastText = "";
	Text inputFieldText;
	TouchScreenKeyboard keyboard;

	// Use this for initialization
	void Start () {
		inputFieldText = transform.FindChild("Text").GetComponent<Text>();
	}
	
	public void OpenKeyboard() {
		if (keyboard != null && keyboard.active)
			return;

		keyboard = TouchScreenKeyboard.Open("");
	}

	// Update is called once per frame
	void Update () {
		if (keyboard != null && keyboard.active && ValueChanged()) {
			print("ValueChanged");
			SearchBar.S.SearchFor();
			lastText = keyboard.text;
			inputFieldText.text = keyboard.text;
		}
	}

	bool ValueChanged() {
		return (lastText != keyboard.text);
	}
}
