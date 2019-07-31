using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiTransitionScript : MonoBehaviour {

	public GameObject GameUI;
	public GameObject HighscoreUI;

	void Start() {
		GameUI.SetActive(true);
		HighscoreUI.SetActive(false);
	}

	void Update() {

	}

	public void GameOver() {
		GameUI.SetActive(true);
		HighscoreUI.SetActive(false);
	}
}
