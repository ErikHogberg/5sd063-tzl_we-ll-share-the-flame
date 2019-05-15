using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class ExtinguishableScript : MonoBehaviour
{

    public int ScoreWorth = 10;

    void OnParticleCollision(GameObject other) {
        Debug.Log("particle collision");

        Globals.Score += ScoreWorth;

        gameObject.SetActive(false);
    }

    // void OnParticleTrigger() {
    //     Debug.Log("particle trigger");

    // }
	
}
