using UnityEngine;
using UnityEngine.Experimental.Input;

public class NozzleScript : MonoBehaviour {

	public ParticleSystem Foam;

	public float turnSpeedX = 4.0f;
	public float turnSpeedY = 2.0f;

	private bool firing = false;

	private Vector3 offset;
	void Start() {
		Foam.Pause();

		// Create action that binds to the primary action control on all devices.
		var action = new InputAction(binding: "*/{primaryAction}");
		// Have it run your code when action is triggered.
		action.performed += _ => { firing = true; Foam.Play(); };
		action.cancelled += _ => { firing = false; Foam.Stop(); };
		// Start listening for control changes.
		action.Enable();
	}

	void Update() {
		//if (Mouse.current.leftButton.isPressed) {
		if (firing) { 
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
