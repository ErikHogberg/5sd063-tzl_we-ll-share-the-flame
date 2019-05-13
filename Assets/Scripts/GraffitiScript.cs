using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GraffitiScript : MonoBehaviour {

	public float removalSpeed = 0.01f;
	public float alpha = 1f;

	

	void Start() {

	}

	void OnParticleCollision(GameObject other) {
		alpha -= removalSpeed;
		// TODO: set alpha
		// gameObject.SetActive(false);
	}

	// void OnParticleTrigger() {
	//     Debug.Log("particle trigger");

	// }

}
