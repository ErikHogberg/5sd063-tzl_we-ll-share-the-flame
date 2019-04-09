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
			// TODO: limit angle
			transform.Rotate(
				Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime,
				Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime,
				0,
				Space.World
			);

		}

	}

}
