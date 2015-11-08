using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class UIPanel : MonoBehaviour {

	public List<Button> buttons;

	void Awake() {
		buttons = new List<Button>(transform.GetComponentsInChildren<Button>());
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
