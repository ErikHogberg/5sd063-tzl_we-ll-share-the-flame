using Assets.Scripts;
using Assets.Scripts.Utilities;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.UI;

public class WheelchairMoveScript : MonoBehaviour {

	[Header("Required Objects")]
	public GameObject WheelChair;
	public GameObject LeftWheel;
	public GameObject RightWheel;
	public ParticleSystem LeftWheelSparks;
	public ParticleSystem RightWheelSparks;
	private TrailRenderer leftWheelTrail;
	private TrailRenderer rightWheelTrail;
	public GameObject LeftBoostFoam;
	public GameObject RightBoostFoam;
	private ParticleSystem[] BoostFoamParticles;

	[Tooltip("How fast the wheels spin compared to the movement speed")]
	public float WheelAnimationSpeed = 1.0f;

	public GameObject TrajectoryArrow;
	public GameObject DirectionArrow;

	[Header("Movement")]
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

	public bool EnableCollision = false;
	[Tooltip("Scale of how much speed is kept when bouncing off a wall")]
	public float CollisionSlowdownMultiplier = 1.0f;

	[Tooltip("How many seconds before regaining control after bouncing off wall")]
	public float CollisionTime = 0.5f;
	private Timer collisionTimer;

	[Tooltip("Around which axis the wheel models turn")]
	public Vector3 WheelRotationAxis = Vector3.down;

	// Drifting
	private bool drifting = false;
	[Header("Drifting")]
	[Tooltip("How fast the wheelchair has to turn before losing traction")]
	public float DriftAngleThreshold = 1.0f;
	[Tooltip("How fast the wheelchair has to move before potentially losing traction")]
	public float DriftStartSpeedThreshold = 5.0f;
	[Tooltip("How slow the wheelchair has to move before gaining traction")]
	public float DriftEndSpeedThreshold = 5.0f;
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


	[Header("Ramp jumping")]
	// Ramp jumping	
	[Tooltip("How long the wheelchair is in the air when jumping")]
	public float JumpTime = 1.0f;
	[Tooltip("How high the wheelchair jumps (unless specified by the ramps settings)")]
	public float JumpHeight = 1.0f;
	private bool useTempJumpHeight = false;
	private float tempJumpHeight = 1.0f;
	private Timer jumpTimer;
	private float jumpSpeed;
	private float initialPlayerY;
	private float playerY;
	private float jumpTargetY;
	[Tooltip("The curve of the jumping arc (needs to be set to \"ping pong\" to mirror the graph when overflowing)")]
	public AnimationCurve JumpArc;
	private bool skipUp = false;
	private bool setJumpSpeed = false;
	private float nextJumpSpeed = 1f;
	private bool setJumpTime = false;
	private float nextJumpTime = 1f;
	[Tooltip("How much the wheelchair rotates when jumping")]
	public float StuntAngle = 360f;
	[Tooltip("Around which axis the wheelchair rotates when jumping")]
	public Vector3 StuntAxis = new Vector3(1f, 0f, 0f);
	private Vector3 nextStuntAxis = new Vector3(1f, 0f, 0f);
	[Tooltip("Animation curve for the rotation of the wheelchair when jumping")]
	public AnimationCurve StuntCurve;
	private float nextStuntAngle;
	[Tooltip("Whether or not the jump animation curve should repeat (allows setting the curve to \"ping pong\" to loop back)")]
	public bool StuntPingPong = false;
	private bool nextStuntPingPong = false;

	private Quaternion preJumpRotation;

	[Header("Boosting")]
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
	private float boostEndSpeed;

	// Zipline
	private bool ziplining = false;
	private Vector3 ziplineTarget;
	private float ziplineSpeed;

	[Header("Optional objects")]
	[Tooltip("UI text to output debug info to (optional)")]
	public Text InfoPane;

