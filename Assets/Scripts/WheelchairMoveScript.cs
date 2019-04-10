using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.UI;

public class WheelchairMoveScript : MonoBehaviour {

	public GameObject WheelChair;
	public GameObject LeftWheel;
	public GameObject RightWheel;

	public GameObject TrajectoryArrow;
	public GameObject DirectionArrow;


	public bool FlipKeys = false;
	public bool UseMouse = false;

	public float Speed = 1.0f;
	public float Acceleration = 1.0f;
	public float Damping = 0.3f;
	public float ForwardCorrectionSpeed = 0.2f;
	public float TopSpeed = 1.0f;

	private bool drifting = false;
	public float DriftThreshold = 1.0f;
	public float DriftDuration = 1.0f;
	//private Timer DriftTimer;
	private float driftAngle = 0.0f;
	private float driftSpeed = 0.0f;

	private float leftWheelSpeed = 0.0f;
	private float rightWheelSpeed = 0.0f;

	public Text InfoPane;


	void Start() {
		//DriftTimer = new Timer(DriftDuration);
		//DriftTimer.Stop();
	}

	void Update() {
		var keyboard = Keyboard.current;

		string infoText = "";

		if (UseMouse) {
			if (keyboard.leftShiftKey.isPressed) {

				float x = Mouse.current.delta.x.ReadValue() * Speed * Time.deltaTime;
				float y = Mouse.current.delta.y.ReadValue() * Speed * Time.deltaTime;

				if (FlipKeys) {
					leftWheelSpeed = y;
					rightWheelSpeed = x;
				} else {
					leftWheelSpeed = x;
					rightWheelSpeed = y;
				}
			}

			// TODO: stabilize forward movement
			// TODO: make sure stabilization doesn't interfere with turning

		} else {

			float leftWheelDir = 0.0f;
			float rightWheelDir = 0.0f;

			if (keyboard.wKey.isPressed) {
				if (FlipKeys) {
					rightWheelDir = Acceleration;
				} else {
					leftWheelDir = Acceleration;
				}
			} else if (keyboard.sKey.isPressed) {
				if (FlipKeys) {
					rightWheelDir = -Acceleration;
				} else {
					leftWheelDir = -Acceleration;
				}
			}

			if (keyboard.eKey.isPressed) {
				if (FlipKeys) {
					leftWheelDir = Acceleration;
				} else {
					rightWheelDir = Acceleration;
				}
			} else if (keyboard.dKey.isPressed) {
				if (FlipKeys) {
					leftWheelDir = -Acceleration;
				} else {
					rightWheelDir = -Acceleration;
				}
			}

			// damping
			leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, 0.0f, Damping * Time.deltaTime);
			rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, 0.0f, Damping * Time.deltaTime);

			// add to speed if pressed
			//leftWheelSpeed += leftWheelDir * Speed * Time.deltaTime;
			//rightWheelSpeed += rightWheelDir * Speed * Time.deltaTime;
			leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, TopSpeed, leftWheelDir * Speed * Time.deltaTime);
			rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, TopSpeed, rightWheelDir * Speed * Time.deltaTime);

			// stabilize forward movement
			if (leftWheelDir > 0.0f && rightWheelDir > 0.0f) {
				if (leftWheelDir > rightWheelDir) {
					rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, leftWheelSpeed, ForwardCorrectionSpeed * Time.deltaTime);
				} else if (rightWheelDir > leftWheelDir) {
					leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, rightWheelSpeed, ForwardCorrectionSpeed * Time.deltaTime);
				}
			}
		}

		float angle = leftWheelSpeed - rightWheelSpeed;
		//angle %= Mathf.PI * 2.0f;
		float speed = Time.deltaTime * (leftWheelSpeed + rightWheelSpeed);

		// turn
		infoText += angle + "\n";
		if (Mathf.Abs(angle) < DriftThreshold || Mathf.Abs(speed) < 0.1f) {

			transform.Rotate(transform.up, angle);
			WheelChair.transform.localRotation = Quaternion.identity;
			TrajectoryArrow.SetActive(false);
			DirectionArrow.SetActive(false);

		} else {

			//WheelChair.transform.Rotate(transform.up, angle);
			WheelChair.transform.localRotation = Quaternion.AngleAxis(
				(Mathf.Abs(angle) - DriftThreshold) * (angle / Mathf.Abs(angle)) * Mathf.Rad2Deg,
				Vector3.up
			);

			// TODO: don't stop drifting until matching direction
			TrajectoryArrow.SetActive(true);
			DirectionArrow.SetActive(true);
		}

		// TODO: drifting
		// IDEA: compare turn delta to velocity, trigger drift event if turning too fast.
		// IDEA: when drift mode triggers, a timer starts, during which you can turn independently from movement direction.
		// IDEA: instead of timer, stop drifting when trajectory is within x degrees of wheelchair angle, x degrees depends on movement/drifting speed
		// IDEA: moving while drifting will alter trajectory in turning direction.
		// IDEA: render arrows under player for drift trajectory and player direction.

		// move forward
		infoText += speed + "\n";
		transform.position += transform.forward * speed;

		InfoPane.text = infoText;

	}
}
