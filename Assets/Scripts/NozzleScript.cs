using UnityEngine;
using UnityEngine.Experimental.Input;
using WiimoteApi;


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

	// wiimote
	private Wiimote wiimote;
	private Vector3 wmpOffset = Vector3.zero;

	//private Quaternion orientationBuffer = Quaternion.AngleAxis(60, Vector3.right);
	private float yaw = 0.0f;
	private float pitch = 90.0f;
	public Vector3 angleOut = Vector3.zero;
	public float MaxUpPitch = 85.0f;
	public float MaxDownPitch = 45.0f;

	void Start() {

		foamParticles = Foam.GetComponentsInChildren<ParticleSystem>();

		foreach (ParticleSystem particles in foamParticles) {
			particles.Pause();
		}

		// Create action that binds to the primary action control on all devices.
		// var action = new InputAction(binding: "*/{primaryAction}");
		//  var action = new InputAction(binding: "shooter/{shoot}");
		InputAction action = controls.TryGetActionMap("shooter").TryGetAction("shoot");
		// Have it run your code when action is triggered.
		action.performed += _ => { firing = true; };
		action.cancelled += _ => { firing = false; };
		// Start listening for control changes.
		action.Enable();
	}

	void Update() {

		Keyboard keyboard = Keyboard.current;

		bool wiimoteFiring = false;

		if (!WiimoteManager.HasWiimote()) {
			WiimoteManager.FindWiimotes();
		} else {
			wiimote = WiimoteManager.Wiimotes[0];

			int ret;
			do {
				ret = wiimote.ReadWiimoteData();
				// if (ret < 1) {
				// 	Debug.Log("ret less than 1");
				// 	continue;
				// }

				if (wiimote.current_ext != ExtensionController.MOTIONPLUS) {
					wiimote.RequestIdentifyWiiMotionPlus();
					wiimote.ActivateWiiMotionPlus();
				} else {
					if (keyboard.gKey.wasPressedThisFrame) {
						wmpOffset = Vector3.zero;
					}
					if (keyboard.hKey.wasPressedThisFrame) {
						wiimote.RequestIdentifyWiiMotionPlus();
					}
					if (keyboard.jKey.wasPressedThisFrame) {
						wiimote.MotionPlus.SetZeroValues();
					}



					Vector3 offset = new Vector3(
						wiimote.MotionPlus.PitchSpeed,
						0,
						// FIXME: ignoring roll messes with other axises
						// wiimote.MotionPlus.RollSpeed, 
						wiimote.MotionPlus.YawSpeed
						) / 95f; // Divide by 95Hz (average updates per second from wiimote)

					wmpOffset += offset;

					// TODO: re-aling using sensor bar

					if (wiimote.Button.a) {
						transform.localRotation = Quaternion.AngleAxis(90, transform.parent.right);
						//orientationBuffer = Quaternion.AngleAxis(90, transform.parent.right);
						// IDEA:
						//wiimote.MotionPlus.SetZeroValues();
					}
					if (wiimote.Button.d_down) {
						wmpOffset = Vector3.zero;
					}
					if (wiimote.Button.d_up) {
						wiimote.MotionPlus.SetZeroValues();
					}


					//orientationBuffer *= Quaternion.Euler(offset);
					transform.localRotation *= Quaternion.Euler(offset);

				}
			} while (ret > 0);

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

				//orientationBuffer *= Quaternion.AngleAxis(Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime, new Vector3(transform.forward.x + 45.0f, 0, 0));
				//orientationBuffer *= Quaternion.AngleAxis(Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime, Vector3.up);


				//transform.localRotation *= Quaternion.AngleAxis(Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime, transform.InverseTransformVector(Vector3.up));
				//transform.localRotation *= Quaternion.AngleAxis(Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime, transform.InverseTransformVector(transform.right));

				yaw += Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime;
				pitch += Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime;

				if (pitch < 90.0f - MaxUpPitch) {
					pitch = 90.0f - MaxUpPitch;
				} else
				if (pitch > 90.0f + MaxDownPitch) {
					pitch = 90.0f + MaxDownPitch;
				}

				//*
				transform.localRotation = Quaternion.identity
					* Quaternion.AngleAxis(yaw, Vector3.up)
					* Quaternion.AngleAxis(pitch, Vector3.right)
					;
				// */

				//transform.localRotation = Quaternion.Euler(yaw, pitch, 0);

			}

		} else {
			if (wasFiring) {
				foreach (ParticleSystem particles in foamParticles) {
					particles.Stop();
				}
				wasFiring = false;
			}
		}

		//float yaw = Mathf.Asin(2 * orientationBuffer.x * orientationBuffer.y + 2 * orientationBuffer.z * orientationBuffer.w);
		//float pitch = Mathf.Atan2(2 * orientationBuffer.x * orientationBuffer.w - 2 * orientationBuffer.y * orientationBuffer.z, 1 - 2 * orientationBuffer.x * orientationBuffer.x - 2 * orientationBuffer.z * orientationBuffer.z);
		//float roll = Mathf.Atan2(2 * orientationBuffer.y * orientationBuffer.w - 2 * orientationBuffer.x * orientationBuffer.z, 1 - 2 * orientationBuffer.y * orientationBuffer.y - 2 * orientationBuffer.z * orientationBuffer.z);
		//Vector3 eulerAngles = new Vector3(yaw, pitch, roll) * Mathf.Rad2Deg;
		//angleOut = eulerAngles;

		//angleOut = transform.forward;
		angleOut = new Vector3(yaw, pitch, 0);

		/*
		Vector3 eulerAngles = transform.eulerAngles;
		if (eulerAngles.z < 180.0f) {
			if (eulerAngles.x < 90.0f - MaxUpPitch ) {
				eulerAngles.x = 90.0f - MaxUpPitch;
			}
		} else {
			if (eulerAngles.x < 90.0f - MaxDownPitch) {
				eulerAngles.x = 90.0f - MaxDownPitch;
			}
		}
		//eulerAngles.x = Mathf.Clamp(eulerAngles.x, 15, 90);
		transform.eulerAngles = eulerAngles;
		// */

		/*
		if (forward.x < 0.01f) {
			.x = 5.0f;
		} else if (forward.x > 135.0f) {
			eulerAngles.x = 135.0f;
		}

		orientationBuffer.eulerAngles = eulerAngles;

		// */



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
		*/

	}

}
