using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtinguishableScript : MonoBehaviour
{

    void OnParticleCollision(GameObject other) {
        Debug.Log("particle collision");
        gameObject.SetActive(false);
    }

    // void OnParticleTrigger() {
    //     Debug.Log("particle trigger");

    // }
	
}
