using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TouchAimControlScript : MonoBehaviour {

	private RectTransform rectTransform;

	public VerticalTilingScript VerticalTrack;
	public HorizontalTilingScript HorizontalTrack;
	public Vector2 AnimationSpeed = new Vector2(1f, 1f);

	private Vector2? lastTouch = null;
	// private float speedBuffer = 0f;

	public Vector2 YawPitchSpeed = new Vector2(1f, 1f);

	void Start() {

		rectTransform = GetComponent<RectTransform>();

	}

	void Update() {

		Globals.Player.LeftWheelSpeed = Mathf.MoveTowards(Globals.Player.LeftWheelSpeed, 0f, Globals.Player.Damping * Time.deltaTime);
		Globals.Player.RightWheelSpeed = Mathf.MoveTowards(Globals.Player.RightWheelSpeed, 0f, Globals.Player.Damping * Time.deltaTime);

		// input

		// IDEA: for coop shooter only mode: alternative to gyro; drag to aim, press second finger to shoot, can only shoot if aiming with another finger

		if (lastTouch.HasValue) {

			int id = Globals.AimFingerId;

			Touch? currentTouch = null;
			for (int i = 0; i < Input.touchCount; i++) {
				if (Input.GetTouch(i).fingerId == id) {
					currentTouch = Input.GetTouch(i);
				}
			}

			if (currentTouch.HasValue) {
				switch (currentTouch.Value.phase) {
					case TouchPhase.Began:
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						Vector2 delta = (currentTouch.Value.position - lastTouch.Value) * YawPitchSpeed;

						Globals.Nozzle.AddYawPitch(delta.x, delta.y);

						HorizontalTrack.MoveTrack(delta.x * AnimationSpeed.x);
						VerticalTrack.MoveTrack(delta.y * AnimationSpeed.y);

						lastTouch = currentTouch.Value.position;
						break;
					case TouchPhase.Canceled:
					case TouchPhase.Ended:
						Globals.AimFingerId = -1;
						lastTouch = null;
						break;
				}
			}

		} else {
			for (int i = 0; i < Input.touchCount; i++) {
				Touch currentTouch = Input.GetTouch(i);

				// skip if used by wheels
				if (currentTouch.fingerId == Globals.LeftWheelFingerId
				 || currentTouch.fingerId == Globals.RightWheelFingerId)
					continue;

				switch (currentTouch.phase) {
					case TouchPhase.Began:
						if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, currentTouch.position)) {
							lastTouch = currentTouch.position;
							// touch = currentTouch;
							Globals.AimFingerId = currentTouch.fingerId;
							break;
						}
						continue;
					case TouchPhase.Canceled:
					case TouchPhase.Ended:
					case TouchPhase.Moved:
					case TouchPhase.Stationary:
						continue;
				}

				break;
			}

		}

	}
}
