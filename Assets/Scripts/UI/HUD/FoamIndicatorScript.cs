using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class FoamIndicatorScript : MonoBehaviour {

	public GameObject Foam;
	public GameObject Water;

	private bool useWaterBuffer;
	private bool notYetInitialized = true;

	void Start() {
	}

	void Update() {
		if (notYetInitialized) {
			useWaterBuffer = !Globals.Nozzle.particleModeUseWater;
		}
		bool useWater = Globals.Nozzle.particleModeUseWater;

		if (useWater) {
			if (!useWaterBuffer) {
				Water.SetActive(true);
				Foam.SetActive(false);
			}
		} else {
			if (useWaterBuffer) {
				Foam.SetActive(true);
				Water.SetActive(false);
			}
		}
		useWaterBuffer = useWater;
	}

}
