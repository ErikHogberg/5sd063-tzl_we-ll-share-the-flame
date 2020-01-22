using UnityEngine;
using UnityEngine.UI;
using Assets.Scripts;

public class DisableDriftingToggleScript : MonoBehaviour {

	// private void Start() {
	// 	SubmitEvent();
	// }
	public void SubmitEvent() {
		Globals.DisableDrifting = GetComponent<Toggle>().isOn;
	}

	private void OnDestroy() {
		SubmitEvent();
	}
}
