using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
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

	void Start() {
		switch (ButtonAction) {
			case ButtonAction.StartLevel:
				GetComponent<Button>().onClick.AddListener(StartLevel);
				break;
			case ButtonAction.QuitGame:
				GetComponent<Button>().onClick.AddListener(QuitGame);
				break;
			case ButtonAction.RestartScene:
				GetComponent<Button>().onClick.AddListener(RestartScene);
				break;
			case ButtonAction.Pause:
				//GetComponent<Button>().onClick.AddListener(Pause);
				GetComponent<Button>().onClick.AddListener(TogglePause);
				break;
			case ButtonAction.Resume:
				GetComponent<Button>().onClick.AddListener(ResumePausedScene);
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
}
