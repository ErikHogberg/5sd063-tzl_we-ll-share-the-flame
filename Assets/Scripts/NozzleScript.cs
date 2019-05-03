using UnityEngine;
using UnityEngine.Experimental.Input;
using WiimoteApi;

public enum RotationMethod {
	World,
	Self,
	LocalRotation
}
public class NozzleScript : MonoBehaviour {

	[SerializeField]
	private InputActionAsset controls;

	public ParticleSystem Foam;

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


	// wiimote
	private Wiimote wiimote;
	private Vector3 wmpOffset = Vector3.zero;
	public RotationMethod RotationMethod = RotationMethod.LocalRotation;

	private Quaternion orientationBuffer = Quaternion.identity;

	void Start() {
		Foam.Pause();

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
				if (ret < 1) {
					continue;
				}

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

					if (wiimote.Button.a) {
                        // transform.localRotation = Quaternion.AngleAxis(90, transform.parent.right);
                        orientationBuffer = Quaternion.AngleAxis(90, transform.parent.right);
					}
                    if (wiimote.Button.d_down) {
						wmpOffset = Vector3.zero;
                    }

					switch (RotationMethod) {
						case RotationMethod.World:
							transform.Rotate(offset, Space.World);
							break;
						case RotationMethod.Self:
							transform.Rotate(offset, Space.Self);
							break;
						case RotationMethod.LocalRotation:
                            // transform.localRotation = Quaternion.Euler(offset);					
                            // transform.localRotation *= Quaternion.Euler(offset);
                            orientationBuffer *= Quaternion.Euler(offset);
                            transform.localRotation = Quaternion.identity
							* orientationBuffer 
							// * Quaternion.Euler(-wmpOffset)
							;

                            break;
					}

					// model.rot.Rotate(offset, Space.Self);
					// transform.Rotate(offset, Space.Self);
					// transform.localRotation = Quaternion.Euler(offset);					

				}
			} while (ret > 0);

			wiimoteFiring = wiimote.Button.b;
		}

		//if (Mouse.current.leftButton.isPressed) {
		if (firing || wiimoteFiring) {
			if (!wasFiring) {
				Foam.Play();
				wasFiring = true;
			}

			// TODO: limit angle
			// IDEA: add "buffer" rotation that is assigned current rotation, but is capped to certain angles
			/*
			transform.Rotate(
				Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime,
				Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime,
				0,
				Space.World
			);
			 */

		} else {
			if (wasFiring) {
				Foam.Stop();
				wasFiring = false;
			}
		}

		// TODO: change percentually between min and max values, make both speed and spread reach their limits at the same time.
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
	}

}
