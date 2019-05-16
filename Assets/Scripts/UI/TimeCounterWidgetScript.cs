using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class TimeCounterWidgetScript : MonoBehaviour {

	[Tooltip("time in seconds")]
	public float time;
	private Timer timer;

	private Text text;

	public GameObject GameOverPanel;

	void Start() {

		text = GetComponent<Text>();
		GameOverPanel.SetActive(false);

		timer = new Timer(time);

	}


	void Update() {

		if (timer.Update()) {
			
			text.text = "0:00:00";
			
			// TODO: do something when time runs out, change scene? trigger game over?

			GameOverPanel.SetActive(true);

		} else if (timer.IsRunning()) {

			int timeLeft = Mathf.FloorToInt(timer.TimeLeft() * 100);

			int minutes = timeLeft / 6000;
			int seconds = (timeLeft % 6000) / 100;
			int tensOfMilliseconds = (timeLeft % 6000) % 100;

			text.text = "" + minutes + ":" + seconds.ToString("00") + ":" + tensOfMilliseconds.ToString("00");

		}

	}
}
