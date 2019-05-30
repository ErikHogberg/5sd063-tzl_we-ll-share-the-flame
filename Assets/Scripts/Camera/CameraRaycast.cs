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


	void Update() {
		Vector3 heading = Camera.main.transform.position - transform.position;
		// Vector3 heading = MainCamera.position - transform.position;
		float distance = heading.magnitude;
		//Vector3 direction = heading / distance;
		if (DrawRay) {
			Debug.DrawRay(transform.position, heading, Color.green, 10f);
		}

		RaycastHit hit;
		Ray checkRay = new Ray(transform.position, heading);

		// FIXME: buildings with multiple colliders get "unhid" when another collider is raycasted while the building is already "hidden"
		// FIXME: buildings are not "unhid" if another building is hidden while the previous building is still hidden, because objectToHide is reassigned (make list of hidden objects instead?)

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
		Material material = objectToHide.GetComponent<MeshRenderer>().material;
		Color color = material.color;
		color.a = alpha;
		material.color = color;
	}
}
