using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;



public class ScoreMultiplierWidgetScript : MonoBehaviour {

	public Text MultiplierText;
	public Text NotificationText;
	public Image TimerBar;
	public float NotificationTime;
	public AnimationCurve TimerCurve;

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
