using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using WiimoteApi;

public class PressWiimoteToStartScript : MonoBehaviour {

	public string LevelToStart;
	
	void Start() {

	}

	void Update() {
		if (!WiimoteManager.HasWiimote()) {
			WiimoteManager.FindWiimotes();
		} else {
			Wiimote wiimote = WiimoteManager.Wiimotes[0];
			if (wiimote.Button.b) {
				Globals.FadePanel.StartLevelTransition(LevelToStart);
			}
		}
	}
}
