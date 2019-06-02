using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class DaylightScript : MonoBehaviour {

	public AnimationCurve SunsetCurve;
	public float StartSunsetTimeLeft = 60f;
	public float FinishSunsetTimeLeft = 30f;

	private Light sunlight;

	void Start() {
		sunlight = GetComponent<Light>();
	}

	void Update() {
		float timeLeft = Globals.TimerPanel.TimeLeft();

		if (Globals.StartZone.isActiveAndEnabled) {
			return;
		}

		if (!Globals.TimerPanel.IsRunning() || timeLeft < FinishSunsetTimeLeft) {
			// NOTE: night
			sunlight.intensity = 0f;
		} else if (timeLeft < StartSunsetTimeLeft) {
			// NOTE: sunset
			float sunsetPercentage = (timeLeft - FinishSunsetTimeLeft) / (StartSunsetTimeLeft - FinishSunsetTimeLeft);
			// Debug.Log("sunset: " + sunsetPercentage);
			sunlight.intensity = SunsetCurve.Evaluate(sunsetPercentage);

		} else {
			// NOTE: day
			sunlight.intensity = 1f;
		}

	}
}
