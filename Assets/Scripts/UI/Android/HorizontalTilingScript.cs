using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class HorizontalTilingScript : MonoBehaviour {

	private RectTransform rectTransform;

	public RectTransform Track;
	public RectTransform TrackTile;

	void Start() {
		rectTransform = GetComponent<RectTransform>();
		// float rectHeight = rectTransform.rect.height;
		float rectWidth = rectTransform.rect.width;

		float tileWidth = TrackTile.rect.width;

		int tiling = Mathf.CeilToInt(rectWidth / tileWidth);

		float rightTileCenter = rectWidth / 2f - tileWidth / 2f;

		Vector2 tileCenter = TrackTile.anchoredPosition;
		tileCenter.x = rightTileCenter;
		TrackTile.anchoredPosition = tileCenter;

		Instantiate(
			TrackTile,
			Track.position + new Vector3(
				rightTileCenter + tileWidth,
				TrackTile.transform.localPosition.y,
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
					rightTileCenter - tileWidth * i,
					TrackTile.transform.localPosition.y,
					TrackTile.transform.localPosition.z
				),
				TrackTile.transform.localRotation,
				Track
			);
		}
	}

	public void UpdateTrack() {
		// tiling
		float tileWidth = TrackTile.rect.width;

		while (Track.anchoredPosition.x > tileWidth) {
			Vector2 pos = Track.anchoredPosition;
			pos.x -= tileWidth;
			Track.anchoredPosition = pos;
		}

		while (Track.anchoredPosition.x < -tileWidth) {
			Vector2 pos = Track.anchoredPosition;
			pos.x += tileWidth;
			Track.anchoredPosition = pos;
		}
	}

	public void MoveTrack(float amount) {
		Vector2 pos = Track.anchoredPosition;
		pos.x += amount;
		Track.anchoredPosition = pos;
		UpdateTrack();
	}
}
