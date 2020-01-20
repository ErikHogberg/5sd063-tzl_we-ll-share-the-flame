using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Utilities;
using UnityEngine;
using UnityEngine.UI;

public class NotificationWidgetScript : MonoBehaviour {

	// TODO: list of notifications that stack automatically

	private Text text;
	private Image background;

	public float Time = 1f;
	private Timer timer;

	public AnimationCurve FadeCurve;

	void Start() {
		Globals.NotificationPanel = this;
		timer = new Timer(Time);

		background = GetComponent<Image>();
		text = GetComponentInChildren<Text>();

		gameObject.SetActive(false);
	}


	void Update() {
		if (timer.Update()) {
			gameObject.SetActive(false);
		} else {
			{
				Color color = background.color;
				color.a = FadeCurve.Evaluate(timer.TimeLeft() / Time);
				background.color = color;
			}

			{
				Color color = text.color;
				color.a = FadeCurve.Evaluate(timer.TimeLeft() / Time);
				text.color = color;
			}
		}
	}

	public void Notify(string text) {
		gameObject.SetActive(true);
		this.text.text = text;
		timer.Restart(Time);
	}

}
