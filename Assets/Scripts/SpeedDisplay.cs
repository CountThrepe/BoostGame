using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour {
	private float speed = 5;

	public Text speedText;
	public string title = "Speed: ";

	void Update() {
		speedText.text = title + speed.ToString("0.00");
	}

	public void setSpeed(float s) {
		speed = s;
	}
}
