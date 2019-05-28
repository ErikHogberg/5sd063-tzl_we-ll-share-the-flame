using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;



public class ScoreMultiplierWidgetScript : MonoBehaviour {

	public Text MultiplierText;
	public Image MultiplierBackground;
	public Text NotificationText;
	public Image TimerBar;
	public float NotificationTime;
	public AnimationCurve TimerCurve;

	private float ScoreMultiplierBuffer;

	[Serializable]
	public class MultiplierMaterialSettings {
		public Material TextBackground;
		public float MultiplierStartThreshold;
	}

	public List<MultiplierMaterialSettings> MaterialSettings;

	private class ScoreNotification {
		public Timer Timer;
		public string Text;

		public ScoreNotification(float time, string text) {
			Timer = new Timer(time);
			Text = text;
		}
	}

	private List<ScoreNotification> notifications;

	void Start() {
		Globals.ScoreMultiplierPanel = this;
		notifications = new List<ScoreNotification>();
	}

	void Update() {
		string outText = "";

		float timerBarProgress = 0f;

		// TODO: limit number of lines shown, only show newest
		notifications.RemoveAll((n) => {
			if (n.Timer.Update()) {
				return true;
			} else {
				outText += n.Text;
				if (timerBarProgress < n.Timer.TimeLeft()) {
					timerBarProgress = n.Timer.TimeLeft();
				}
				return false;
			}
		});

		if (notifications.Count < 1) {
			Globals.ScoreMultiplier = 1f;
		}

		Vector3 scale = TimerBar.transform.localScale;
		scale.x = TimerCurve.Evaluate(timerBarProgress / NotificationTime);
		TimerBar.transform.localScale = scale;

		// TODO: set material

		Material material = null;
		float scoreThreshold = -1f;
		bool bufferInSameThreshold = false;
		foreach (MultiplierMaterialSettings item in MaterialSettings) {
			if (item.MultiplierStartThreshold > scoreThreshold && item.MultiplierStartThreshold < Globals.ScoreMultiplier) {
				material = item.TextBackground;
				scoreThreshold = item.MultiplierStartThreshold;
			}
		}

		if (!bufferInSameThreshold && material != null) {
			MultiplierBackground.material = material;
		}

		ScoreMultiplierBuffer = Globals.ScoreMultiplier;

		NotificationText.text = outText;
		MultiplierText.text = "x" + Globals.ScoreMultiplier.ToString("F1");
	}

	public float AddPoints(float points) {
		notifications.Add(new ScoreNotification(
			NotificationTime,
			"+" + Globals.ScoreMultiplier.ToString("F1")
			+ "x" + points.ToString("F0") + Environment.NewLine
			));
		return points * Globals.ScoreMultiplier;
	}
}
