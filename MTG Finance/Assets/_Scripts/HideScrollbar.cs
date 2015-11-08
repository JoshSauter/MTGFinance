using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HideScrollbar : MonoBehaviour {
	public GameObject 	contentsPanel;
	Scrollbar 			scrollbar;
	ScrollRect			scrollRect;

	// Use this for initialization
	void Start () {
		scrollRect = this.gameObject.GetComponent<ScrollRect>();
		scrollbar = scrollRect.verticalScrollbar;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void FixedUpdate() {
//		print("contentsSize = " + contentsPanel.GetComponent<RectTransform>().rect.height +  
//			  "\nscrollRectSize = " + this.GetComponent<RectTransform>().rect.height);
		if (Mathf.Abs(contentsPanel.GetComponent<RectTransform>().rect.height) >
			Mathf.Abs(this.GetComponent<RectTransform>().rect.height)) {
			scrollbar.gameObject.SetActive(true);
			//scrollRect.vertical = true;
		}
		else {
			scrollbar.gameObject.SetActive(false);
			//scrollRect.vertical = false;
		}
	}
}
