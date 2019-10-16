using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class InputDropdownScript : MonoBehaviour {
	public void SubmitEvent() {

		Dropdown dropdown = GetComponent<Dropdown>();

		// FIXME: doesnt work in highscore screen

		switch (dropdown.value) {
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

		// Globals.ControlType 
	}
}
