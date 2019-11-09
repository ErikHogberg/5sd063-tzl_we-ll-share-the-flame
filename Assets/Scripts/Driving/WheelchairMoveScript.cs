using Assets.Scripts;
using Assets.Scripts.Utilities;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
// using UnityEngine.Experimental.Input;
using UnityEngine.UI;

[Serializable]
public struct MovementSettings {
	[Tooltip("Movement speed multiplier, for both wheels, when using either trackballs or keys")]
	public float Speed;
	[Tooltip("How much the difference between the speed of the wheels causes the wheelchair to turn")]
	public float TurningSpeed;
	[Tooltip("Acceleration speed for keys (not used with trackballs)")]
	public float Acceleration;
	[Tooltip("How quickly the wheelchair stops (keys only)")]
	public float Damping;
	[Tooltip("How different the wheel speeds can be (in abstract \"speed\" units) and still go straight forward")]
	public float ForwardCorrectionSpeed;
	[Tooltip("Top speed of each wheel")]
	public float TopSpeed;
}

public class WheelchairMoveScript : MonoBehaviour {

	#region Unity fields

	//Mick start
	[Header("Sound FX")]
	public AudioSource AS_Boing;
	public AudioClip SFX_Boing;
	//Mick end

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
	public AnchorScript NozzleAnchor;

	public GameObject StandingKid;
	public GameObject StandingKidZipline;

	public GameObject TrajectoryArrow;
	public GameObject DirectionArrow;

	[Header("Network")]
	public NetworkMode Network = NetworkMode.None;
	public float networkSendTime = 1f / 30f;
	private Timer networkSendTimer;

	private Task UdpTask;
	private UdpClient udpClient;
	private ConcurrentQueue<Vector2> messageQueue = new ConcurrentQueue<Vector2>();


	[Tooltip("How fast the wheels spin compared to the movement speed")]
	public float WheelAnimationSpeed = 1.0f;

	[Header("Movement")]
	[Tooltip("Disables all movement, stops the script from updating")]
	public bool DisableMovement = false;
	[Tooltip("Overrides controls read from global, set in menus, etc.")]
	public bool OverrideGlobalControls = false;
	[Tooltip("Flips keys and trackballs")]
	public bool FlipWheels = false;
	[Tooltip("Choose control type for movement")]
	public ControlType CurrentControlType = ControlType.Mouse;
	/* 
	[Tooltip("Enable trackballs (mouse x/y) for wheel movement")]
	public bool UseMouse = false;
	[Tooltip("Enable keys (w/s, e/d) for wheel movement")]
	public bool UseKeys = false;
	*/
	[Tooltip("Adjustments for trackball direction and relative (to eachother) speed")]
	public Vector2 MouseAdjust = new Vector2(-1f, 1f);

	/*	mouse
	speed 8
	turn sp 0.04
	acc 1
	damping 10
	fw c 0.4
	t sp 80

	kbd
	speed 30
	turn sp .2
	fw c 1
	 */

	/*
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
	 */

	public InputActionAsset controls;

	public MovementSettings[] AllMovementSettings ={
		new MovementSettings{ // mouse
			Speed = 8f,
			TurningSpeed = 0.04f,
			Acceleration = 1,
			Damping = 10,
			ForwardCorrectionSpeed = 0.4f,
			TopSpeed = 80
		},
		new MovementSettings{ // keyboard
			Speed = 30,
			TurningSpeed = 0.2f,
			Acceleration = 1,
			Damping = 10,
			ForwardCorrectionSpeed = 1,
			TopSpeed = 80
		},
		new MovementSettings{ // controller
			Speed = 8f,
			TurningSpeed = 0.04f,
			Acceleration = 1,
			Damping = 10,
			ForwardCorrectionSpeed = 0.4f,
			TopSpeed = 80
		},
		// TODO: touch
	};

	public MovementSettings CurrentMovementSettings {
		get {
			switch (CurrentControlType) {
				case ControlType.Mouse:
					return AllMovementSettings[0];
				case ControlType.Keyboard:
					return AllMovementSettings[1];
				case ControlType.Controller:
					return AllMovementSettings[2];
				default:
					return AllMovementSettings[0];

			}
		}
	}

