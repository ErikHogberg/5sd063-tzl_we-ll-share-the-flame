using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class WheelchairMoveScript : MonoBehaviour {

	public GameObject LeftWheel;
	public GameObject RightWheel;

	public float Speed = 1.0f;
	public float Acceleration = 1.0f;
	public float Damping = 0.3f;
	public float ForwardCorrectionSpeed = 0.2f;
	public float TopSpeed = 1.0f;

	private float leftWheelSpeed = 0.0f;
	private float rightWheelSpeed = 0.0f;

	void Start() {

	}

	void Update() {
		var keyboard = Keyboard.current;

		float leftWheelDir = 0.0f;
		if (keyboard.wKey.isPressed)
			leftWheelDir = Acceleration;
		else if (keyboard.sKey.isPressed)
			leftWheelDir = -Acceleration;

		float rightWheelDir = 0.0f;
		if (keyboard.eKey.isPressed)
			rightWheelDir = Acceleration;
		else if (keyboard.dKey.isPressed)
			rightWheelDir = -Acceleration;


		// damping
		leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, 0.0f, Damping * Time.deltaTime);
		rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, 0.0f, Damping * Time.deltaTime);

		// add to speed if pressed
		//leftWheelSpeed += leftWheelDir * Speed * Time.deltaTime;
		//rightWheelSpeed += rightWheelDir * Speed * Time.deltaTime;
		leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, TopSpeed, leftWheelDir * Speed * Time.deltaTime);
		rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, TopSpeed, rightWheelDir * Speed * Time.deltaTime);

		// stabilize forward movement
		// NOTE: correction will be applied depending on speed in the future, instead of using buttons
		// TODO: make sure stabilization doesn't interfere with turning
		if (leftWheelDir > 0.0f && rightWheelDir > 0.0f) {
			if (leftWheelDir > rightWheelDir) {
				rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, leftWheelSpeed, ForwardCorrectionSpeed * Time.deltaTime);
			} else if (rightWheelDir > leftWheelDir) {
				leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, rightWheelSpeed, ForwardCorrectionSpeed * Time.deltaTime);
			}
		}

		// turn
		transform.Rotate(transform.up, leftWheelSpeed - rightWheelSpeed);
		// move forward
		transform.position += transform.forward * Time.deltaTime * (leftWheelSpeed + rightWheelSpeed);

	}
}
