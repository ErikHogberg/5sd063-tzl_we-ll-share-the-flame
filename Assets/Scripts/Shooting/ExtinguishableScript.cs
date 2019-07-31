using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ExtinguishableScript : MonoBehaviour {

	public float ScoreWorth = 10f;
	public float ScoreMultiplierIncrease = 0.1f;

	public RespawnExtinguishableScript Respawner;

	//Mick start
	public AudioClip ExtinguishSFX;
	public AudioSource AS_Extinguish;

	// IDEA: network: have each extinguishable register itself to a list with a unique index that it keeps track of and a bool "is extinguished" value,
	// on extinguish it sends its index to the other player, on receive the slot of the received index in the list is marked as extinguished, 
	// on extinguisher update it checks its slot if has been marked as extinguished, at which point it extinguishes itself.

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
		// Globals.NotificationPanel.Notify(ScoreWorth.ToString());

		// RespawnExtinguishableScript respawner = GetComponentInParent<RespawnExtinguishableScript>();
		if (Respawner != null) {
			Respawner.StartRespawnTimer();
		}

		//Mick start
		AS_Extinguish.Play();
		//Mick end
		Globals.AddScore(ScoreWorth, ScoreMultiplierIncrease);

		gameObject.SetActive(false);
	}

	// void OnParticleTrigger() {
	//     Debug.Log("particle trigger");

	// }

}
