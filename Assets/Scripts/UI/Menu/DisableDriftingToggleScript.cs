using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class DisableDriftingToggleScript : MonoBehaviour {

	private void Start() {
		Globals.DisableDrifting = GetComponent<Toggle>().isOn;
	}
	public void SubmitEvent() {
		Globals.DisableDrifting = GetComponent<Toggle>().isOn;
	}
}
