using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class AmmoCounterWidgetScript : MonoBehaviour {

	private float startTopY;

	private RectTransform dims;

	public float amount = 100;

	void Start() {
		dims = GetComponent<RectTransform>();
		startTopY = dims.anchorMax.y;
	}

	void Update() {

		Vector3 scale = transform.localScale;
		scale.y = Globals.Nozzle.AmmoAmount / Globals.Nozzle.AmmoCapacity;
		transform.localScale = scale;

	}
}
