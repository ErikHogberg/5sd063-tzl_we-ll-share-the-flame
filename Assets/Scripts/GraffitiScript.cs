using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class GraffitiScript : MonoBehaviour {

	public float removalSpeed = 0.01f;
	private float alpha = 1f;

	public float ScoreWorth = 10f;
	public float ScoreMultiplierIncrease = 0.1f;

	// private MeshRenderer renderer;
	private Material material;

	void Start() {
		material = this.GetComponent<MeshRenderer>().material;
	}

	void OnParticleCollision(GameObject other) {

		alpha -= removalSpeed;

		// TODO: change object alpha without affecting all objects with same texture
		Color color = material.color;
		color.a = alpha;
		material.color = color;

		if (alpha < 0f) {
			Globals.AddScore(ScoreWorth, ScoreMultiplierIncrease);
			transform.parent.gameObject.SetActive(false);
		}
	}

}
