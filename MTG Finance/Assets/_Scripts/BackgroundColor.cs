using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BackgroundColor : MonoBehaviour {
	enum State {
		redToPink,
		pinkToBlue,
		blueToTeal,
		tealToGreen,
		greenToYellow,
		yellowToRed
	}
	State state = State.redToPink;

	Color red = new Color(206f/255f, 120f/255f, 120f/255f, 178f/255f);
	Color pink = new Color(206f/255f, 120f/255f, 206/255f, 178f/255f);
	Color blue = new Color(120/255f, 120f/255f, 206/255f, 178f/255f);
	Color teal = new Color(120/255f, 206/255f, 206/255f, 178f/255f);
	Color green = new Color(120/255f, 206/255f, 120/255f, 178f/255f);
	Color yellow = new Color(206/255f, 206/255f, 120/255f, 178f/255f);

	float timeIntoTransition = 0; //How much time has passed since the last transition began
	public float transitionTime = 30f; //How many seconds it takes to change from one color to another

	public Image background;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Increment timer
		timeIntoTransition += Time.deltaTime;

		//Check if we should state change
		if (timeIntoTransition >= transitionTime) {
			timeIntoTransition = 0;
			ChangeState();
		}
		
		//Calculate the percentage to use into this transition
		float percentage = timeIntoTransition/transitionTime;

		switch (state) {
			case State.redToPink:
				background.color = Color.Lerp(red, pink, percentage);
				break;
			case State.pinkToBlue:
				background.color = Color.Lerp(pink, blue, percentage);
				break;
			case State.blueToTeal:
				background.color = Color.Lerp(blue, teal, percentage);
				break;
			case State.tealToGreen:
				background.color = Color.Lerp(teal, green, percentage);
				break;
			case State.greenToYellow:
				background.color = Color.Lerp(green, yellow, percentage);
				break;
			case State.yellowToRed:
				background.color = Color.Lerp(yellow, red, percentage);
				break;
		}
	}

	void ChangeState() {
		switch (state) {
			case State.redToPink:
				state = State.pinkToBlue;
				return;
			case State.pinkToBlue:
				state = State.blueToTeal;
				return;
			case State.blueToTeal:
				state = State.tealToGreen;
				return;
			case State.tealToGreen:
				state = State.greenToYellow;
				return;
			case State.greenToYellow:
				state = State.yellowToRed;
				return;
			case State.yellowToRed:
				state = State.redToPink;
				return;
		}
	}
}
