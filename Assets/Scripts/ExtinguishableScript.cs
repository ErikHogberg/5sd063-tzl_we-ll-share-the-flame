using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ExtinguishableScript : MonoBehaviour {

	public float ScoreWorth = 10f;
	public RespawnExtinguishableScript Respawner;

	//Mick start
	public AudioClip ExtinguishSFX;
	public AudioSource AS_Extinguish;

	public void Start() {
		AS_Extinguish.clip = ExtinguishSFX;
	}
	//Mick end

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			Extinguish();
		}
	}

	void OnParticleCollision(GameObject other) {
		Extinguish();
	}

	public void Extinguish() {
		// Debug.Log("Extinguished " + name);
		//Globals.NotificationPanel.Notify("Extinguished " + name + " for " + ScoreWorth + " points!");
		Globals.NotificationPanel.Notify(ScoreWorth.ToString());

		// RespawnExtinguishableScript respawner = GetComponentInParent<RespawnExtinguishableScript>();
		if (Respawner != null) {
			Respawner.StartRespawnTimer();
		}

		//Mick start
		AS_Extinguish.Play();
		//Mick end
		Globals.AddScore(ScoreWorth, 0.1f);

		gameObject.SetActive(false);
	}

	// void OnParticleTrigger() {
	//     Debug.Log("particle trigger");

	// }

}
