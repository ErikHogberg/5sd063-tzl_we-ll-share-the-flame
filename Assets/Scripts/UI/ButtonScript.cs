using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum ButtonAction {
	StartLevel,
	QuitGame,
	RestartScene,
	Pause,
	Resume
}

public class ButtonScript : MonoBehaviour {

	[Tooltip("The action of the button when clicked.")]
	public ButtonAction ButtonAction = ButtonAction.StartLevel;

	[Tooltip("The level/scene to start when clicked, is only used if action is set to StartLevel. Uses the name of the file without the .scene suffix.")]
	public string LevelToStart = "Forest Level";



	public UnityEvent ClickEvents;

	void Start() {
		Button button =	GetComponent<Button>();

		button.onClick.AddListener(TriggerClickEvents);

		switch (ButtonAction) {
			case ButtonAction.StartLevel:
				button.onClick.AddListener(StartLevel);
				break;
			case ButtonAction.QuitGame:
				button.onClick.AddListener(QuitGame);
				break;
			case ButtonAction.RestartScene:
				button.onClick.AddListener(RestartScene);
				break;
			case ButtonAction.Pause:
				//button.onClick.AddListener(Pause);
				button.onClick.AddListener(TogglePause);
				break;
			case ButtonAction.Resume:
				button.onClick.AddListener(ResumePausedScene);
				break;
		}
	}

	void Update() {

	}

	public void StartLevel() {
		StartLevel(LevelToStart);
	}
	public void StartLevel(string level) {
		if (Globals.FadePanel != null) {
			Globals.FadePanel.StartLevelTransition(level);
		} else {
			SceneManager.LoadScene(level, LoadSceneMode.Single);
		}
	}

	public void QuitGame() {
		Application.Quit();
	}

	public void RestartScene() {
		if (Globals.FadePanel != null) {
			Globals.FadePanel.StartLevelTransition(SceneManager.GetActiveScene().name);
		} else {
			SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
		}
	}

	public void TogglePause() {
		//Time.timeScale = Mathf.Approximately(Time.timeScale, 0.0f) ? 1.0f : 0.0f;

	}

	public void Pause() {
		//Globals.PausedScene = SceneManager.GetActiveScene().name;
		SceneManager.LoadScene("Pause Scene", LoadSceneMode.Additive);
	}

	public void ResumePausedScene() {

		//SceneManager.LoadScene(Globals.PausedScene, LoadSceneMode.Additive);
	}

	public void PrepareNetworkReceive() {
		Globals.DriverNetworkMode = NetworkMode.Receive;
	}
	
	public void PrepareNetworkSend() {
		Globals.DriverNetworkMode = NetworkMode.Send;
	}

	public void TriggerClickEvents(){
		ClickEvents.Invoke();
	}

}
