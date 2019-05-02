﻿using UnityEngine;
using UnityEngine.Experimental.Input;
using WiimoteApi;

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

		bool wiimoteFiring = false;

        if (!WiimoteManager.HasWiimote()) { WiimoteManager.FindWiimotes(); } else {
			wiimote = WiimoteManager.Wiimotes[0];
			
			int ret;
			do {
				ret = wiimote.ReadWiimoteData();

				if (ret > 0 && wiimote.current_ext == ExtensionController.MOTIONPLUS)
				{
					Vector3 offset = new Vector3(-wiimote.MotionPlus.PitchSpeed,
													wiimote.MotionPlus.YawSpeed,
													wiimote.MotionPlus.RollSpeed) / 95f; // Divide by 95Hz (average updates per second from wiimote)
					// wmpOffset += offset;

					// model.rot.Rotate(offset, Space.Self);
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
            transform.Rotate(
				Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime,
				Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime,
				0,
				Space.World
			);

		} else {
            if (wasFiring) {
				Foam.Stop();
                wasFiring = false;
            }
		}

		// TODO: change percentually between min and max values, make both speed and spread reach their limits at the same time.
		if(Input.GetKey(KeyCode.L)) {
			if(Foam.startSpeed > MinFoamSpeed) {
				Foam.startSpeed -= FoamSwitchSpeed;
			}

			// if(Foam.startSize < 3) {
			// 	Foam.startSize += 0.1f;
			// }

			ParticleSystem.ShapeModule shape = Foam.shape;

            if (shape.angle < MaxFoamSpread) {
                shape.angle += FoamSwitchSpeed;
            }

        } else if (Input.GetKey(KeyCode.K)) {
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
