using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class StartZoneScript : MonoBehaviour {

	private bool notInit = true;

	private void Update() {
		if (notInit) {
			Globals.TimerPanel.StopCountdown();
			notInit = false;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.CompareTag("Player")) {
			Globals.TimerPanel.StartCountdown();
    
            // TODO: wiimote calibration process
            // TODO: disable wheel movement until calibration is done
            // IDEA: disable foam/water until timer starts
	
    		gameObject.SetActive(false);
		}


	}
}
