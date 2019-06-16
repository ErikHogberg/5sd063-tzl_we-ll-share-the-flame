using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
// using UnityEngine.Experimental.Input;
using WiimoteApi;

public class CalibrateWiimoteIRScript : MonoBehaviour {
	void Update() {
		Keyboard keyboard = Keyboard.current;
		if (Input.GetKeyDown("8")) {
			if (WiimoteManager.HasWiimote()) {
				Wiimote wiimote = WiimoteManager.Wiimotes[0];
				wiimote.MotionPlus.SetZeroValues();
			}
			Debug.Log("Calibrated Motion Plus");
		}
		if (Input.GetKeyDown("8")) {
			Globals.irToggle = false;
			Debug.Log("Disabled IR");
		}
		if (Input.GetKeyDown("9")) {
			Globals.irToggle = true;
			Debug.Log("Enabled IR");
		}
		if (Input.GetKeyDown("0")) {
			Debug.Log("Calibrated IR");
			if (WiimoteManager.HasWiimote()) {
                Wiimote wiimote = WiimoteManager.Wiimotes[0];
				wiimote.SetupIRCamera(Globals.SensorBarMode);
			}
		}

	}
}
