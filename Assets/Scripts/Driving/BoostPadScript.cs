using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class BoostPadScript : MonoBehaviour {

	public float BoostTime = 1f;

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {
			// TODO: option to set rotation to ramp facing
			// TODO: set boost time
			Globals.Player.Boost(BoostTime, transform.rotation);
		}
	}

}
