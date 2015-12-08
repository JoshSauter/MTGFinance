using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using JSONObj;

public class CardVersions : MonoBehaviour {
	public Text cardNameText;

	public GameObject versionResultPrefab;
	public Transform versionsPanel;
	List<CardVersionResult> versionResults = new List<CardVersionResult>();

    ServerComm server;

    int maxNumberResultsShown = 10;

    ///Look at SearchBar.SearchFor(string search) to see how to populate the versionsPanel in this function
    public void DisplayInformationFor(string cardName) {
		DeleteVersionResults();

        cardNameText.text = cardName;
        //Get the list of all of the versions of the searched card
        //Then build a dictionary out of that list (indexed with multiverse IDs)
        JSONObject curListJSON = server.RequestCardList(cardName);

        //Display the set names for each set the card appears in
        int curResults = 0;
        foreach (var setInfo in curListJSON["cards"].list){
            if (curResults >= maxNumberResultsShown) {
                GameObject moreResultsGameObject = Instantiate(versionResultPrefab);
                CardVersionResult moreResults = moreResultsGameObject.GetComponent<CardVersionResult>();
                moreResultsGameObject.transform.SetParent(versionsPanel);
                moreResults.buttonText.text = "-- All Results --";
                versionResults.Add(moreResults);
                break;
            }

            GameObject newResultGameObject = Instantiate(versionResultPrefab);
            CardVersionResult newResult = newResultGameObject.GetComponent<CardVersionResult>();
            newResultGameObject.transform.SetParent(versionsPanel);
            //Set the version of the card
            newResult.SetVersion(setInfo["set_name"].str, setInfo);

			curResults++;
            versionResults.Add(newResult);
        }

		//Debug.LogError("Stub function CardVersions.DisplayInformationFor(string cardName) called.");
	}

    public void DeleteVersionResults() {
        foreach (CardVersionResult item in versionResults) {
            Destroy(item.gameObject);
        }
        versionResults.Clear();
    }

    // Use this for initialization
    void Start () {
        server = GetComponent<ServerComm>();
    }
	
	// Update is called once per frame
	void Update () {
	
	}
}
