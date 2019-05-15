using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ExtinguishableScript : MonoBehaviour
{

    public int ScoreWorth = 10;

    void OnParticleCollision(GameObject other) {
        // Debug.Log("Extinguished " + name);
		Globals.NotificationPanel.Notify("Extinguished " + name + " for " + ScoreWorth + " points!");

        Globals.Score += ScoreWorth;

        gameObject.SetActive(false);
    }

    // void OnParticleTrigger() {
    //     Debug.Log("particle trigger");

    // }
	
}
