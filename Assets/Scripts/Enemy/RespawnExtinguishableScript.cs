using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;

public class RespawnExtinguishableScript : MonoBehaviour {

	public float Time = 10f;
	private Timer timer;

	public GameObject ObjectToRespawn;

	void Start() {
		timer = new Timer(Time);
		timer.Stop();
	}

	void Update() {
		if (timer.Update()) {
			// GetComponentInChildren<ExtinguishableScript>().gameObject.SetActive(true);
			ObjectToRespawn.SetActive(true);
		}
	}

	public void StartRespawnTimer() {
		timer.Restart();
	}
}
