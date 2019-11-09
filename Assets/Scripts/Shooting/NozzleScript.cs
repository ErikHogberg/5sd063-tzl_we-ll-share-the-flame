using UnityEngine;
// using UnityEngine.Experimental.Input;
using WiimoteApi;
using Assets.Scripts.Utilities;
using Assets.Scripts;
using UnityEngine.InputSystem;

public class NozzleScript : MonoBehaviour {

	// [SerializeField]
	public InputActionAsset controls;

	public GameObject WaterJet;
	public GameObject Foam;
	private ParticleSystem[] waterJetParticles;
	private ParticleSystem[] foamParticles;
	public SpraySound ExtinguisherSound;

	public bool DisableFiring = false;

	public bool particleModeUseWater = false;

	[Tooltip("Amount of foam in tank")]
	public float AmmoAmount = 1f;
	public float AmmoConsumptionRate = 0f;

	public float turnSpeedX = 4.0f;
	public float turnSpeedY = 2.0f;

	// public bool AlwaysFiring = false;
	private bool firing = false;
	private bool wasFiring = false;

	private Vector3 offset;

	public float MaxFoamSpread = 10.0f;
	public float MinFoamSpread = 0.1f;

	public float MaxFoamSpeed = 10;
	public float MinFoamSpeed = 1;

	public float FoamSwitchSpeed = 0.1f;

	public bool AllowAimWithMouse = false;
	private float yaw = 0.0f;
	private float pitch = 90.0f;
	public float MaxUpPitch = 85.0f;
	public float MaxDownPitch = 45.0f;

	public bool FlipYaw = false;
	public bool FlipPitch = false;
	public bool FlipRoll = false;
	public bool IgnoreRoll = true;

	// wiimote
	public bool OverrideGlobal = false;
	public bool EnableWiimote = false;
	private Wiimote wiimote;
	public bool DisableRumble = false;

	private bool wiimoteWasFiring = false;

	public float WiimoteYawSensitivity = 1.0f;
	public float WiimotePitchSensitivity = 1.0f;
	public float WiimoteRollSensitivity = 1.0f;

	private Vector3 wiimoteOrientation = new Vector3(0f, 90f, 0f); // Yaw, Pitch, Roll

	// public IRDataType InitialSensorBarMode = IRDataType.BASIC;

	// private RectTransform ir_pointer;
	public Vector2 SensorBarAngleScale = new Vector2(10f, 10f);
	public Vector2 SensorBarAngleOffset = new Vector2(0f, 90f);

	private Timer ledTimer;
	private int ledState = 0;

	private bool dLeftWasPressed = false;
	private bool dRightWasPressed = false;
	private bool aWasPressed = false;
	// private bool irToggle = false;
	public Vector2 IrOuterDeadzone = new Vector2(0f, 0f);

	public bool UseAccelerometer = false;

	private bool controllerAimToggle = false;
	public Vector2 GamepadAimSpeed = Vector2.one;
	private Vector2 GamepadAim = Vector2.zero;

	void Start() {

		Globals.Nozzle = this;
		// Globals.SensorBarMode = InitialSensorBarMode;

		if (!OverrideGlobal) {
			EnableWiimote = !Globals.UseWiimote;
		}

		foamParticles = Foam.GetComponentsInChildren<ParticleSystem>();
		waterJetParticles = WaterJet.GetComponentsInChildren<ParticleSystem>();

		foreach (ParticleSystem particles in foamParticles) {
			particles.Pause();
		}
		foreach (ParticleSystem particles in waterJetParticles) {
			particles.Pause();
		}

		// var action = new InputAction(binding: "*/{primaryAction}");
		//  var action = new InputAction(binding: "shooter/{shoot}");

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("shoot");
			action.performed += _ => { firing = true; };
			action.canceled += _ => { firing = false; };
			action.Enable();
		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("switch foam");
			action.performed += _ => { SwitchParticles(firing); };
			action.Enable();
		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("controller shoot");
			action.performed += _ => { firing = true; };
			action.canceled += _ => { firing = false; };
			action.Enable();
		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("controller switch foam");
			action.performed += _ => { SwitchParticles(firing); };
			action.Enable();
		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("controller aim modifier");
			action.performed += _ => { controllerAimToggle = true; };
			action.canceled += _ => { controllerAimToggle = false; };
			action.Enable();
		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("controller aim x");
			action.performed += callback => {
				if (controllerAimToggle) {
					// Debug.Log("gamepad x: " + callback.ReadValue<float>());
					// yaw += callback.ReadValue<float>() * GamepadAimSpeed.x * Time.deltaTime;
					GamepadAim.x = callback.ReadValue<float>();
				}
			};
			action.Enable();
		}

