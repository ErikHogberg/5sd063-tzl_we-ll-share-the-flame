using Assets.Scripts;
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
	public ParticleSystem LeftWheelSparks;
	public ParticleSystem RightWheelSparks;
	private TrailRenderer leftWheelTrail;
	private TrailRenderer rightWheelTrail;

	public float WheelAnimationSpeed = 1.0f;

	public GameObject TrajectoryArrow;
	public GameObject DirectionArrow;


	public bool FlipKeys = false;
	public bool UseMouse = false;
	public Vector2 MouseAdjust = new Vector2(-1f, 1f);

	public float Speed = 1.0f;
	public float TurningSpeed = 1.0f;
	public float Acceleration = 1.0f;
	public float Damping = 0.3f;
	public float ForwardCorrectionSpeed = 0.2f;
	public float TopSpeed = 1.0f;

	private bool drifting = false;
	public float DriftAngleThreshold = 1.0f;
	public float DriftSpeedThreshold = 5.0f;
	public float DriftDuration = 1.0f;
	public float DriftDampingAdd = 1.0f;
	public float DriftDampingMul = 0.2f;
	//private Timer DriftTimer;
	public float DriftScale = 0.1f;

	// private float driftAngle = 0.0f;
	private float driftSpeed = 0.0f;

	[HideInInspector]
	public float leftWheelSpeed = 0.0f;
	[HideInInspector]
	public float rightWheelSpeed = 0.0f;
	public Vector3 WheelRotationAxis = Vector3.down;

	public Text InfoPane;

	public bool EnableCollision = false;
	public float CollisionSlowdownMultiplier = 1.0f;

	// Ramp jumping	
	public float JumpTime = 1.0f;
	public float JumpHeight = 1.0f;
	private Timer jumpTimer;
	private float jumpSpeed;
	private float playerY;
	public AnimationCurve JumpArc;

	public float BoostTime = 2.5f;
	private Timer boostTimer;
	public float BoostAcceleration = 1f;
	public float BoostMaxSpeed = 20f;


	void Start() {

		Globals.Player = this;

		//DriftTimer = new Timer(DriftDuration);
		//DriftTimer.Stop();

		playerY = transform.position.y;

		leftWheelTrail = LeftWheelSparks.GetComponentInChildren<TrailRenderer>();
		rightWheelTrail = RightWheelSparks.GetComponentInChildren<TrailRenderer>();

		jumpTimer = new Timer(JumpTime);
		jumpTimer.Stop();

		boostTimer = new Timer(BoostTime);
		boostTimer.Stop();

		if (UseMouse) {
			Cursor.lockState = CursorLockMode.Locked;
			// Cursor.visible = false;
		}

	}

	void Update() {
		var keyboard = Keyboard.current;

		string infoText = "";

		if (jumpTimer.IsRunning()) {
			if (jumpTimer.Update()) {
				Vector3 pos = transform.position;
				pos.y = playerY;
				transform.position = pos;
			} else {
				transform.position += transform.forward * jumpSpeed * Time.deltaTime;
				Vector3 pos = transform.position;
				pos.y = playerY + JumpHeight * JumpArc.Evaluate(jumpTimer.TimeLeft() / JumpTime);
				transform.position = pos;
				LeftWheel.transform.Rotate(-WheelRotationAxis, leftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
				RightWheel.transform.Rotate(WheelRotationAxis, rightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
				return;
			}
		}

		if (keyboard.bKey.wasPressedThisFrame && !boostTimer.IsRunning()) {
			boostTimer.Restart(BoostTime);
		}

		if (boostTimer.IsRunning()) {
			boostTimer.Update();

			leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, BoostMaxSpeed, BoostAcceleration * Time.deltaTime);
			rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, BoostMaxSpeed, BoostAcceleration * Time.deltaTime);
			transform.position += transform.forward * (leftWheelSpeed + rightWheelSpeed) * Time.deltaTime;
			LeftWheel.transform.Rotate(-WheelRotationAxis, leftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
			RightWheel.transform.Rotate(WheelRotationAxis, rightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
			return;
		}

		if (UseMouse) {
			if (!keyboard.leftShiftKey.isPressed) {

				// float x = Mouse.current.delta.x.ReadValue() * MouseAdjust.x * Speed * Time.deltaTime;
				// float y = Mouse.current.delta.y.ReadValue() * MouseAdjust.y * Speed * Time.deltaTime;
				float x = Input.GetAxis("Mouse X") * MouseAdjust.x * Speed * Time.deltaTime;
				float y = Input.GetAxis("Mouse Y") * MouseAdjust.y * Speed * Time.deltaTime;

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

			// TODO: migrate to event based system
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
			leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, TopSpeed * (leftWheelDir / Acceleration), Mathf.Abs(leftWheelDir) * Speed * Time.deltaTime);
			rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, TopSpeed * (rightWheelDir / Acceleration), Mathf.Abs(rightWheelDir) * Speed * Time.deltaTime);

			if (keyboard.spaceKey.isPressed) {
				leftWheelSpeed = 0.0f;
				rightWheelSpeed = 0.0f;

				// TODO: drift when stopping too fast
			}

			//*
			// stabilize forward movement
			if (leftWheelDir > 0.0f && rightWheelDir > 0.0f) {
				if (leftWheelDir > rightWheelDir) {
					rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, leftWheelSpeed, ForwardCorrectionSpeed * Time.deltaTime);
				} else if (rightWheelDir > leftWheelDir) {
					leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, rightWheelSpeed, ForwardCorrectionSpeed * Time.deltaTime);
				}
			}
			// */
		}

		float angle = leftWheelSpeed - rightWheelSpeed;
		angle *= TurningSpeed;
		//angle %= Mathf.PI * 2.0f;
		float speed = leftWheelSpeed + rightWheelSpeed;

		// turn
		infoText += angle + "\n";
		if (Mathf.Abs(angle) < DriftAngleThreshold || Mathf.Abs(speed) < DriftSpeedThreshold) {

			if (drifting) {
				drifting = false;

				//float wheelchairAngle = 0.0f;
				WheelChair.transform.localRotation.ToAngleAxis(out float wheelchairAngle, out Vector3 axis);
				transform.Rotate(transform.up, wheelchairAngle * axis.y * Time.deltaTime * 60);

				LeftWheelSparks.Stop();
				RightWheelSparks.Stop();
				leftWheelTrail.emitting = false;
				rightWheelTrail.emitting = false;
			}

			transform.Rotate(transform.up, angle * Time.deltaTime * 60);
			WheelChair.transform.localRotation = Quaternion.identity;
			TrajectoryArrow.SetActive(false);
			DirectionArrow.SetActive(false);

		} else {

			if (!drifting) {
				drifting = true;

				driftSpeed = speed;

				LeftWheelSparks.Play();
				RightWheelSparks.Play();
				leftWheelTrail.emitting = true;
				rightWheelTrail.emitting = true;

			}

			float driftAngle =
			(Mathf.Abs(angle) - DriftAngleThreshold) * DriftScale
			 * (angle / Mathf.Abs(angle))
			 ;


			//WheelChair.transform.Rotate(transform.up, angle);
			WheelChair.transform.localRotation = Quaternion.AngleAxis(
				// TODO: turn relativly while drifting
				// (Mathf.Abs(angle) - DriftAngleThreshold) * (angle / Mathf.Abs(angle)) * Mathf.Rad2Deg,
				driftAngle * Mathf.Rad2Deg,
				Vector3.up
			);

			// TODO: move trajectory angle towards player angle
			float trajectoryAngleChange = angle;
			//angle %= Mathf.PI;
			// if (Mathf.Abs( angle)*DriftScale > Mathf.PI + DriftAngleThreshold) {
			if (driftAngle > Mathf.PI) {
				trajectoryAngleChange *= -1.0f;

				// TODO: equalize wheel speed
				// float totalSpeed = leftWheelSpeed + rightWheelSpeed;


			}

			float trajectorySpeedChange = speed;
			/*
			if (Mathf.Abs(angle) > Mathf.PI/2.0f + DriftAngleThreshold) {
				//trajectorySpeedChange *= -1.0f;
				
			}
			// */

			transform.Rotate(
				transform.up,
				(
					trajectoryAngleChange
					* trajectorySpeedChange
					* DriftDampingMul
					+ DriftDampingAdd
				)
				* Time.deltaTime
			);
			// TODO: don't gain speed while drifting
			// IDEA: use wheel speed to increase trajectory influence during drift

			// TODO: don't stop drifting until matching direction
			TrajectoryArrow.SetActive(true);
			DirectionArrow.SetActive(true);
		}

		// TODO: drifting
		// IDEA: compare turn delta to velocity, trigger drift event if turning too fast.
		// IDEA: when drift mode triggers, a timer starts, during which you can turn independently from movement direction.
		// IDEA: instead of timer, stop drifting when trajectory is within x degrees of wheelchair angle, x degrees depends on movement/drifting speed
		// IDEA: moving while drifting will alter trajectory in turning direction.

		// move forward
		infoText += speed + "\n";
		if (drifting) {
			driftSpeed = Mathf.MoveTowards(driftSpeed, 0, Damping * Time.deltaTime);
			transform.position += transform.forward * driftSpeed * Time.deltaTime;
		} else {
			transform.position += transform.forward * speed * Time.deltaTime;
		}


		// moving wheels
		LeftWheel.transform.Rotate(-WheelRotationAxis, leftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
		RightWheel.transform.Rotate(WheelRotationAxis, rightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);

		if (InfoPane != null) {
			InfoPane.text = infoText;
		}

	}

	private void OnTriggerEnter(Collider other) {

		if (!EnableCollision || other.tag == "Ignore Collision") {
			return;
		}

		if (other.tag == "Ramp") {
			// Debug.Log("hit ramp!");
			jumpTimer.Restart(JumpTime);
			jumpSpeed = leftWheelSpeed + rightWheelSpeed;
			return;
		}

		Debug.Log("hit wall: " + other.name);
		// Turn 180 degrees when hitting a wall
		// TODO: turn 90 (or 135?) degrees left or right depending on which direction wall was hit
		transform.Rotate(Vector3.up, 180);
		leftWheelSpeed *= CollisionSlowdownMultiplier;
		rightWheelSpeed *= CollisionSlowdownMultiplier;
	}

}
