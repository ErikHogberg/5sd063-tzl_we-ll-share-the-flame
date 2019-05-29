using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BoostAmmoCounterWidgetScript : MonoBehaviour {

	private float startTopY;
	private RectTransform dims;

	private float startThresholdTopY;
	public RectTransform ThresholdSquare;

	// public float amount = 1f;

	void Start() {
		dims = GetComponent<RectTransform>();
		startTopY = dims.anchorMax.y;

		startThresholdTopY = ThresholdSquare.anchorMax.y;
	}

	void Update() {

		Vector3 scale = transform.localScale;
		scale.y = Globals.Player.BoostAmmo / 1f;
		transform.localScale = scale;

		scale = ThresholdSquare.transform.localScale;
		scale.y = Globals.Player.BoostAmmoDisableTreshold / 1f;
		ThresholdSquare.transform.localScale = scale;

	}
}
