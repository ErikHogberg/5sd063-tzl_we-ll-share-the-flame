using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounterWidgetScript : MonoBehaviour {

    private Text text;

	void Start() {
        text = GetComponent<Text>();
	}

	void Update() {
        text.text = Globals.Score.ToString("F0");
	}
}
