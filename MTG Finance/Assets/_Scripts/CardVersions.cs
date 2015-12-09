using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using JSONObj;

public class CardVersions : MonoBehaviour {
	public static CardVersions S;
	public Text cardNameText;

	public GameObject versionResultPrefab;
	public Transform versionsPanel;
	List<CardVersionResult> versionResults = new List<CardVersionResult>();
	List<JSONObject> allResults = new List<JSONObject>();

    ServerComm server;

	string curCard;
    int maxNumberResultsShown = 10;
	int page = 0;

	void Awake() {
		S = this;
	}

    ///Look at SearchBar.SearchFor(string search) to see how to populate the versionsPanel in this function
    public void DisplayInformationFor(string cardName) {
		DeleteVersionResults();
		allResults.Clear();
		page = 0;
		curCard = cardName;

        //Get the list of all of the versions of the searched card
        //Then build a dictionary out of that list (indexed with multiverse IDs)
        JSONObject curListJSON = server.RequestCardList(cardName);
		cardNameText.text = cardName;

        //Display the set names for each set the card appears in
        int curResults = 0;
		page = 0;
		allResults = curListJSON["cards"].list;
        foreach (var setInfo in allResults){
            if (curResults >= maxNumberResultsShown) {
                GameObject moreResultsGameObject = Instantiate(versionResultPrefab);
                CardVersionResult moreResults = moreResultsGameObject.GetComponent<CardVersionResult>();
                moreResultsGameObject.transform.SetParent(versionsPanel);
				moreResults.SetVersion(cardName,"-- Page Down --", setInfo);
                versionResults.Add(moreResults);
                break;
            }

            GameObject newResultGameObject = Instantiate(versionResultPrefab);
            CardVersionResult newResult = newResultGameObject.GetComponent<CardVersionResult>();
            newResultGameObject.transform.SetParent(versionsPanel);
            //Set the version of the card
            newResult.SetVersion(cardName,setInfo["set_display_name"].str, setInfo);

			curResults++;
            versionResults.Add(newResult);
        }

		//Debug.LogError("Stub function CardVersions.DisplayInformationFor(string cardName) called.");
	}

	public void PageDown() {
		page++;
		DeleteVersionResults();
		for (int i = page*maxNumberResultsShown - 1; i < (page+1)*maxNumberResultsShown && i < allResults.Count; i++) {
			JSONObject setInfo = allResults[i];
			if (i == (page+1)*maxNumberResultsShown-1) {
				GameObject moreResultsGameObject = Instantiate(versionResultPrefab);
				CardVersionResult moreResults = moreResultsGameObject.GetComponent<CardVersionResult>();
				moreResultsGameObject.transform.SetParent(versionsPanel);
				moreResults.SetVersion(curCard,"-- Page Down --", setInfo);
				versionResults.Add(moreResults);
				break;
			}
			else if (i == page*maxNumberResultsShown-1) {
				GameObject moreResultsGameObject = Instantiate(versionResultPrefab);
				CardVersionResult moreResults = moreResultsGameObject.GetComponent<CardVersionResult>();
				moreResultsGameObject.transform.SetParent(versionsPanel);
				moreResults.SetVersion(curCard,"-- Page Up --", setInfo);
				versionResults.Add(moreResults);
				continue;
			}

			GameObject newResultGameObject = Instantiate(versionResultPrefab);
			CardVersionResult newResult = newResultGameObject.GetComponent<CardVersionResult>();
			newResultGameObject.transform.SetParent(versionsPanel);
			//Set the version of the card
			newResult.SetVersion(curCard,setInfo["set_display_name"].str, setInfo);
			
			versionResults.Add(newResult);
		}
	}

	public void PageUp() {
		page--;
		DeleteVersionResults();

		//Page Up Button
		if (page != 0) {
			GameObject moreResultsUpGameObject = Instantiate(versionResultPrefab);
			CardVersionResult moreResultsUp = moreResultsUpGameObject.GetComponent<CardVersionResult>();
			moreResultsUpGameObject.transform.SetParent(versionsPanel);
			moreResultsUp.SetVersion(curCard,"-- Page Up --", new JSONObject());
			versionResults.Add(moreResultsUp);
		}

		for (int i = page * maxNumberResultsShown; i < (page + 1) * maxNumberResultsShown && i < allResults.Count; i++) {
			JSONObject setInfo = allResults[i];
			//Page Up Button
			if (i == (page + 1) * maxNumberResultsShown - 1) {
				GameObject moreResultsGameObject = Instantiate(versionResultPrefab);
				CardVersionResult moreResults = moreResultsGameObject.GetComponent<CardVersionResult>();
				moreResultsGameObject.transform.SetParent(versionsPanel);
				moreResults.SetVersion(curCard,"-- Page Down --", setInfo);
				versionResults.Add(moreResults);
				break;
			}

			GameObject newResultGameObject = Instantiate(versionResultPrefab);
			CardVersionResult newResult = newResultGameObject.GetComponent<CardVersionResult>();
			newResultGameObject.transform.SetParent(versionsPanel);
			//Set the version of the card
			newResult.SetVersion(curCard,setInfo["set_display_name"].str, setInfo);

			versionResults.Add(newResult);
		}
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
