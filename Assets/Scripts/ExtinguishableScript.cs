﻿using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ExtinguishableScript : MonoBehaviour {

	public float ScoreWorth = 10f;
	public RespawnExtinguishableScript Respawner;

	void OnParticleCollision(GameObject other) {
		// Debug.Log("Extinguished " + name);
		Globals.NotificationPanel.Notify("Extinguished " + name + " for " + ScoreWorth + " points!");

		// RespawnExtinguishableScript respawner = GetComponentInParent<RespawnExtinguishableScript>();
		if (Respawner != null) {
			Respawner.StartRespawnTimer();
		}

		Globals.AddScore(ScoreWorth, 0.1f);

		gameObject.SetActive(false);
	}

	// void OnParticleTrigger() {
	//     Debug.Log("particle trigger");

	// }

}