	void Start() {

		Globals.Player = this;

		nextJumpTime = JumpTime;
		nextStuntAngle = StuntAngle;
		nextStuntAxis = StuntAxis;
		nextStuntPingPong = StuntPingPong;

		//DriftTimer = new Timer(DriftDuration);
		//DriftTimer.Stop();

		initialPlayerY = transform.position.y;
		playerY = initialPlayerY;
		jumpTargetY = playerY;

		leftWheelTrail = LeftWheelSparks.GetComponentInChildren<TrailRenderer>();
		rightWheelTrail = RightWheelSparks.GetComponentInChildren<TrailRenderer>();

		BoostFoamParticles = LeftBoostFoam.GetComponentsInChildren<ParticleSystem>();
		BoostFoamParticles = BoostFoamParticles.Concat(RightBoostFoam.GetComponentsInChildren<ParticleSystem>()).ToArray();

		StopBoostParticles();

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
			Cursor.visible = false;
		}



	}

	void Update() {

		var keyboard = Keyboard.current;

		string infoText = "";

		boostSlowdownTimer.Update();

		if (collisionTimer.IsRunning()) {
			collisionTimer.Update();
			float boostSlowdownProgress = 0f;
			if (boostSlowdownTimer.IsRunning()) {
				boostSlowdownProgress = boostSlowdownTimer.TimeLeft() / BoostSlowdownTime;
			}
			transform.position += transform.forward
			 * (leftWheelSpeed + rightWheelSpeed - boostSlowdownProgress * boostEndSpeed)
			 * Time.deltaTime;

			// float turnAngle = rightWheelSpeed - leftWheelSpeed;
			float turnAngle = leftWheelSpeed - rightWheelSpeed;
			turnAngle *= TurningSpeed;
			turnAngle = Mathf.MoveTowards(turnAngle, 0, ForwardCorrectionSpeed);
			transform.Rotate(transform.up, turnAngle * Time.deltaTime * 60);

			SpinWheels();

			if (boostTimer.IsRunning()) {
				if (boostTimer.Update()) {
					boostEndSpeed = Mathf.Max(leftWheelSpeed, rightWheelSpeed);
					boostSlowdownTimer.Restart(BoostSlowdownTime);
					StopBoostParticles();
				}
			}

			return;
		}

		if (ziplining) {
			float ziplineDelta = ziplineSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, ziplineTarget, ziplineSpeed);
			if (Vector3.Distance(transform.position, ziplineTarget) < 0.01f) {
				ziplining = false;
				// jumpTargetY = playerY;
				// playerY = transform.position.y;
				// skipUp = true;
				jumpTimer.Restart();
			}
			return;
		}

		if (jumpTimer.IsRunning()) {
			if (jumpTimer.Update()) {

				// TODO: reset values when jump is cancelled elsewhere

				Vector3 pos = transform.position;
				playerY = jumpTargetY;
				pos.y = playerY;
				transform.position = pos;

				useTempJumpHeight = false;
				skipUp = false;
				setJumpSpeed = false;
				setJumpTime = false;

				transform.localRotation = preJumpRotation;

				nextStuntAngle = StuntAngle;
				nextStuntAxis = StuntAxis;
				nextStuntPingPong = StuntPingPong;

			} else {
				transform.localRotation = preJumpRotation;
				transform.position += transform.forward * jumpSpeed * Time.deltaTime;
				Vector3 pos = transform.position;

				float jumpProgress = 1f - jumpTimer.TimeLeft() / nextJumpTime;
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
				float progressLoop = 1f;
				if (nextStuntPingPong) {
					progressLoop = 2f;
				}
				transform.localRotation = preJumpRotation
				 * Quaternion.AngleAxis(StuntCurve.Evaluate(jumpProgress * progressLoop) * nextStuntAngle, nextStuntAxis);

				LeftWheel.transform.Rotate(-WheelRotationAxis, leftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60f);
				RightWheel.transform.Rotate(WheelRotationAxis, rightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60f);
				return;
			}
		}

		if (Mouse.current.rightButton.isPressed) {// && !boostTimer.IsRunning()) {
												  // boostTimer.Restart(BoostTime);
			Boost();
		}


		if (boostTimer.IsRunning()) {
			if (boostTimer.Update()) {
				boostEndSpeed = Mathf.Max(leftWheelSpeed, rightWheelSpeed);
				boostSlowdownTimer.Restart(BoostSlowdownTime);
				StopBoostParticles();
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

				// float x = Mouse.current.delta.x.ReadValue() * MouseAdjust.x * Speed;
				// float y = Mouse.current.delta.y.ReadValue() * MouseAdjust.y * Speed;
				float x = Input.GetAxis("Mouse X") * MouseAdjust.x * Speed;
				float y = Input.GetAxis("Mouse Y") * MouseAdjust.y * Speed;



				if (FlipKeys) {
					leftWheelSpeed = x;
					rightWheelSpeed = y;

					// if (x == 0f) {
					// 	leftWheelSpeed = boostEndSpeed * boostSlowdownProgress;
					// } else {
					// 	leftWheelSpeed = Mathf.Min(Mathf.Abs(x), TopSpeed) * (x / Mathf.Abs(x)) + boostEndSpeed * boostSlowdownProgress;
					// }
					// if (y == 0f) {
					// 	rightWheelSpeed = boostEndSpeed * boostSlowdownProgress;
					// } else {
					// 	rightWheelSpeed = Mathf.Min(Mathf.Abs(y), TopSpeed) * (y / Mathf.Abs(y)) + boostEndSpeed * boostSlowdownProgress;
					// }
				} else {
					leftWheelSpeed = y;
					rightWheelSpeed = x;

					// if (y == 0f) {
					// 	leftWheelSpeed = boostEndSpeed * boostSlowdownProgress;
					// } else {
					// 	leftWheelSpeed = Mathf.Min(Mathf.Abs(y), TopSpeed) * (y / Mathf.Abs(y)) + boostEndSpeed * boostSlowdownProgress;
					// }
					// if (x == 0f) {
					// 	rightWheelSpeed = boostEndSpeed * boostSlowdownProgress;
					// } else {
					// 	rightWheelSpeed = Mathf.Min(Mathf.Abs(x), TopSpeed) * (x / Mathf.Abs(x)) + boostEndSpeed * boostSlowdownProgress;
					// }
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

		float angle = leftWheelSpeed - rightWheelSpeed;
		angle *= TurningSpeed;
		angle = Mathf.MoveTowards(angle, 0, ForwardCorrectionSpeed);

		//angle %= Mathf.PI * 2.0f;

		float speed = leftWheelSpeed + rightWheelSpeed;
		float fastestWheelSpeed = Mathf.Max(leftWheelSpeed, rightWheelSpeed);

		// turn
		infoText += angle + "\n";
		if (Mathf.Abs(angle) < DriftAngleThreshold
		|| (drifting && Mathf.Abs(driftSpeed) < DriftEndSpeedThreshold)
		// || (drifting && Mathf.Abs(driftSpeed) < DriftSpeedThreshold)
		|| (!drifting && Mathf.Abs(speed) < DriftStartSpeedThreshold)

		) {
			// NOTE: Not drifring

			if (drifting) {
				drifting = false;
				// NOTE: On drift end

				//float wheelchairAngle = 0.0f;
				WheelChair.transform.localRotation.ToAngleAxis(out float wheelchairAngle, out Vector3 axis);
				transform.Rotate(transform.up, wheelchairAngle * axis.y * Time.deltaTime * 60f);

				driftSpeed = 0;

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
			// NOTE: Drifring


			if (!drifting) {
				drifting = true;
				// NOTE: On drift start

				driftSpeed = speed;
				// driftAngle = 0f;

				LeftWheelSparks.Play();
				RightWheelSparks.Play();
				leftWheelTrail.emitting = true;
				rightWheelTrail.emitting = true;

			}

			float driftSign = 1f;//angle / Mathf.Abs(angle);
			if (angle < 0f) {
				driftSign = -1;
			}
			driftAngle = (Mathf.Abs(angle) - DriftAngleThreshold) * DriftTurnScale * driftSign;
			// TODO: reduce drift turn scale as movement speed increases
			// driftAngle += angle * DriftTurnScale * DriftTurnScale;

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
			/*
			if (Mathf.Abs(angle) > Mathf.PI / 2.0f + DriftAngleThreshold) {
				trajectorySpeedChange *= -1.0f;

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
			float boostSlowdownProgress = 0f;
			if (boostSlowdownTimer.IsRunning()) {
				boostSlowdownProgress = boostSlowdownTimer.TimeLeft() / BoostSlowdownTime;
			}
			transform.position += transform.forward * (Mathf.Min(speed, TopSpeed) + boostEndSpeed * boostSlowdownProgress) * Time.deltaTime;
		}

		SpinWheels();

		if (InfoPane != null) {
			InfoPane.text = infoText;
		}

	}

	private void SpinWheels() {
		LeftWheel.transform.Rotate(-WheelRotationAxis, leftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
		RightWheel.transform.Rotate(WheelRotationAxis, rightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
	}

	private void OnTriggerEnter(Collider other) {

		// NOTE: ziplining through buildings is allowed
		if (!EnableCollision || other.tag == "Ignore Collision" || ziplining) {
			return;
		}

		if (other.tag == "Ramp") {
			Debug.Log("hit ramp " + other.name + "!");

			// NOTE: ignores jump if already in air
			if (jumpTimer.IsRunning()) {
				return;
			}

			CancelBoost();

			RampScript rampScript = other.GetComponent<RampScript>();

			if (rampScript != null) {
				StartJump(
					rampScript.TargetHeightRelativity, rampScript.TargetHeight,
					rampScript.JumpHeightRelativity, rampScript.JumpHeight,
					rampScript.SkipUp,
					rampScript.SetSpeed, rampScript.Speed,
					rampScript.SetTime, rampScript.Time,
					rampScript.AlignPlayer, rampScript.transform.localRotation,
					rampScript.StuntAngle, rampScript.StuntAxis, rampScript.StuntPingPong
				);
			} else {
				StartJump();
			}

			return;
		}

		if (other.tag == "Zipline") {
			Debug.Log("hit zipline " + other.name + "!");
			ZiplineScript zipline = other.GetComponent<ZiplineScript>();
			if (zipline != null) {
				CancelBoost();

				transform.position = zipline.transform.position;
				ziplining = true;
				ziplineTarget = zipline.End.transform.position;
				// preJumpRotation = zipline.End.transform.rotation;

				ziplineSpeed = zipline.Speed;
				transform.rotation = zipline.End.transform.rotation;

				switch (zipline.TargetHeightRelativity) {
					case JumpTargetSetting.Absolute:
						jumpTargetY = zipline.TargetHeight;
						break;
					case JumpTargetSetting.Relative:
						jumpTargetY = playerY + zipline.TargetHeight;
						break;
					case JumpTargetSetting.Reset:
						jumpTargetY = initialPlayerY + zipline.TargetHeight;
						break;
				}

				SetupJump(
					zipline.TargetHeightRelativity, zipline.TargetHeight,
					JumpTargetSetting.Relative, 1,
					false,
					false, 1,
					false, 1,
					true, zipline.End.transform.rotation,
					// TODO: bool to use default stunt settings
					StuntAngle, StuntAxis, StuntPingPong//tempStuntAngle, tempStuntAxis, tempStuntPingPong
				);

			} else {
				Debug.LogError("Zipline script not found!");
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
		StartBoostParticles();
	}

	public void Boost(float boostTime, Quaternion facing) {
		transform.rotation = facing;
		Boost(boostTime);
	}

	public void Boost() {
		Boost(BoostTime);
	}

	public void CancelBoost() {
		boostTimer.Stop();
		StopBoostParticles();
	}

	public void StartBoostParticles() {
		foreach (ParticleSystem particles in BoostFoamParticles) {
			particles.Play();
		}
	}

	public void StopBoostParticles() {
		foreach (ParticleSystem particles in BoostFoamParticles) {
			particles.Stop();
		}
	}

	public void PauseBoostParticles() {
		foreach (ParticleSystem particles in BoostFoamParticles) {
			particles.Pause();
		}
	}

	public void SetupJump(
		JumpTargetSetting TargetHeightRelativity, float TargetHeight,
		JumpTargetSetting JumpHeightRelativity, float JumpHeight,
		bool SkipNextUp,
		bool SetSpeed, float Speed,
		bool SetTime, float Time,
		bool AlignPlayer, Quaternion Rotation,
		float tempStuntAngle, Vector3 tempStuntAxis, bool tempStuntPingPong
	){
		// NOTE: ignores jump if already in air
		if (jumpTimer.IsRunning()) {
			return;
		}

		CancelBoost();

		switch (TargetHeightRelativity) {
			case JumpTargetSetting.Absolute:
				jumpTargetY = TargetHeight;
				break;
			case JumpTargetSetting.Relative:
				jumpTargetY = playerY + TargetHeight;
				break;
			case JumpTargetSetting.Reset:
				jumpTargetY = initialPlayerY + TargetHeight;
				break;
		}

		useTempJumpHeight = true;
		switch (JumpHeightRelativity) {
			case JumpTargetSetting.Absolute:
				tempJumpHeight = JumpHeight;
				break;
			case JumpTargetSetting.Relative:
				tempJumpHeight = playerY + JumpHeight;
				break;
			case JumpTargetSetting.Reset:
				tempJumpHeight = initialPlayerY + JumpHeight;
				break;
		}

		skipUp = SkipNextUp;
		setJumpSpeed = SetSpeed;
		nextJumpSpeed = Speed;
		setJumpTime = SetTime;
		nextJumpTime = Time;

		if (AlignPlayer) {
			transform.rotation = Rotation;
		}

		nextStuntAngle = tempStuntAngle;
		nextStuntAxis = tempStuntAxis;
		nextStuntPingPong = tempStuntPingPong;
	}

	public void StartJump(
		JumpTargetSetting TargetHeightRelativity, float TargetHeight,
		JumpTargetSetting JumpHeightRelativity, float JumpHeight,
		bool SkipNextUp,
		bool SetSpeed, float Speed,
		bool SetTime, float Time,
		bool AlignPlayer, Quaternion Rotation,
		float tempStuntAngle, Vector3 tempStuntAxis, bool tempStuntPingPong
	) {

		SetupJump(
			TargetHeightRelativity, TargetHeight,
			JumpHeightRelativity, JumpHeight,
			SkipNextUp,
			SetSpeed, Speed,
			SetTime, Time,
			AlignPlayer, Rotation,
			tempStuntAngle, tempStuntAxis, tempStuntPingPong
		);

		StartJump();
	}

	public void StartJump() {
		if (!setJumpTime) {
			nextJumpTime = JumpTime;
		}
		jumpTimer.Restart(nextJumpTime);
		preJumpRotation = transform.localRotation;
		if (setJumpSpeed) {
			jumpSpeed = nextJumpSpeed;
		} else {
			jumpSpeed = leftWheelSpeed + rightWheelSpeed;
		}
	}

}
