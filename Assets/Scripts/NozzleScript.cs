using UnityEngine;
using UnityEngine.Experimental.Input;

public class NozzleScript : MonoBehaviour {
	public float turnSpeedX = 4.0f;
	public float turnSpeedY = 2.0f;

	private Vector3 offset;
	void Start() {

	}

	void Update() {
		if (Mouse.current.leftButton.isPressed) {

			transform.Rotate(
				Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime,
				Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime,
				0
			);

		}

	}

}
