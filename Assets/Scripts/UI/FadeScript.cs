using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour {

	public float Time;
	public bool FadeOut;
	public string NextLevel;

	public AnimationCurve easing;
	public bool ChangeLevel = false;

	private Image blackImage;
	private Timer timer;

	public float timerTime;
	public float alphaTemp;

	void Start() {
		blackImage = GetComponent<Image>();
		timer = new Timer(Time);

		Color color = blackImage.color;
		if (FadeOut) {
			color.a = 0.0f;
		} else {
			color.a = 1.0f;
		}
		blackImage.color = color;

		//easing = AnimationCurve.EaseInOut(0.85f, 0.15f, 0.15f, 0.85f);
	}

	void Update() {

		if (timer.Update()) {
			if (FadeOut && ChangeLevel) {
				SceneManager.LoadScene(NextLevel, LoadSceneMode.Single);
			} else {
				gameObject.SetActive(false);
			}
		}
		CalcAlpha();

		//if (Input.GetKeyDown(KeyCode.L)) {
		//	StartLerpIn();
		//}
		//if (Input.GetKeyDown(KeyCode.K)) {
		//	StartLerpOut();
		//}
		
	}

	public bool IsDone() {
		return !timer.IsRunning();
	}

	public void StartLevelTransition(string Level) {
		FadeOut = true;
		NextLevel = Level;
		ChangeLevel = true;
		timer.Restart(Time);
		gameObject.SetActive(true);
	}

	public void StartLevelTransition() {
		StartLevelTransition(NextLevel);
	}

	public void StartLerpOut() {
		ChangeLevel = false;
		FadeOut = true;
		timer.Restart(Time);
	}

	public void StartLerpIn() {
		FadeOut = false;
		timer.Restart(Time);
	}


	private void CalcAlpha() {

		timerTime = timer.TimeLeft();
		if (!timer.IsRunning()) {
			timerTime = 0.0f;
		}

		float alpha = easing.Evaluate(timerTime / Time);
		if (FadeOut) {
			alpha = 1.0f - alpha;
		}

		alphaTemp = alpha;

		Color color = blackImage.color;
		color.a = alpha;
		blackImage.color = color;

	}

}