	private float leftStickValue = 0;
	private float rightStickValue = 0;

	public bool EnableCollision = false;
	[Tooltip("Scale of how much speed is kept when bouncing off a wall")]
	public float CollisionSlowdownMultiplier = 1.0f;

	[Tooltip("How many seconds before regaining control after bouncing off wall")]
	public float CollisionTime = 0.5f;
	private Timer collisionTimer;
	private bool collidedThisFrame = false;
	// private float knockbackSpeed = 0f;
	public float MinCollisionKnockbackSpeed = 0.1f;

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
	public float LeftWheelSpeed = 0.0f;
	[HideInInspector]
	public float RightWheelSpeed = 0.0f;


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
	public float JumpScoreWorth = 100f;
	public float JumpScoreMultiplierWorth = 0.1f;

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

	[Tooltip("Ammo amount, in %")]
	public float BoostAmmo = 1f;
	[Tooltip("How fast (% per sec) the boost canister is drained when used")]
	public float BoostAmmoDrainRate = .1f;
	[Tooltip("How fast (% per sec) the boost canister is refilled when not used")]
	public float BoostAmmoUpkeep = .1f;
	[Tooltip("How much (in %) the boost canister needs to be filled to start boosting")]
	public float BoostAmmoDisableTreshold = .25f;

	private bool boostToggle = false;

	// Zipline
	private bool ziplining = false;
	private Vector3 ziplineTarget;
	private float ziplineSpeed;

	#endregion

	void Start() {

		//Mick start
		AS_Boing.clip = SFX_Boing;
		//Mick end

		Globals.Player = this;

		StandingKid.SetActive(true);
		StandingKidZipline.SetActive(false);


		// networkSendTimer = new Timer(networkSendTime);
		/*
		Network = Globals.DriverNetworkMode;
		if (Network == NetworkMode.Receive) {
			UdpClient receivingUdpClient = new UdpClient(11000);
			Task UdpListener = new Task(() => { UdpUtilities.UdpLoop(receivingUdpClient, messageQueue); });
			UdpListener.Start();
		} else if (Network == NetworkMode.Send) {
			udpClient = new UdpClient(11001);
		}
		*/

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

		// input

		if (!OverrideGlobalControls) {
			CurrentControlType = Globals.ControlType;
			FlipWheels = Globals.FlipWheels;
		}

		if (CurrentControlType == ControlType.Mouse) {
			Cursor.lockState = CursorLockMode.Locked;
			Cursor.visible = false;
		}

		switch (CurrentControlType) {
			case ControlType.Mouse:
				Cursor.lockState = CursorLockMode.Locked;
				Cursor.visible = false;
				break;
			default:
				Cursor.lockState = CursorLockMode.None;
				Cursor.visible = true;
				break;
		}

		{
			InputAction action = controls.FindActionMap("driver").FindAction("left wheel joystick");
			action.performed += c => {
				leftStickValue = c.ReadValue<float>();
				// Debug.Log("left stick " + c.ReadValue<float>());
			};
			action.canceled += _ => {
				leftStickValue = 0;
			};
			action.Enable();

		}

		{
			InputAction action = controls.FindActionMap("driver").FindAction("right wheel joystick");
			action.performed += c => {
				rightStickValue = c.ReadValue<float>();
				// Debug.Log("right stick " + c.ReadValue<float>());
			};
			action.canceled += _ => {
				rightStickValue = 0;
			};
			action.Enable();

		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("controller boost");
			action.performed += c => {
				boostToggle = true;
			};
			action.canceled += _ => {
				boostToggle = false;
			};
			action.Enable();

		}

		{
			InputAction action = controls.FindActionMap("driver").FindAction("controller brake");
			action.performed += c => {
				// TODO: brake
			};
			action.canceled += _ => {
				// TODO: stop braking
			};
			action.Enable();

		}



	}


