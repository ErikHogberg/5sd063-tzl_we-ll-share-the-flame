using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingChangeMaterialScript : MonoBehaviour {

	private Material FirstMaterial;
	public Material SecondMaterial;

	void Start() {
		FirstMaterial = GetComponent<MeshRenderer>().material;
	}

	public void SwitchToFirstMaterial() {
		GetComponent<MeshRenderer>().material = FirstMaterial;
	}

	public void SwitchToSecondMaterial() {
		GetComponent<MeshRenderer>().material = SecondMaterial;
	}

	public void ToggleMaterial() {
		Material currentMaterial = GetComponent<MeshRenderer>().material;
		if (currentMaterial == FirstMaterial) {
			SwitchToSecondMaterial();
		} else {
			SwitchToFirstMaterial();
		}
	}


}
