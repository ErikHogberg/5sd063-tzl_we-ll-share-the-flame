using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class VerticalTilingScript : MonoBehaviour {

	private RectTransform rectTransform;

	public RectTransform Track;
	public RectTransform TrackTile;

	// Start is called before the first frame update
	void Start() {
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

	// Update is called once per frame
	void LateUpdate() {
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