	void Update() {

		/*
		if (Network == NetworkMode.Receive) {
			while (true) {
				bool successful = messageQueue.TryDequeue(out Vector2 pos);
				if (successful) {
					Vector3 tempPos = transform.position;
					tempPos.x = pos.x;
					tempPos.z = pos.y;
					transform.position = tempPos;
					Globals.NotificationPanel.Notify("dequeued position");
				} else {
					break;
				}
			}
			return;
		}
		*/

		if (DisableMovement) {
			UpdateWheels();
			SpinWheels();
			return;
		}

		collidedThisFrame = false;

		var keyboard = Keyboard.current;

		if (UpdateCollision()) {
		} else if (UpdateZipline()) {
		} else if (UpdateJump()) {
		} else {

			// if (Mouse.current.rightButton.isPressed) {
			if (keyboard.digit2Key.isPressed || boostToggle) {
				// && !boostTimer.IsRunning()) {
				// boostTimer.Restart(BoostTime);
				Boost();
			}

			if (UpdateBoost()) {
			} else {


				UpdateWheels();
				Turn();
				MoveForward();
				SpinWheels();
			}

		}

		NozzleAnchor.UpdateAnchor();

		/*
		if (Network == NetworkMode.Send) {
			// TODO: message ordering
			byte[] udpBytes = new byte[9];
			udpBytes[0] = 1;
			Array.Copy(BitConverter.GetBytes(transform.position.x), 0, udpBytes, 1, 4);
			Array.Copy(BitConverter.GetBytes(transform.position.y), 0, udpBytes, 5, 4);

			udpClient.SendAsync(udpBytes, 9, new IPEndPoint(IPAddress.Loopback, 11002));
			return;
		}
		*/

	}

	private void MoveForward() {
		float speed = LeftWheelSpeed + RightWheelSpeed;

		if (drifting) {
			driftSpeed = Mathf.MoveTowards(
				driftSpeed,// + speed * DriftSpeedAddScale, 
				0, 
				DriftDamping * Time.deltaTime
				);
			transform.position += transform.forward * driftSpeed * Time.deltaTime;
		} else {
			float boostSlowdownProgress = 0f;
			if (boostSlowdownTimer.IsRunning()) {
				boostSlowdownProgress = boostSlowdownTimer.TimeLeft() / BoostSlowdownTime;
			}

			float truncatedSpeed = 0f;
			if (speed < 0f) {
				truncatedSpeed = Mathf.Max(speed, -CurrentMovementSettings.TopSpeed);
			} else {
				truncatedSpeed = Mathf.Min(speed, CurrentMovementSettings.TopSpeed);
			}

			transform.position += transform.forward
			* (truncatedSpeed + boostEndSpeed * boostSlowdownProgress)
			 * Time.deltaTime;
		}
	}