		{
			InputAction action = controls.FindActionMap("shooter").FindAction("controller aim y");
			action.performed += callback => {
				if (controllerAimToggle) {
					// Debug.Log("gamepad y: " + callback.ReadValue<float>());
					// pitch += callback.ReadValue<float>() * GamepadAimSpeed.y * Time.deltaTime;
					GamepadAim.y = callback.ReadValue<float>();
				}
			};
			action.Enable();
		}

		ledTimer = new Timer(0.1f);

	}

	void Update() {

		Keyboard keyboard = Keyboard.current;

		bool wiimoteFiring = false;

		if (EnableWiimote) {
			if (!WiimoteManager.HasWiimote()) {
				WiimoteManager.FindWiimotes();
				// Debug.Log("scanned wiimotes");
			} else {
				wiimoteFiring = WiimoteUpdate();
			}
		}



		//if (Mouse.current.leftButton.isPressed) {
		if (!DisableFiring && (firing || wiimoteFiring) && AmmoAmount > 0f) {
			if (!wasFiring) {
				if (particleModeUseWater) {
					foreach (ParticleSystem particles in waterJetParticles) {
						particles.Play();
					}
				} else {
					foreach (ParticleSystem particles in foamParticles) {
						particles.Play();
					}
				}

				ExtinguisherSound.PlaySound();
				wasFiring = true;
			}

			AmmoAmount -= AmmoConsumptionRate * Time.deltaTime;
			if (AmmoAmount < 0f) {
				AmmoAmount = 0f;
			}

			if (AllowAimWithMouse) {
				yaw += Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime;
				pitch += Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime;
			}

		} else {
			if (wasFiring) {

				if (particleModeUseWater) {
					foreach (ParticleSystem particles in waterJetParticles) {
						particles.Stop();
					}
				} else {
					foreach (ParticleSystem particles in foamParticles) {
						particles.Stop();
					}
				}

				ExtinguisherSound.StopSound();
				wasFiring = false;
			}
		}

		// gamepad aim
		yaw = GamepadAim.x * GamepadAimSpeed.x;
		pitch = GamepadAim.y * GamepadAimSpeed.y + 90f;

		if (pitch < 90.0f - MaxUpPitch) {
			pitch = 90.0f - MaxUpPitch;
		} else
		if (pitch > 90.0f + MaxDownPitch) {
			pitch = 90.0f + MaxDownPitch;
		}

		transform.localRotation = Quaternion.identity
			* Quaternion.AngleAxis(yaw, Vector3.up)
			* Quaternion.AngleAxis(pitch, Vector3.right)
		;

		if (Globals.CameraScript != this) {
			Globals.CameraScript.UpdateAimOffset(yaw, -pitch + 90f);
		}

	}

	private bool WiimoteUpdate() {
		Keyboard keyboard = Keyboard.current;

		wiimote = WiimoteManager.Wiimotes[0];

		UpdateLEDs();

		int ret;
		do {
			ret = wiimote.ReadWiimoteData();

			// FIXME: ret returns less than 1 when reporting motion plus rotation
			// if (ret < 1) {
			// 	Debug.Log("ret less than 1");
			// 	continue;
			// }

			if (wiimote.current_ext != ExtensionController.MOTIONPLUS) {
				wiimote.RequestIdentifyWiiMotionPlus(); // find/prepare wmp
				wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_EXT8); // set data reporting to support wmp
				wiimote.ActivateWiiMotionPlus(); // force wmp
			} else {

				if (UseAccelerometer) {
					float[] accel = wiimote.Accel.GetCalibratedAccelData();
					wiimoteOrientation = new Vector3(
						accel[0],
						-accel[2],
						-accel[1]
					);
				} else {

					Vector3 offset = new Vector3(
						wiimote.MotionPlus.YawSpeed * WiimoteYawSensitivity,
						wiimote.MotionPlus.PitchSpeed * WiimotePitchSensitivity,
						wiimote.MotionPlus.RollSpeed * WiimoteRollSensitivity
						) / 95f; // Divide by 95Hz (average updates per second from wiimote)

					if (FlipYaw)
						offset.x *= -1;
					if (FlipPitch)
						offset.y *= -1;
					if (FlipRoll)
						offset.z *= -1;

					wiimoteOrientation += offset;

					float rollRadians = 0f;
					if (!IgnoreRoll)
						rollRadians = wiimoteOrientation.z * Mathf.Deg2Rad;

					yaw = wiimoteOrientation.x * Mathf.Cos(rollRadians)
					+ wiimoteOrientation.y * Mathf.Sin(rollRadians);

					pitch = wiimoteOrientation.y * Mathf.Cos(rollRadians)
					+ wiimoteOrientation.x * Mathf.Sin(rollRadians);

					if (wiimote.Button.minus) {
						Debug.Log("yaw: " + yaw + ", pitch: " + pitch);
						Debug.Log("orientation: " + wiimoteOrientation);
					}

					// transform.localRotation *= Quaternion.Euler(offset);
				}

			}
		} while (ret > 0);

		// TODO: use wiimote.Accel.GetCalibratedAccelData() to reduce wmp drift


		if (wiimote.Button.d_left) {
			if (dLeftWasPressed) {

			} else {
				Globals.ToggleIr();

				dLeftWasPressed = true;
			}

		} else {
			dLeftWasPressed = false;
		}

		if (wiimote.Button.d_right) { // || keyboard.kKey.wasPressedThisFrame) {
			if (dRightWasPressed) {

			} else {
				Debug.Log("ir setup");
				wiimote.SetupIRCamera(Globals.SensorBarMode);

				dRightWasPressed = true;
			}
		} else if (dRightWasPressed) {
			dRightWasPressed = false;
		}

		if (Globals.irToggle) {
			// left 0, down 0
			float[] pointer = wiimote.Ir.GetPointingPosition();
			// Debug.Log("ir loop");
			// if (pointer[0] > -1f && pointer[1] > -1f) {
			if (pointer[0] > IrOuterDeadzone.x && pointer[0] < 1f - IrOuterDeadzone.x
			&& pointer[1] > IrOuterDeadzone.y && pointer[1] < 1f - IrOuterDeadzone.y) {
				wiimoteOrientation = new Vector3(
					(pointer[0] - 0.5f) * SensorBarAngleScale.x + SensorBarAngleOffset.x,
					 (pointer[1] - 0.5f) * SensorBarAngleScale.y + SensorBarAngleOffset.y,
					 0f
				);
				Debug.Log("orientation set from sensor bar");
			}

			// Debug.Log("pointer: " + pointer[0] + ", " + pointer[1]);
			// ir_pointer.anchorMin = new Vector2(pointer[0], pointer[1]);
			// ir_pointer.anchorMax = new Vector2(pointer[0], pointer[1]);

		}

		bool wiimoteFiring = wiimote.Button.b;

		if (wiimote.Button.a) {
			if (!aWasPressed) {
				SwitchParticles(wiimoteFiring);
			}
		} else {
			aWasPressed = false;
		}

		if (wiimote.Button.d_up) {
			// transform.localRotation = Quaternion.AngleAxis(90, transform.parent.right);
			wiimoteOrientation = new Vector3(0f, 90f, 0f);
			//wiimote.MotionPlus.SetZeroValues();
		}

		if (wiimote.Button.d_down || keyboard.jKey.wasPressedThisFrame) {
			wiimote.MotionPlus.SetZeroValues();
		}



		if (wiimoteFiring || firing) {
			if (wiimoteWasFiring) {

			} else {
				if (!DisableRumble) {
					wiimote.RumbleOn = true;
				}
				wiimoteWasFiring = true;
			}

		} else {
			if (wiimoteWasFiring) {
				wiimote.RumbleOn = false;
				wiimoteWasFiring = false;
			} else {
			}
		}

		return wiimoteFiring;

	}

	private void UpdateLEDs() {
		if (ledTimer.Update()) {
			ledState++;
			if (ledState > 3)
				ledState = 0;

			if (ledState == 0)
				wiimote.SendPlayerLED(true, true, false, false);
			if (ledState == 1)
				wiimote.SendPlayerLED(false, true, true, false);
			if (ledState == 2)
				wiimote.SendPlayerLED(false, false, true, true);
			if (ledState == 3)
				wiimote.SendPlayerLED(true, false, false, true);

			ledTimer.RestartWithDelta();
		}
	}

	public void SwitchParticles(bool nozzleIsFiring) {
		aWasPressed = true;
		Debug.Log("change foam mode");
		particleModeUseWater = !particleModeUseWater;
		if (nozzleIsFiring) {
			if (particleModeUseWater) {
				foreach (ParticleSystem particles in waterJetParticles) {
					particles.Play();
				}
				foreach (ParticleSystem particles in foamParticles) {
					particles.Stop();
				}
			} else {

				foreach (ParticleSystem particles in foamParticles) {
					particles.Play();
				}
				foreach (ParticleSystem particles in waterJetParticles) {
					particles.Stop();
				}
			}
		}
	}

	public void SwitchParticles() {
		SwitchParticles(firing);
	}

	public void SetFiring(bool fire) {
		firing = fire;
	}

	public void AddYawPitch(float yawAdd, float pitchAdd) {
		yaw += yawAdd;
		pitch += pitchAdd;
	}

}
