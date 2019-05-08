using UnityEngine;
using UnityEngine.Experimental.Input;
using WiimoteApi;
using Assets.Scripts.Utilities;

public class NozzleScript : MonoBehaviour {

	[SerializeField]
	private InputActionAsset controls;

	public GameObject Foam;
	private ParticleSystem[] foamParticles;

	public float turnSpeedX = 4.0f;
	public float turnSpeedY = 2.0f;

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

	// wiimote
	private Wiimote wiimote;
	private Vector3 wmpOffset = Vector3.zero;

	public float WiimoteSensetivityX = 1.0f;
	public float WiimoteSensetivityY = 1.0f;

	private float wiimoteYaw = 0.0f;
	private float wiimotePitch = 90.0f;

	public RectTransform ir_pointer;

	private Timer ledTimer;
	private int ledState = 0;

	void Start() {

		foamParticles = Foam.GetComponentsInChildren<ParticleSystem>();

		foreach (ParticleSystem particles in foamParticles) {
			particles.Pause();
		}

		// var action = new InputAction(binding: "*/{primaryAction}");
		//  var action = new InputAction(binding: "shooter/{shoot}");
		InputAction action = controls.TryGetActionMap("shooter").TryGetAction("shoot");
		action.performed += _ => { firing = true; };
		action.cancelled += _ => { firing = false; };
		action.Enable();

		ledTimer = new Timer(0.1f);
	}

	void Update() {

		Keyboard keyboard = Keyboard.current;

		bool wiimoteFiring = false;

		if (!WiimoteManager.HasWiimote()) {
			WiimoteManager.FindWiimotes();
		} else {
			wiimote = WiimoteManager.Wiimotes[0];

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

			int ret;
			do {
				ret = wiimote.ReadWiimoteData();

				// FIXME: ret returns less than 1 when reporting motion plus rotation
				// if (ret < 1) {
				// 	Debug.Log("ret less than 1");
				// 	continue;
				// }

				if (wiimote.current_ext != ExtensionController.MOTIONPLUS) {
					wiimote.RequestIdentifyWiiMotionPlus();
					wiimote.ActivateWiiMotionPlus();
				} else {

					Vector3 offset = new Vector3(
						wiimote.MotionPlus.PitchSpeed,
						0,
						// FIXME: ignoring roll messes with other axises
						// wiimote.MotionPlus.RollSpeed, 
						wiimote.MotionPlus.YawSpeed
						) / 95f; // Divide by 95Hz (average updates per second from wiimote)

					wmpOffset += offset;


					wiimoteYaw += -offset.z * WiimoteSensetivityX;
					wiimotePitch += offset.x * WiimoteSensetivityY;

					yaw = wiimoteYaw;
					pitch = wiimotePitch;

					// transform.localRotation *= Quaternion.Euler(offset);

				}
			} while (ret > 0);

			if (keyboard.gKey.wasPressedThisFrame) {
				wmpOffset = Vector3.zero;
			}
			if (keyboard.hKey.wasPressedThisFrame) {
				wiimote.RequestIdentifyWiiMotionPlus();
			}
			if (keyboard.jKey.wasPressedThisFrame) {
				wiimote.MotionPlus.SetZeroValues();
			}
			// TODO: re-aling using sensor bar

			if (wiimote.Button.a) {
				// transform.localRotation = Quaternion.AngleAxis(90, transform.parent.right);
				wiimoteYaw = 0.0f;
				wiimotePitch = 90.0f;
				//wiimote.MotionPlus.SetZeroValues();
			}
			if (wiimote.Button.d_down) {
				wmpOffset = Vector3.zero;
			}
			if (wiimote.Button.d_up) {
				wiimote.MotionPlus.SetZeroValues();
			}
			if (wiimote.Button.d_right) {
				wiimote.SetupIRCamera(IRDataType.FULL);
			}


			wiimoteFiring = wiimote.Button.b;
		}

		//if (Mouse.current.leftButton.isPressed) {
		if (firing || wiimoteFiring) {
			if (!wasFiring) {
				foreach (ParticleSystem particles in foamParticles) {
					particles.Play();
				}
				wasFiring = true;
			}

			// TODO: limit angle
			// IDEA: add "buffer" rotation that is assigned current rotation, but is capped to certain angles
			if (AllowAimWithMouse) {
				//transform.Rotate(
				//	Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime,
				//	0,
				//	Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime,
				//	Space.World
				//);

				//transform.localRotation *= Quaternion.AngleAxis(Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime, transform.InverseTransformVector(Vector3.up));
				//transform.localRotation *= Quaternion.AngleAxis(Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime, transform.InverseTransformVector(transform.right));

				yaw += Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime;
				pitch += Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime;

			}

		} else {
			if (wasFiring) {
				foreach (ParticleSystem particles in foamParticles) {
					particles.Stop();
				}
				wasFiring = false;
			}
		}


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

		// TODO: Apply spread change to all relevant particle effects
		// TODO: change percentually between min and max values, make both speed and spread reach their limits at the same time.
		/*
		if (keyboard.lKey.isPressed) {
			if (Foam.startSpeed > MinFoamSpeed) {
				Foam.startSpeed -= FoamSwitchSpeed;
			}

			// if(Foam.startSize < 3) {
			// 	Foam.startSize += 0.1f;
			// }

			ParticleSystem.ShapeModule shape = Foam.shape;

			if (shape.angle < MaxFoamSpread) {
				shape.angle += FoamSwitchSpeed;
			}

		} else if (keyboard.kKey.isPressed) {
			if (Foam.startSpeed < MaxFoamSpeed) {
				Foam.startSpeed += FoamSwitchSpeed;
			}

			// if (Foam.startSize > 0.2) {
			// 	Foam.startSize -= 0.1f;
			// }
			ParticleSystem.ShapeModule shape = Foam.shape;

			if (shape.angle > MinFoamSpread) {
				shape.angle -= FoamSwitchSpeed;
			}

		}
		//*/

	}

}