	private void Turn() {

		float speed = LeftWheelSpeed + RightWheelSpeed;
		float angle = LeftWheelSpeed - RightWheelSpeed;
		angle *= CurrentMovementSettings.TurningSpeed;
		//angle %= Mathf.PI * 2.0f;

		if ((LeftWheelSpeed > 0f && RightWheelSpeed > 0f) || (LeftWheelSpeed < 0f && RightWheelSpeed < 0f)) {
			angle = Mathf.MoveTowards(angle, 0, CurrentMovementSettings.ForwardCorrectionSpeed);
		}

		if (Mathf.Abs(angle) < DriftAngleThreshold
		|| (drifting && Mathf.Abs(driftSpeed) < DriftEndSpeedThreshold)
		// || (drifting && Mathf.Abs(driftSpeed) < DriftSpeedThreshold)
		|| (!drifting && Mathf.Abs(speed) < DriftStartSpeedThreshold)
		) {
			// NOTE: Not drifring

			if (drifting) {
				drifting = false;
				// NOTE: On drift end
				// IDEA: Play explosive sound (with notification?) if wheel velocity is much larger than drift speed

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

			// NOTE: Drifting	
			// IDEA: compare turn delta to velocity, trigger drift event if turning too fast.
			// IDEA: when drift mode triggers, a timer starts, during which you can turn independently from movement direction.
			// IDEA: instead of timer, stop drifting when trajectory is within x degrees of wheelchair angle, x degrees depends on movement/drifting speed
			// IDEA: moving while drifting will alter trajectory in turning direction.

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
					//* trajectorySpeedChange
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

	}

	private bool UpdateCollision() {

		// FIXME: null reference in build only
		boostSlowdownTimer.Update();

		if (collisionTimer.IsRunning()) {
			collisionTimer.Update();
			float boostSlowdownProgress = 0f;
			if (boostSlowdownTimer.IsRunning()) {
				boostSlowdownProgress = boostSlowdownTimer.TimeLeft() / BoostSlowdownTime;
			}

			// UpdateWheels();
			// Turn();

			/*
			if (drifting) {
				driftSpeed = Mathf.MoveTowards(driftSpeed + speed * DriftSpeedAddScale, 0, DriftDamping * Time.deltaTime);
				transform.position += transform.forward * driftSpeed * Time.deltaTime;
			} else {
				float boostSlowdownProgress = 0f;
				if (boostSlowdownTimer.IsRunning()) {
					boostSlowdownProgress = boostSlowdownTimer.TimeLeft() / BoostSlowdownTime;
				}

				float truncatedSpeed = 0f;
				if (speed < 0f) {
					truncatedSpeed = Mathf.Max(speed, -TopSpeed);
				} else {
					truncatedSpeed = Mathf.Min(speed, TopSpeed);
				}

				transform.position += transform.forward
				* (truncatedSpeed + boostEndSpeed * boostSlowdownProgress)
				 * Time.deltaTime;
			}
			// */

			// transform.position += transform.forward
			//  * (-knockbackSpeed - boostSlowdownProgress * boostEndSpeed)
			//  * Time.deltaTime;

			float tempSpeed = LeftWheelSpeed + RightWheelSpeed - boostSlowdownProgress * boostEndSpeed;
			if (tempSpeed < 0f) {
				tempSpeed = Mathf.Min(tempSpeed, -MinCollisionKnockbackSpeed);
			} else {
				tempSpeed = Mathf.Max(tempSpeed, MinCollisionKnockbackSpeed);
			}

			transform.position += transform.forward
			 * tempSpeed
			 * Time.deltaTime;

			/*
			// float turnAngle = rightWheelSpeed - leftWheelSpeed;
			float turnAngle = leftWheelSpeed - rightWheelSpeed;
			turnAngle *= TurningSpeed;
			turnAngle = Mathf.MoveTowards(turnAngle, 0, ForwardCorrectionSpeed);
			transform.Rotate(transform.up, turnAngle * Time.deltaTime * 60);
			// */


			SpinWheels();

			if (boostTimer.IsRunning()) {
				if (boostTimer.Update()) {
					boostEndSpeed = Mathf.Max(LeftWheelSpeed, RightWheelSpeed);
					boostSlowdownTimer.Restart(BoostSlowdownTime);
					StopBoostParticles();
				}
			}

			return true;
		}
		return false;
	}

	private bool UpdateBoost() {
		if (boostTimer.IsRunning()) {
			if (boostTimer.Update()) {
				float x = 0f;
				float y = 0f;
				if (CurrentControlType == ControlType.Mouse) {
					x = Input.GetAxis("Mouse X") * MouseAdjust.x * CurrentMovementSettings.Speed;
					y = Input.GetAxis("Mouse Y") * MouseAdjust.y * CurrentMovementSettings.Speed;
				}
				if (FlipWheels) {
					boostEndSpeed = Mathf.Max(LeftWheelSpeed - y, RightWheelSpeed - x);
				} else {
					boostEndSpeed = Mathf.Max(LeftWheelSpeed - x, RightWheelSpeed - y);
				}
				boostSlowdownTimer.Restart(BoostSlowdownTime);
				StopBoostParticles();
			} else {
				LeftWheelSpeed = Mathf.MoveTowards(LeftWheelSpeed, BoostMaxSpeed, BoostAcceleration * Time.deltaTime);
				RightWheelSpeed = Mathf.MoveTowards(RightWheelSpeed, BoostMaxSpeed, BoostAcceleration * Time.deltaTime);
				transform.position += transform.forward * (LeftWheelSpeed + RightWheelSpeed) * Time.deltaTime;
				LeftWheel.transform.Rotate(-WheelRotationAxis, LeftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
				RightWheel.transform.Rotate(WheelRotationAxis, RightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);

				BoostAmmo -= BoostAmmoDrainRate * Time.deltaTime;
				if (BoostAmmo < 0f) {
					BoostAmmo = 0f;

					boostTimer.Stop();
					float x = 0f;
					float y = 0f;
					if (CurrentControlType == ControlType.Mouse) {
						x = Input.GetAxis("Mouse X") * MouseAdjust.x * CurrentMovementSettings.Speed;
						y = Input.GetAxis("Mouse Y") * MouseAdjust.y * CurrentMovementSettings.Speed;
					}
					if (FlipWheels) {
						boostEndSpeed = Mathf.Max(LeftWheelSpeed - y, RightWheelSpeed - x);
					} else {
						boostEndSpeed = Mathf.Max(LeftWheelSpeed - x, RightWheelSpeed - y);
					}
					boostSlowdownTimer.Restart(BoostSlowdownTime);
					StopBoostParticles();
				}

				return true;
			}
		} else {
			BoostAmmo += BoostAmmoUpkeep * Time.deltaTime;
			if (BoostAmmo > 1f) {
				BoostAmmo = 1f;
			}
		}

		return false;
	}

	private bool UpdateJump() {
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
					// Debug.Log("temp: " + tempJumpHeight + ", lower threshold: " + (jumpTargetY - playerY));
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

				SpinWheels();
				return true;
			}
		}

		return false;
	}

	private bool UpdateZipline() {
		if (ziplining) {
			float ziplineDelta = ziplineSpeed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(transform.position, ziplineTarget, ziplineSpeed);
			if (Vector3.Distance(transform.position, ziplineTarget) < 0.01f) {
				ziplining = false;

				StandingKid.SetActive(true);
				StandingKidZipline.SetActive(false);
				StopBoostParticles();

				// jumpTargetY = playerY;
				// playerY = transform.position.y;
				// skipUp = true;
				// jumpTimer.Restart();
				StartJump();
			}
			return true;
		}

		return false;
	}

	private void SpinWheels() {
		LeftWheel.transform.Rotate(-WheelRotationAxis, LeftWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
		RightWheel.transform.Rotate(WheelRotationAxis, RightWheelSpeed * WheelAnimationSpeed * Time.deltaTime * 60);
	}

	private void OnTriggerEnter(Collider other) {

		// NOTE: ziplining through buildings is allowed
		if (!EnableCollision || other.tag == "Ignore Collision" || ziplining) {
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

				StandingKid.SetActive(false);
				StandingKidZipline.SetActive(true);
				StartBoostParticles();

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

				playerY = ziplineTarget.y;

				Globals.AddScore(zipline.ScoreWorth, zipline.ScoreMultiplierIncrease);
				SetupJump(
					zipline.TargetHeightRelativity, zipline.TargetHeight,
					JumpTargetSetting.Relative, 1,
					zipline.SkipUp,
					true, zipline.EndJumpSpeed,
					true, zipline.EndJumpTime,
					true, zipline.End.transform.rotation,
					// TODO: bool to use default stunt settings
					StuntAngle, StuntAxis, StuntPingPong//tempStuntAngle, tempStuntAxis, tempStuntPingPong
				);

			} else {
				Debug.LogError("Zipline script not found!");
			}
			return;
		}

		if (other.tag == "Ramp") {
			Debug.Log("hit ramp " + other.name + "!");

			// TODO: stop drift trail on jump, resume on landing, for both ramps and ziplines

			// NOTE: ignores jump if already in air
			if (jumpTimer.IsRunning()) {
				return;
			}

			CancelBoost();

			RampScript rampScript = other.GetComponent<RampScript>();

			if (rampScript != null) {
				float facingDifference = Quaternion.Angle(transform.rotation, rampScript.transform.rotation);
				if (rampScript.AlignPlayer &&
				  (rampScript.JumpNormallyIfWrongWay &&
					   ((
					   //Speed > 0f && // FIXME: supposed to use current combined wheel speed, but somehow works anyway?
					   facingDifference > 90f)
					  // || (Speed < 0f && facingDifference < 90f)
					  )
				  )
				) {
					Globals.AddScore(JumpScoreWorth, JumpScoreMultiplierWorth);
					StartJump();
				} else {
					float speed = rampScript.Speed;
					if (!rampScript.AlignPlayer && LeftWheelSpeed + RightWheelSpeed < 0f) {
						speed *= -1f;
					}

					Globals.AddScore(rampScript.ScoreWorth, rampScript.ScoreMultiplierIncrease);
					StartJump(
						rampScript.TargetHeightRelativity, rampScript.TargetHeight,
						rampScript.JumpHeightRelativity, rampScript.JumpHeight,
						rampScript.SkipUp,
						rampScript.SetSpeed, speed,
						rampScript.SetTime, rampScript.Time,
						rampScript.AlignPlayer, rampScript.transform.rotation,
						rampScript.StuntAngle, rampScript.StuntAxis, rampScript.StuntPingPong
					);
				}
			} else {
				Globals.AddScore(JumpScoreWorth, JumpScoreMultiplierWorth);
				StartJump();
			}

			return;
		}

		Debug.Log("hit wall: " + other.name);

		//Mick Start
		AS_Boing.Play();
		//Mick End

		if (collidedThisFrame) {
			Debug.Log("ignored wall: " + other.name);
			return;
		}

		collidedThisFrame = true;

		if (jumpTimer.IsRunning()) {
			transform.Rotate(Vector3.up, 180f);
		} else {
			collisionTimer.Restart(CollisionTime);
			float tempLeftSpeed = LeftWheelSpeed;
			LeftWheelSpeed = -RightWheelSpeed;
			RightWheelSpeed = -tempLeftSpeed;
			// knockbackSpeed = leftWheelSpeed + rightWheelSpeed;
			// knockbackSpeed *= CollisionSlowdownMultiplier;
		}

		LeftWheelSpeed *= CollisionSlowdownMultiplier;
		RightWheelSpeed *= CollisionSlowdownMultiplier;
	}

	private void UpdateWheels() {
		Keyboard keyboard = Keyboard.current;

		if (CurrentControlType == ControlType.Mouse) {
			if (!keyboard.leftShiftKey.isPressed) {

				// float x = Mouse.current.delta.x.ReadValue() * MouseAdjust.x * Speed;
				// float y = Mouse.current.delta.y.ReadValue() * MouseAdjust.y * Speed;
				float x = Input.GetAxis("Mouse X") * MouseAdjust.x * CurrentMovementSettings.Speed;
				float y = Input.GetAxis("Mouse Y") * MouseAdjust.y * CurrentMovementSettings.Speed;

				if (FlipWheels) {
					LeftWheelSpeed = x;
					RightWheelSpeed = y;
				} else {
					LeftWheelSpeed = y;
					RightWheelSpeed = x;
				}
			}

		} else if (CurrentControlType == ControlType.Keyboard) {

			float leftWheelDir = 0.0f;
			float rightWheelDir = 0.0f;

			if (keyboard.wKey.isPressed) {
				if (FlipWheels) {
					rightWheelDir = CurrentMovementSettings.Acceleration;
				} else {
					leftWheelDir = CurrentMovementSettings.Acceleration;
				}
			} else if (keyboard.sKey.isPressed) {
				if (FlipWheels) {
					rightWheelDir = -CurrentMovementSettings.Acceleration;
				} else {
					leftWheelDir = -CurrentMovementSettings.Acceleration;
				}
			}

			if (keyboard.eKey.isPressed) {
				if (FlipWheels) {
					leftWheelDir = CurrentMovementSettings.Acceleration;
				} else {
					rightWheelDir = CurrentMovementSettings.Acceleration;
				}
			} else if (keyboard.dKey.isPressed) {
				if (FlipWheels) {
					leftWheelDir = -CurrentMovementSettings.Acceleration;
				} else {
					rightWheelDir = -CurrentMovementSettings.Acceleration;
				}
			}

			// damping
			LeftWheelSpeed = Mathf.MoveTowards(LeftWheelSpeed, 0.0f, CurrentMovementSettings.Damping * Time.deltaTime);
			RightWheelSpeed = Mathf.MoveTowards(RightWheelSpeed, 0.0f, CurrentMovementSettings.Damping * Time.deltaTime);

			// add to speed if pressed
			LeftWheelSpeed = Mathf.MoveTowards(LeftWheelSpeed, CurrentMovementSettings.TopSpeed * (leftWheelDir / CurrentMovementSettings.Acceleration), Mathf.Abs(leftWheelDir) * CurrentMovementSettings.Speed * Time.deltaTime);
			RightWheelSpeed = Mathf.MoveTowards(RightWheelSpeed, CurrentMovementSettings.TopSpeed * (rightWheelDir / CurrentMovementSettings.Acceleration), Mathf.Abs(rightWheelDir) * CurrentMovementSettings.Speed * Time.deltaTime);

			if (keyboard.spaceKey.isPressed) {
				LeftWheelSpeed = 0.0f;
				RightWheelSpeed = 0.0f;
			}

		} else if (CurrentControlType == ControlType.Controller) {

			// float leftStickY = Gamepad.current.leftStick.y.ReadValue();
			// float rightStickY = Gamepad.current.rightStick.y.ReadValue();
			float leftStickY = leftStickValue;
			float rightStickY = rightStickValue;

			if (!FlipWheels) {
				LeftWheelSpeed = Mathf.MoveTowards(LeftWheelSpeed, CurrentMovementSettings.TopSpeed * leftStickY, CurrentMovementSettings.Acceleration * Time.deltaTime);
				RightWheelSpeed = Mathf.MoveTowards(RightWheelSpeed, CurrentMovementSettings.TopSpeed * rightStickY, CurrentMovementSettings.Acceleration * Time.deltaTime);
			} else {
				LeftWheelSpeed = Mathf.MoveTowards(LeftWheelSpeed, CurrentMovementSettings.TopSpeed * rightStickY, CurrentMovementSettings.Acceleration * Time.deltaTime);
				RightWheelSpeed = Mathf.MoveTowards(RightWheelSpeed, CurrentMovementSettings.TopSpeed * leftStickY, CurrentMovementSettings.Acceleration * Time.deltaTime);
			}
		}
	}

	#region Boost
	public void Boost(float boostTime) {
		if (BoostAmmo < BoostAmmoDisableTreshold && !boostTimer.IsRunning()) {
			return;
		}
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
	#endregion

	#region Jump
	public void SetupJump(
		JumpTargetSetting TargetHeightRelativity, float TargetHeight,
		JumpTargetSetting JumpHeightRelativity, float JumpHeight,
		bool SkipNextUp,
		bool SetSpeed, float newSpeed,
		bool SetTime, float Time,
		bool AlignPlayer, Quaternion Rotation,
		float tempStuntAngle, Vector3 tempStuntAxis, bool tempStuntPingPong
	) {
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
		// jumpTargetY /= transform.lossyScale.y;

		useTempJumpHeight = true;
		switch (JumpHeightRelativity) {
			case JumpTargetSetting.Absolute:
				tempJumpHeight = JumpHeight - playerY;
				break;
			case JumpTargetSetting.Relative:
				tempJumpHeight = JumpHeight;
				break;
			case JumpTargetSetting.Reset:
				tempJumpHeight = initialPlayerY - playerY + JumpHeight;
				break;
		}
		// tempJumpHeight /= transform.lossyScale.y;
		Debug.Log("jump height result: " + tempJumpHeight + ", jump height: " + JumpHeight);

		skipUp = SkipNextUp;
		setJumpSpeed = SetSpeed;
		nextJumpSpeed = newSpeed;
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
		bool SetSpeed, float newSpeed,
		bool SetTime, float Time,
		bool AlignPlayer, Quaternion Rotation,
		float tempStuntAngle, Vector3 tempStuntAxis, bool tempStuntPingPong
	) {

		SetupJump(
			TargetHeightRelativity, TargetHeight,
			JumpHeightRelativity, JumpHeight,
			SkipNextUp,
			SetSpeed, newSpeed,
			SetTime, Time,
			AlignPlayer, Rotation,
			tempStuntAngle, tempStuntAxis, tempStuntPingPong
		);

		StartJump();
	}

	public void StartJump() {
		AS_Boing.Play();

		if (!setJumpTime) {
			nextJumpTime = JumpTime;
		}
		jumpTimer.Restart(nextJumpTime);
		preJumpRotation = transform.localRotation;
		if (setJumpSpeed) {
			jumpSpeed = nextJumpSpeed;
		} else {
			jumpSpeed = LeftWheelSpeed + RightWheelSpeed;
		}
	}
	#endregion

}
