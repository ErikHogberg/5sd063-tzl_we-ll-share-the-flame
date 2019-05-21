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

	[Tooltip("How fast the wheels spin compared to the movement speed")]
	public float WheelAnimationSpeed = 1.0f;

	public GameObject TrajectoryArrow;
	public GameObject DirectionArrow;

	[Tooltip("Flips keys and trackballs")]
	public bool FlipKeys = false;
	[Tooltip("Enable trackballs, disables keys")]
	public bool UseMouse = false;
	[Tooltip("Adjustments for trackball direction and relative (to eachother) speed")]
	public Vector2 MouseAdjust = new Vector2(-1f, 1f);

	[Tooltip("Movement speed multiplier, for both wheels, when using either trackballs or keys")]
	public float Speed = 1.0f;
	[Tooltip("How much the difference between the speed of the wheels causes the wheelchair to turn")]
	public float TurningSpeed = 1.0f;
	[Tooltip("Acceleration speed for keys (not used with trackballs)")]
	public float Acceleration = 1.0f;
	[Tooltip("How quickly the wheelchair stops (keys only)")]
	public float Damping = 0.3f;
	[Tooltip("How different the wheel speeds can be (in abstract \"speed\" units) and still go straight forward")]
	public float ForwardCorrectionSpeed = 0.2f;
	[Tooltip("Top speed of each wheel")]
	public float TopSpeed = 1.0f;

	private bool drifting = false;
	[Tooltip("How fast the wheelchair has to turn before losing traction")]
	public float DriftAngleThreshold = 1.0f;
	[Tooltip("How slow the wheelchair has to move before gaining traction")]
	public float DriftSpeedThreshold = 5.0f;
	// public float DriftDuration = 1.0f;
	[Tooltip("How quickly the drift direction follows the wheelchair facing")]
	public float DriftDampingAdd = 1.0f;
	[Tooltip("How much the wheel speed influences how quickly the drift direction follows the wheelchair facing")]
	public float DriftDampingMul = 0.2f;
	//private Timer DriftTimer;
	[Tooltip("How fast the wheelchair turns while drifting")]
	public float DriftTurnScale = 0.1f;
	[Tooltip("How much the wheel speed influences drifting movement speed, a scale of how much it adds to it")]
	public float DriftSpeedAddScale = 0.1f;
	private float driftAngle = 0f;

	// private float driftAngle = 0.0f;
	private float driftSpeed = 0.0f;
	[Tooltip("How quickly the player loses speed while drifting")]
	public float DriftDamping = 1.0f;

	[HideInInspector]
	public float leftWheelSpeed = 0.0f;
	[HideInInspector]
	public float rightWheelSpeed = 0.0f;

	[Tooltip("Around which axis the wheel models turn")]
	public Vector3 WheelRotationAxis = Vector3.down;

	[Tooltip("UI text to output debug info to (optional)")]
	public Text InfoPane;

	public bool EnableCollision = false;
	[Tooltip("Scale of how much speed is kept when bouncing off a wall")]
	public float CollisionSlowdownMultiplier = 1.0f;

	[Tooltip("How many seconds before regaining control after bouncing off wall")]
	public float CollisionTime = 0.5f;
	private Timer collisionTimer;

	// Ramp jumping	
	[Tooltip("How long the wheelchair is in the air when jumping")]
	public float JumpTime = 1.0f;
	[Tooltip("How high the wheelchair jumps (unless specified by the ramps settings)")]
	public float JumpHeight = 1.0f;
	private bool useTempJumpHeight = false;
	private float tempJumpHeight = 1.0f;
	private Timer jumpTimer;
	private float jumpSpeed;
	private float playerY;
	private float jumpTargetY;
	[Tooltip("The curve of the jumping arc (needs to be set to \"ping pong\" to mirror the graph when overflowing)")]
	public AnimationCurve JumpArc;
	private bool skipUp = false;

	// Boost
	[Tooltip("How Long the player boosts once triggered")]
	public float BoostTime = 2.5f;
	private Timer boostTimer;
	[Tooltip("How fast the wheelchair stops after boosting (the remaining boost speed is added to the wheel speed, shrinking to 0 as it expires), trackballs only")]
	public float BoostSlowdownTime = 1f;
	private Timer boostSlowdownTimer;
	[Tooltip("How fast the wheelchair accelerates when boosting")]
	public float BoostAcceleration = 1f;
	[Tooltip("Max speed when boosting")]
	public float BoostMaxSpeed = 20f;
	private float boostEndSpeedLeft;
	private float boostEndSpeedRight;

	// Zipline
	private bool ziplining = false;
	private Vector3 ziplineTarget;
	private float ziplineSpeed;

	void Start() {

		Globals.Player = this;

		//DriftTimer = new Timer(DriftDuration);
		//DriftTimer.Stop();

		playerY = transform.position.y;
		jumpTargetY = playerY;

		leftWheelTrail = LeftWheelSparks.GetComponentInChildren<TrailRenderer>();
		rightWheelTrail = RightWheelSparks.GetComponentInChildren<TrailRenderer>();

		jumpTimer = new Timer(JumpTime);
		jumpTimer.Stop();

		boostTimer = new Timer(BoostTime);
		boostTimer.Stop();

		boostSlowdownTimer = new Timer(BoostSlowdownTime);
		boostSlowdownTimer.Stop();

		collisionTimer = new Timer(CollisionTime);
		collisionTimer.Stop();

		if (UseMouse) {
			Cursor.lockState = CursorLockMode.Locked;
			// Cursor.visible = false;
		}

	}

	void Update() {
		var keyboard = Keyboard.current;

		string infoText = "";

		if (collisionTimer.IsRunning()) {
			collisionTimer.Update();
			transform.position += transform.forward * (leftWheelSpeed + rightWheelSpeed) * Time.deltaTime;
			return;
		}

		if (ziplining) {
			float ziplineDelta = ziplineSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, ziplineTarget, ziplineSpeed);
			if (Vector3.Distance(transform.position, ziplineTarget) < 0.01f) {
				ziplining = false;
				jumpTargetY = playerY;
				playerY = transform.position.y;
				jumpTimer.Restart();
			}
			return;
		}

		if (jumpTimer.IsRunning()) {
			if (jumpTimer.Update()) {
				Vector3 pos = transform.position;
				playerY = jumpTargetY;
				pos.y = playerY;
				transform.position = pos;
				useTempJumpHeight = false;
				skipUp = false;
			} else {
				transform.position += transform.forward * jumpSpeed * Time.deltaTime;
				Vector3 pos = transform.position;

				float jumpProgress = 1f - jumpTimer.TimeLeft() / JumpTime;
				if (skipUp) {
					jumpProgress = 0.5f + jumpProgress * 0.5f;
				}

				float arcHeight;
				if (useTempJumpHeight) {
					arcHeight = Mathf.Max(tempJumpHeight, jumpTargetY - playerY);
				} else {
					arcHeight = Mathf.Max(JumpHeight, jumpTargetY - playerY);
				}

				if (jumpProgress < 0.5f) {
					pos.y = playerY + arcHeight * JumpArc.Evaluate(jumpProgress * 2f);
				} else {
					pos.y = jumpTargetY + (arcHeight - (jumpTargetY - playerY)) * JumpArc.Evaluate(jumpProgress * 2f);
				}

				transform.position = pos;
				LeftWheel.transform.Rotate(-WheelRotationAxis, leftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60f);
				RightWheel.transform.Rotate(WheelRotationAxis, rightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60f);
				return;
			}
		}

		if (keyboard.bKey.wasPressedThisFrame && !boostTimer.IsRunning()) {
			// boostTimer.Restart(BoostTime);
			Boost();
		}

		boostSlowdownTimer.Update();

		if (boostTimer.IsRunning()) {
			if (boostTimer.Update()) {
				boostEndSpeedLeft = leftWheelSpeed;
				boostEndSpeedRight = rightWheelSpeed;
				boostSlowdownTimer.Restart(BoostSlowdownTime);
			}

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
				float x = Input.GetAxis("Mouse X") * MouseAdjust.x * Speed;
				float y = Input.GetAxis("Mouse Y") * MouseAdjust.y * Speed;

				float boostSlowdownProgress = 0f;
				if (boostSlowdownTimer.IsRunning()) {
					boostSlowdownProgress = boostSlowdownTimer.TimeLeft() / BoostSlowdownTime;
				}

				if (FlipKeys) {
					leftWheelSpeed = Mathf.Min(y, TopSpeed) + boostEndSpeedLeft * boostSlowdownProgress;
					rightWheelSpeed = Mathf.Min(x, TopSpeed) + boostEndSpeedLeft * boostSlowdownProgress;
				} else {
					leftWheelSpeed = Mathf.Min(y, TopSpeed) + boostEndSpeedLeft * boostSlowdownProgress;
					rightWheelSpeed = Mathf.Min(x, TopSpeed) + boostEndSpeedLeft * boostSlowdownProgress;
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

			/*
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

		float angle = Mathf.MoveTowards(leftWheelSpeed - rightWheelSpeed, 0, ForwardCorrectionSpeed);
		angle *= TurningSpeed;
		//angle %= Mathf.PI * 2.0f;
		float speed = leftWheelSpeed + rightWheelSpeed;

		// turn
		infoText += angle + "\n";
		if (Mathf.Abs(angle) < DriftAngleThreshold
		|| (drifting && Mathf.Abs(driftSpeed) < DriftSpeedThreshold)
		) {

			if (drifting) {
				drifting = false;
				// NOTE: On drift end

				//float wheelchairAngle = 0.0f;
				WheelChair.transform.localRotation.ToAngleAxis(out float wheelchairAngle, out Vector3 axis);
				transform.Rotate(transform.up, wheelchairAngle * axis.y * Time.deltaTime * 60f);

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
				// NOTE: On drift start

				driftSpeed = speed;
				driftAngle = 0f;

				LeftWheelSparks.Play();
				RightWheelSparks.Play();
				leftWheelTrail.emitting = true;
				rightWheelTrail.emitting = true;

			}

			// driftAngle = (Mathf.Abs(angle) - DriftAngleThreshold) * DriftScale * (angle / Mathf.Abs(angle));
			// TODO: reduce drift turn scale as movement speed increases
			driftAngle += angle * DriftTurnScale;

			//WheelChair.transform.Rotate(transform.up, angle);
			WheelChair.transform.localRotation = Quaternion.AngleAxis(
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
			//*
			if (Mathf.Abs(angle) > Mathf.PI / 2.0f + DriftAngleThreshold) {
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
			driftSpeed = Mathf.MoveTowards(driftSpeed + speed * DriftSpeedAddScale, 0, DriftDamping * Time.deltaTime);
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
			Debug.Log("hit ramp " + other.name + "!");

			// NOTE: ignores jump if already in air
			if (jumpTimer.IsRunning()) {
				return;
			}

			boostTimer.Stop();

			RampScript rampScript = other.GetComponent<RampScript>();

			if (rampScript != null) {
				if (rampScript.RelativeHeight) {
					jumpTargetY = playerY + rampScript.TargetHeight;
				} else {
					jumpTargetY = rampScript.TargetHeight;
				}
				useTempJumpHeight = true;
				tempJumpHeight = rampScript.JumpHeight;
				skipUp = rampScript.SkipUp;
			}

			jumpTimer.Restart(JumpTime);
			jumpSpeed = leftWheelSpeed + rightWheelSpeed;
			return;
		}

		if (other.tag == "Zipline") {
			Debug.Log("hit zipline " + other.name + "!");
			ZiplineScript zipline = other.GetComponent<ZiplineScript>();
			if (zipline != null) {
				Debug.Log("Found zipline script");
				ziplining = true;
				ziplineTarget = zipline.End.transform.position;
				ziplineSpeed = zipline.Speed;
			}
			return;
		}

		Debug.Log("hit wall: " + other.name);
		// Turn 180 degrees when hitting a wall
		// IDEA: turn 90 (or 135?) degrees left or right depending on which direction wall was hit
		// IDEA: Stop and teleport backwards instead

		if (jumpTimer.IsRunning()) {
			transform.Rotate(Vector3.up, 180f);
		} else {
			collisionTimer.Restart(CollisionTime);
			float tempLeftSpeed = leftWheelSpeed;
			leftWheelSpeed = -rightWheelSpeed;
			rightWheelSpeed = -tempLeftSpeed;
		}

		leftWheelSpeed *= CollisionSlowdownMultiplier;
		rightWheelSpeed *= CollisionSlowdownMultiplier;
	}

	public void Boost(float boostTime) {
		boostTimer.Restart(boostTime);
	}

	public void Boost(float boostTime, Quaternion facing) {
		transform.rotation = facing;
		Boost(boostTime);
	}

	public void Boost() {
		Boost(BoostTime);
	}

}
