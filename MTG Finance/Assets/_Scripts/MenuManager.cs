using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour {
	public static MenuManager S;

	public GameObject menuCanvas;
	public GameObject searchPanel;
	public GameObject cardInfoPanel;
	public GameObject cardVersionsPanel;

	public GameObject editorInputField;
	public GameObject mobileInputField;

	float transitionTime = 0.375f; //How long panels take to swap out and in
	Vector2 panelLeftPosition = new Vector2(-1200f,0); //Resting position offscreen left
	Vector2 panelRightPosition = new Vector2(1200f,0); //Resting position offscreen right

	bool inCoroutine = false;

	enum States {
		search,
		watchlist,
		cardVersions,
		cardInfo
	}
	States state = States.search;

	void Awake() {
		S = this;

		if (Application.isEditor) {
			editorInputField.SetActive(true);
			mobileInputField.SetActive(false);
			SearchBar.S = editorInputField.GetComponent<SearchBar>();
		}
		else {
			editorInputField.SetActive(false);
			mobileInputField.SetActive(true);
			SearchBar.S = mobileInputField.GetComponent<SearchBar>();
		}
	}

	// Use this for initialization
	void Start() {
		//Search through all UI elements to find buttons
		foreach (Transform canvasChild in menuCanvas.transform) {
			UIPanel panel = canvasChild.GetComponent<UIPanel>();
			if (panel == null) {
				continue;
			}

			foreach (Button button in panel.buttons) {
				//Temp copy means that each button will have it's own copy rather than a reused reference
				Button tempButton = button;
				//Set up each button
				button.onClick.RemoveAllListeners();
				button.onClick.AddListener(delegate {
					MenuManager.S.Click(tempButton);
				});
			}
		}
		
		StartCoroutine(PanelInCoroutine(searchPanel));
	}

	public void Click(Button button) {
		Click(button.name);
	}
	public void Click(string name) {
		print("Button " + name + " clicked\nState: " + state.ToString());
		Command(name);
	}
	
	void Command(string command) {
		switch (state) {
			case States.search:
				switch (command) {
					case "Search":
						state = States.search;
						break;
					case "Watchlist":
						state = States.watchlist;
						break;
					case "Menu":
						Quit();
						break;
				}
				break;

			case States.cardVersions:
				switch (command) {
					case "Search":
						if (!inCoroutine) {
							PanelIn(searchPanel);
							state = States.search;
						}
						break;
					case "Watchlist":
						state = States.watchlist;
						break;
					case "Menu":
						if (!inCoroutine) {
							PanelIn(searchPanel);
							state = States.search;
						}
						break;
				}
				break;

			case States.cardInfo:
				switch (command) {
					case "Search":
						if (!inCoroutine) {
							PanelIn(searchPanel);
							state = States.search;
						}
						break;
					case "Watchlist":
						state = States.watchlist;
						break;
					case "Menu":
						if (!inCoroutine) {
							PanelIn(cardVersionsPanel);
							state = States.cardVersions;
						}
						break;
				}
				break;

			case States.watchlist:
				switch (command) {
					case "Search":
						if (!inCoroutine) {
							PanelIn(searchPanel);
							state = States.search;
						}
						break;
					case "Watchlist":
						state = States.watchlist;
						break;
					case "Menu":
						if (!inCoroutine) {
							PanelIn(searchPanel);
							state = States.search;
						}
						break;
				}
				break;
		}
	}

	void Quit() {
		Application.Quit();
	}

	public void PopulateVersionList(string cardName) {
		print(cardName);
		PanelIn(cardVersionsPanel);
		state = States.cardVersions;

		cardVersionsPanel.GetComponent<CardVersions>().DisplayInformationFor(cardName);
	}

	public void DisplayCardInfo(string cardName, int multiverseID) {
		print(multiverseID);
		PanelIn(cardInfoPanel);
		state = States.cardInfo;

		cardInfoPanel.GetComponent<CardInfo>().DisplayInformationFor(cardName, multiverseID);
	}

	//Make sure to call PanelIn(GameObject panelIn) before changing state
	void PanelIn(GameObject panelIn) {
		switch (state) {
			case States.search:
				StartCoroutine(PanelOutCoroutine(searchPanel));
				break;
			case States.cardVersions:
				StartCoroutine(PanelOutCoroutine(cardVersionsPanel));
				break;
			case States.cardInfo:
				StartCoroutine(PanelOutCoroutine(cardInfoPanel));
				break;
			case States.watchlist:
				break;
		}
		StartCoroutine(PanelInCoroutine(panelIn));
	}
	public IEnumerator PanelInCoroutine(GameObject panelIn) {
		inCoroutine = true;
		RectTransform panelRect = panelIn.GetComponent<RectTransform>();
		float timeElapsed = 0;
		while (timeElapsed < transitionTime) {
			timeElapsed += Time.deltaTime;
			panelRect.anchoredPosition = Vector2.Lerp(panelLeftPosition, Vector2.zero, timeElapsed / transitionTime);
			yield return 0;
		}

		panelRect.anchoredPosition = Vector2.zero;
		inCoroutine = false;
	}
	IEnumerator PanelOutCoroutine(GameObject panelOut) {
		inCoroutine = true;
		RectTransform panelRect = panelOut.GetComponent<RectTransform>();
		float timeElapsed = 0;
		while (timeElapsed < transitionTime) {
			timeElapsed += Time.deltaTime;
			panelRect.anchoredPosition = Vector2.Lerp(Vector2.zero, panelRightPosition, timeElapsed/transitionTime);
			yield return 0;
		}

		panelRect.anchoredPosition = panelLeftPosition;
		inCoroutine = false;
	}
}
