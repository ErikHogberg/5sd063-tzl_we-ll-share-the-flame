using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;


public enum CollectibleType {
	Cat,
	Graffiti,
	BigBlueElemental,
}

public class CollectableScript : MonoBehaviour {

	public CollectibleType Type;

	private bool notInit = true;

	void Start() {

	}

	private void Update() {
		if (notInit) {

			if (Globals.CollectiblePanel != null) {
				Globals.CollectiblePanel.AddTotalCollectibles(Type);
			}

			notInit = false;
		}
	}

	private void OnDisable() {
		if (Globals.TimerPanel != null && Globals.TimerPanel.IsRunning()) {
			if (Globals.CollectiblePanel != null) {
				Globals.CollectiblePanel.AddCollectibles(Type);
			}
		}
	}

}
