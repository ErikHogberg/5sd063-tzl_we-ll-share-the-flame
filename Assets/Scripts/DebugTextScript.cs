using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.UI;
// using Input = UnityEngine.Experimental.Input;

public class DebugTextScript : MonoBehaviour {

	private Text text;

	// Start is called before the first frame update
	void Start() {

		text = GetComponent<Text>();

	}

	// Update is called once per frame
	void Update() {
		string output = "";

		output += "mice\n";
		foreach (InputDevice mouse in InputDevice.all) {
			
			output += mouse + "\n";
            // Vector2 scroll = Mouse.current.scroll.ReadValue();
            Vector2 scroll = Input.mouseScrollDelta;
            output += "wheel: " + scroll.x + ", " + scroll.y + "\n";


        }

		output += "gamepads\n";
		foreach (Gamepad gamepad in Gamepad.all) {

			output += gamepad + "\n";
		}

		text.text = output;
	}
}
