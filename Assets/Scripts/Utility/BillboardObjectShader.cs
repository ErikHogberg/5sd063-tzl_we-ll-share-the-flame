using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardObjectShader : MonoBehaviour {

	// public bool BillboardX = true;
	// public bool BillboardY = true;
	// public bool BillboardZ = true;

	void Update() {
        transform.rotation = Camera.main.transform.rotation;
	}
}
