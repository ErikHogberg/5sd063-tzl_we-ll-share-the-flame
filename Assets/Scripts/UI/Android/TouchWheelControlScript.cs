using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class TouchWheelControlScript : MonoBehaviour {

	public RectTransform Track;
	public RectTransform TrackTile;

	public bool LeftWheel = true;

	void Start() {
		RectTransform rectTransform = GetComponent<RectTransform>();
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

		// input
		// TODO: save current wheel finger id global
		// TODO: check if finger id is used by other wheel

		Touch? touch = null;// = Input.GetTouch(0);

		if (LeftWheel) {
			for (int i = 0; i < Input.touchCount; i++) {
				Touch currentTouch = Input.GetTouch(i);
				if (currentTouch.fingerId != Globals.RightWheelFingerId) {
					touch = currentTouch;
					break;
				}
			}
		} else {
			for (int i = 0; i < Input.touchCount; i++) {
				Touch currentTouch = Input.GetTouch(i);
				if (currentTouch.fingerId != Globals.LeftWheelFingerId) {
					touch = currentTouch;
					break;
				}
			}
		}

		if (!touch.HasValue) {
			return;
		}
		
		int id = touch.Value.fingerId;

		for (int i = 0; i < Input.touchCount; i++) {
			if (Input.GetTouch(i).fingerId == id) {

			}
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

}
