using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorUpdateOrderScript : MonoBehaviour {

	public bool CalledManually = false;
	public List<AnchorScript> AnchorScripts;

	void Update() {
		if (!CalledManually) {
			UpdateAnchors();
		}
	}

	public void UpdateAnchors() {
		foreach (AnchorScript anchorScript in AnchorScripts) {
			anchorScript.UpdateAnchor();
		}
	}
}
