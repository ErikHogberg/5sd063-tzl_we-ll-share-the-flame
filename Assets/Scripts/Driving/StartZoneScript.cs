using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;

public class StartZoneScript : MonoBehaviour
{
    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player"))
        {
            Globals.TimerPanel.StartCountdown();
            gameObject.SetActive(false);
        }
    }
}
