using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class StartZoneScript : MonoBehaviour {

	private bool notInit = true;

	private void Start() {
		Globals.StartZone = this;
	}

	private void Update() {
		if (notInit) {
			Globals.TimerPanel.StopCountdown();
			Globals.Player.DisableMovement = true;

			notInit = false;
		}

		Keyboard keyboard = Keyboard.current;

		if (keyboard.spaceKey.wasPressedThisFrame) {
			Globals.Player.DisableMovement = false;
		}

		// TODO: wiimote calibration process
		// IDEA: disable foam/water until timer starts

		// TODO: assign wiimote to global in nozzle script (or other script?), if not assigned yet, check if this makes calibration persistent
		// TODO: button to recalibrate

	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			Globals.TimerPanel.StartCountdown();


			gameObject.SetActive(false);
		}


	}
}
