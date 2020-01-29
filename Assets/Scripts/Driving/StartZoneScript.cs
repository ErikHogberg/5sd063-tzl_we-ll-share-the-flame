using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
// using UnityEngine.Experimental.Input;

public class StartZoneScript : MonoBehaviour {

	public AudioSource Music;
	public AudioSource PreStartMusic;

	private bool notInit = true;
	private bool enterNotified = false;

	private void Start() {
		Globals.StartZone = this;
		Globals.DisableScoreCollection = true;
	}

	private void Update() {
		if (notInit) {
			Globals.TimerPanel.StopCountdown();
			// Globals.Player.DisableMovement = true;

			notInit = false;
		}

		Keyboard keyboard = Keyboard.current;



		// TODO: wiimote calibration process
		// IDEA: disable foam/water until timer starts

		// TODO: assign wiimote to global in nozzle script (or other script?), if not assigned yet, check if this makes calibration persistent
		// TODO: button to recalibrate

	}

	private void OnTriggerEnter(Collider other) {
		if (other.CompareTag("Player") && !enterNotified) {
			Globals.CollectibleNotificationPanel.Notify("Leave the circle to start!");
			enterNotified = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			Globals.TimerPanel.StartCountdown();
			Globals.DisableScoreCollection = false;

			Globals.Nozzle.DisableFiring = false;
			if (Globals.CollectibleNotificationPanel != null) {
				Globals.CollectibleNotificationPanel.Notify("Game Start!");
			}

			if (Music != null) {
				Music.Play();
			}
			if (PreStartMusic != null) {
				PreStartMusic.Stop();
			}

			gameObject.SetActive(false);
		}


	}
}
