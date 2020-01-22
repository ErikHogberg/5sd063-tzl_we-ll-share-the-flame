using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class UseWiimoteToggleScript : MonoBehaviour {
	

	public void SubmitEvent() {
		Globals.UseWiimote = GetComponent<Toggle>().isOn;
	}

	private void OnDestroy() {
		SubmitEvent();
	}
}
