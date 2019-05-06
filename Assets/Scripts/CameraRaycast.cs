﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraRaycast : MonoBehaviour
{
	public Transform player;
	private GameObject objectToHide = null;

	void Update() {
		Vector3 heading = player.position - transform.position;
		float distance = heading.magnitude;
		Vector3 direction = heading / distance;

		Ray ray = new Ray(transform.position, direction);
		RaycastHit hit;

		if(Physics.Raycast(ray, out hit)) {
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