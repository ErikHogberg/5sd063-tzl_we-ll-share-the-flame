using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class InputDropdownScript : MonoBehaviour {

	private void Start() {
		SetControlType(GetComponent<Dropdown>().value);
	}

	public void SubmitEvent() {
		SetControlType(GetComponent<Dropdown>().value);
	}

	private void SetControlType(int dropdownValue) {
		switch (dropdownValue) {
			case 0: // mouse
				Globals.ControlType = ControlType.Mouse;
				break;
			case 1: // keyboard
				Globals.ControlType = ControlType.Keyboard;
				break;
			case 2: // controller
				Globals.ControlType = ControlType.Controller;
				break;
			default:
				Globals.ControlType = ControlType.Mouse;
				break;
		}
	}
}
