using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class CatSavingScript : MonoBehaviour {

	public int ScoreWorth = 10;

	void OnParticleCollision(GameObject other) {
		// Debug.Log("Extinguished " + name);
		Globals.NotificationPanel.Notify("You saved a Cat! " + ScoreWorth + " points!");

		Globals.Score += ScoreWorth;

		gameObject.SetActive(false);
	}

}
