﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.Input;
using Assets.Scripts;
using UnityEngine.SceneManagement;

public class TimeCounterWidgetScript : MonoBehaviour {

	[Tooltip("time in seconds")]
	public float time;
	private Timer timer;

	private Text text;

	public GameObject GameOverPanel;

	public bool RestartOnEnd = false;
	public string NextScene;


	void Start() {

		text = GetComponent<Text>();
		GameOverPanel.SetActive(false);

		timer = new Timer(time);

	}


	void Update() {

		if (Keyboard.current.pKey.wasPressedThisFrame) {
			timer.Stop();
			//Cursor.lockState = CursorLockMode.None;
			EndLevel();

		}

		if (timer.Update()) {

			text.text = "0:00:00";

			// TODO: do something when time runs out, change scene? trigger game over?
			//Cursor.lockState = CursorLockMode.Confined;
			//Cursor.visible = true;
			EndLevel();

		} else if (timer.IsRunning()) {

			int timeLeft = Mathf.FloorToInt(timer.TimeLeft() * 100);

			int minutes = timeLeft / 6000;
			int seconds = (timeLeft % 6000) / 100;
			int tensOfMilliseconds = (timeLeft % 6000) % 100;

			text.text = "" + minutes + ":" + seconds.ToString("00") + ":" + tensOfMilliseconds.ToString("00");

		}

	}

	private void EndLevel() {
		if (RestartOnEnd) {
			Globals.FadePanel.StartLevelTransition(SceneManager.GetActiveScene().name);
		} else {
			Globals.FadePanel.StartLevelTransition(NextScene);
		}

		GameOverPanel.SetActive(true);
	}
}
