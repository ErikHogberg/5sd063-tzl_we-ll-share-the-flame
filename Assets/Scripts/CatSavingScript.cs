using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class CatSavingScript : MonoBehaviour {

	public float ScoreWorth = 10f;
	public float MultiplierIncrease = 2f;

	void OnParticleCollision(GameObject other) {
		// Debug.Log("Extinguished " + name);
		Globals.NotificationPanel.Notify("You saved a Cat! " + ScoreWorth + " points!");

		Globals.AddScore(ScoreWorth, MultiplierIncrease);

		gameObject.SetActive(false);
	}

}
