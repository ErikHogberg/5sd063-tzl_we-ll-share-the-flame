using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class FlipWheelsToggleUIScript : MonoBehaviour
{
    public void SubmitEvent() {
		Globals.FlipWheels = GetComponent<Toggle>().isOn;
	}

	private void OnDestroy() {
		SubmitEvent();
	}
}
