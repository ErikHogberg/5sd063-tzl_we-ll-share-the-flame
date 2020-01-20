using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour {
	// public Transform MainCamera;
	private GameObject objectToHide = null;

	[Tooltip("Alpha percentage for when a building is \"hidden\"")]
	public float Alpha = .2f;
	[Tooltip("Wether or not to render the raycast in the editor view")]
	public bool DrawRay = false;

	private Camera mainCamera;

	private void Start() {
		mainCamera = Camera.main;
	}

	void Update() {
		Vector3 heading = mainCamera.transform.position - transform.position;
		// Vector3 heading = MainCamera.position - transform.position;
		float distance = heading.magnitude;
		//Vector3 direction = heading / distance;
		if (DrawRay) {
			Debug.DrawRay(transform.position, heading, Color.green, 10f);
		}

		RaycastHit hit;
		Ray checkRay = new Ray(transform.position, heading);

		// FIXME: only one building can be hid at a time, because objectToHide is reassigned (make list of hidden objects instead?)

		if (Physics.Raycast(checkRay, out hit, distance)) {
			if (hit.collider.CompareTag("Building")) {
				if (objectToHide != null) {
					// objectToHide.GetComponent<MeshRenderer>().enabled = true;
					SetAlpha(1f);
					objectToHide = null;
				}
				objectToHide = hit.collider.gameObject;
				// objectToHide.GetComponent<MeshRenderer>().enabled = false;
				SetAlpha(Alpha);
			} else {
				if (objectToHide != null) {
					// objectToHide.GetComponent<MeshRenderer>().enabled = true;
					SetAlpha(1f);
					objectToHide = null;
				}
			}
		} else {
			if (objectToHide != null) {
				// objectToHide.GetComponent<MeshRenderer>().enabled = true;
				SetAlpha(1f);
				objectToHide = null;
			}
		}
	}

	private void SetAlpha(float alpha) {

		BuildingChangeMaterialScript materialScript = objectToHide.GetComponent<BuildingChangeMaterialScript>();

		if (materialScript != null) {

			if (alpha < 1f) {
				materialScript.SwitchToSecondMaterial();
				Material material = objectToHide.GetComponent<MeshRenderer>().material;

				Color color = material.color;
				color.a = alpha;
				material.color = color;
			} else {
				materialScript.SwitchToFirstMaterial();
			}
		} else {
			if (alpha < 1f) {
				objectToHide.GetComponent<MeshRenderer>().enabled = false;
			} else {
				objectToHide.GetComponent<MeshRenderer>().enabled = true;
			}
		}
	}
}
