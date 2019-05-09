using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
	public Transform MainCamera;
	private GameObject objectToHide = null;

	void Update() {
		Vector3 heading = MainCamera.position - transform.position;
		float distance = heading.magnitude;
		Vector3 direction = heading / distance;

		Ray ray = new Ray(transform.position, direction);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)) {
			Debug.DrawRay(transform.position, direction, Color.green, 10f);
			Debug.Log(hit.collider.name);
			if(hit.collider.CompareTag("Building")) {
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
		}
	}
}
