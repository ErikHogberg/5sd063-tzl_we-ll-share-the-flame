using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
	// public Transform MainCamera;
	private GameObject objectToHide = null;

	void Update() {
		Vector3 heading = Camera.main.transform.position - transform.position;
		// Vector3 heading = MainCamera.position - transform.position;
		float distance = heading.magnitude;
		Vector3 direction = heading / distance;

		RaycastHit hit;

		if(Physics.Raycast(transform.position, direction, out hit, distance)) {
			if(hit.collider.CompareTag("Building")) {
				Debug.DrawRay(transform.position, direction, Color.green, 10f);
				if(objectToHide != null) {
					objectToHide.GetComponent<MeshRenderer>().enabled = true;
					objectToHide = null;
				}
				objectToHide = hit.collider.gameObject;
				objectToHide.GetComponent<MeshRenderer>().enabled = false;
			} else {
				if(objectToHide != null) {
					objectToHide.GetComponent<MeshRenderer>().enabled = true;
					objectToHide = null;
				}
			}
		} else {
			if(objectToHide != null) {
				objectToHide.GetComponent<MeshRenderer>().enabled = true;
				objectToHide = null;
			}
		}
	}
}
