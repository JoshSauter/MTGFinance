using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SearchBar : MonoBehaviour {
	public static SearchBar S;
	public Text inputFieldText;
	public Transform searchPanel;
	public GameObject searchResultPrefab;

	List<SearchResult> searchResults = new List<SearchResult>();
	int maxNumberResultsShown = 10;

	// Use this for initialization
	void Awake () {
		S = this;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	//Will search for whatever is currently in the search input field
	public void SearchFor() {
		Invoke("SearchForHelper", 0.02f);
	}
	void SearchForHelper() {
		if (inputFieldText.text.Length > 1) {
			SearchFor(inputFieldText.text);
		}
		else {
			DeleteSearchResults();
		}
	}
	public void SearchFor(string search) {
		List<string> matches = CardDictionary.AllCardsContaining(search);
		DeleteSearchResults();
		ClearConsole();
		print("Number of matches: " + matches.Count);
		for (int i = 0; i < maxNumberResultsShown && i < matches.Count; i++) {
			GameObject newResultGameObject = Instantiate(searchResultPrefab);
			SearchResult newResult = newResultGameObject.GetComponent<SearchResult>();
            newResultGameObject.transform.SetParent(searchPanel);
			newResult.resultName = matches[i];

			searchResults.Add(newResult);
		}
		//Allow the option of showing more results than the first 10
		if (matches.Count > maxNumberResultsShown) {
			GameObject moreResultsGameObject = Instantiate(searchResultPrefab);
			SearchResult moreResults = moreResultsGameObject.GetComponent<SearchResult>();
			moreResultsGameObject.transform.SetParent(searchPanel);
			moreResults.resultName = "-- All Results --";

			searchResults.Add(moreResults);
		}

	}

	public void DeleteSearchResults() {
		foreach (SearchResult item in searchResults) {
			Destroy(item.gameObject);
		}
		searchResults.Clear();
	}

	public void ClearSearchTerms() {
		inputFieldText.gameObject.GetComponentInParent<InputField>().text = "";
		inputFieldText.text = "";
	}

	static void ClearConsole() {
		// This simply does "LogEntries.Clear()" the long way:
		var logEntries = System.Type.GetType("UnityEditorInternal.LogEntries,UnityEditor.dll");
		var clearMethod = logEntries.GetMethod("Clear", System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);
		clearMethod.Invoke(null, null);
	}
}
