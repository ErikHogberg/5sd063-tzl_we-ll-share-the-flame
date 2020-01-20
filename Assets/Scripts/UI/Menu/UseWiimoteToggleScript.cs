using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UseWiimoteToggleScript : MonoBehaviour {
	private void Start() {
		Globals.UseWiimote = GetComponent<Toggle>().isOn;
	}

	public void SubmitEvent() {
		Globals.UseWiimote = GetComponent<Toggle>().isOn;
	}
}
