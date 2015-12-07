using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardVersions : MonoBehaviour {
	public Text cardNameText;

	public GameObject versionResultPrefab;
	public Transform versionsPanel;
	List<CardVersionResult> versionResults = new List<CardVersionResult>();

	///Look at SearchBar.SearchFor(string search) to see how to populate the versionsPanel in this function
	public void DisplayInformationFor(string cardName) {
		Debug.LogError("Stub function CardVersions.DisplayInformationFor(string cardName) called.");
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
