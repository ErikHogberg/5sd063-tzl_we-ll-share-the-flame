using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TouchWheelControlScript : MonoBehaviour {

	private RectTransform rectTransform;

	public RectTransform Track;
	public RectTransform TrackTile;

	public bool LeftWheel = true;
	public float BrakeDeadzone = 0f;

	private float? lastTouchY = null;
	// private float speedBuffer = 0f;

	public float AnimationSpeed = 1f;

	void Start() {

		Input.multiTouchEnabled = true;

		rectTransform = GetComponent<RectTransform>();
		float rectHeight = rectTransform.rect.height;
		float rectwidth = rectTransform.rect.width;

		float tileHeight = TrackTile.rect.height;

		int tiling = Mathf.CeilToInt(rectHeight / tileHeight);

		float topTileCenter = rectHeight / 2f - tileHeight / 2f;

		Vector2 tileCenter = TrackTile.anchoredPosition;
		tileCenter.y = topTileCenter;
		TrackTile.anchoredPosition = tileCenter;

		Instantiate(
			TrackTile,
			Track.position + new Vector3(
				TrackTile.transform.localPosition.x,
				topTileCenter + tileHeight,
				TrackTile.transform.localPosition.z
			),
			TrackTile.transform.localRotation,
			Track
		);

		// Debug.Log("Tiling: " + tiling);
		for (int i = 1; i < tiling + 1; i++) {
			Instantiate(
				TrackTile,
				Track.position + new Vector3(
					TrackTile.transform.localPosition.x,
					topTileCenter - tileHeight * i,
					TrackTile.transform.localPosition.z
				),
				TrackTile.transform.localRotation,
				Track
			);
		}

	}

	void Update() {

		Globals.Player.LeftWheelSpeed = Mathf.MoveTowards(Globals.Player.LeftWheelSpeed, 0f, Globals.Player.Damping * Time.deltaTime);
		Globals.Player.RightWheelSpeed = Mathf.MoveTowards(Globals.Player.RightWheelSpeed, 0f, Globals.Player.Damping * Time.deltaTime);

		// input
		// FIXME: right wheel controls requires touching left wheel control to get touch

		if (lastTouchY.HasValue) {

			int id;
			if (LeftWheel) {
				id = Globals.LeftWheelFingerId;
			} else {
				id = Globals.RightWheelFingerId;
			}

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
						float delta = lastTouchY.Value - currentTouch.Value.position.y;
						if (delta < 0f) {
							// delta backwards
							if (LeftWheel) {
								if (Globals.Player.LeftWheelSpeed < 0f) {
									// speed backwards
									if (delta < Globals.Player.LeftWheelSpeed) {
										Globals.Player.LeftWheelSpeed = delta;
									}
								} else {
									// speed forwards
									if (-delta > BrakeDeadzone) {
										Globals.Player.LeftWheelSpeed = delta;
									}
								}
							} else {
								if (Globals.Player.RightWheelSpeed < 0f) {
									// speed backwards
									if (delta < Globals.Player.RightWheelSpeed) {
										Globals.Player.RightWheelSpeed = delta;
									}
								} else {
									// speed forwards
									if (-delta > BrakeDeadzone) {
										Globals.Player.RightWheelSpeed = delta;
									}
								}
							}

						} else {
							// delta forwards
							if (LeftWheel) {
								if (Globals.Player.LeftWheelSpeed < 0f) {
									// speed backwards
									if (delta > BrakeDeadzone) {
										Globals.Player.LeftWheelSpeed = delta;
									}
								} else {
									// speed forwards
									if (delta > Globals.Player.LeftWheelSpeed) {
										Globals.Player.LeftWheelSpeed = delta;
									}

								}
							} else {
								if (Globals.Player.RightWheelSpeed < 0f) {
									// speed backwards
									if (delta > BrakeDeadzone) {
										Globals.Player.RightWheelSpeed = delta;
									}
								} else {
									// speed forwards
									if (delta > Globals.Player.RightWheelSpeed) {
										Globals.Player.RightWheelSpeed = delta;
									}
								}
							}
						}
						lastTouchY = currentTouch.Value.position.y;
						break;
					case TouchPhase.Canceled:
					case TouchPhase.Ended:
						if (LeftWheel)
							Globals.LeftWheelFingerId = -1;
						else
							Globals.RightWheelFingerId = -1;

						lastTouchY = null;
						break;
				}
			}

		} else {
			for (int i = 0; i < Input.touchCount; i++) {
				Touch currentTouch = Input.GetTouch(i);

				// skip if used by other wheel
				if (LeftWheel) {
					if (currentTouch.fingerId == Globals.RightWheelFingerId)
						continue;
				} else {
					if (currentTouch.fingerId == Globals.LeftWheelFingerId)
						continue;
				}

				switch (currentTouch.phase) {
					case TouchPhase.Began:
						if (RectTransformUtility.RectangleContainsScreenPoint(rectTransform, currentTouch.position)) {
							lastTouchY = currentTouch.position.y;
							// touch = currentTouch;
							if (LeftWheel) {
								Globals.LeftWheelFingerId = currentTouch.fingerId;
							} else {
								Globals.RightWheelFingerId = currentTouch.fingerId;
							}
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

		{
			Vector2 pos = Track.anchoredPosition;
			if (LeftWheel) {
				pos.y -= Globals.Player.LeftWheelSpeed * AnimationSpeed * Time.deltaTime;
			} else {
				pos.y -= Globals.Player.RightWheelSpeed * AnimationSpeed * Time.deltaTime;
			}
			Track.anchoredPosition = pos;
		}

		// tiling
		float tileHeight = TrackTile.rect.height;

		while (Track.anchoredPosition.y > tileHeight) {
			Vector2 pos = Track.anchoredPosition;
			pos.y -= tileHeight;
			Track.anchoredPosition = pos;
		}

		while (Track.anchoredPosition.y < -tileHeight) {
			Vector2 pos = Track.anchoredPosition;
			pos.y += tileHeight;
			Track.anchoredPosition = pos;
		}


	}

	void OnGUI() {
		for (int i = 0; i < Input.touchCount; i++) {
			Touch currentTouch = Input.GetTouch(i);
			float boxHalfSize = 300f;
			GUI.color = Color.blue;
			GUI.Box(new Rect(
				currentTouch.position.x,
				-(currentTouch.position.y),
				boxHalfSize,
				boxHalfSize
				), "touch: " + i + ", id: " + currentTouch.fingerId);
		}
	}

}
