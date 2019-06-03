using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Experimental.Input;
using WiimoteApi;

public class CalibrateWiimoteIRScript : MonoBehaviour {
	void Update() {
		Keyboard keyboard = Keyboard.current;
		if (keyboard.digit8Key.wasPressedThisFrame) {
			Globals.irToggle = false;
		}
		if (keyboard.digit9Key.wasPressedThisFrame) {
			Globals.irToggle = true;
		}
		if (keyboard.digit0Key.wasPressedThisFrame) {
			if (WiimoteManager.HasWiimote()) {
                Wiimote wiimote = WiimoteManager.Wiimotes[0];
				wiimote.SetupIRCamera(Globals.SensorBarMode);
			}
		}

	}
}
