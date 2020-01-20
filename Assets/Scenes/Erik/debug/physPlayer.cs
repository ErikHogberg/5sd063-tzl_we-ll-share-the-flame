using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(Rigidbody))]
public class physPlayer : MonoBehaviour {
	private Rigidbody rb;

	public float TurnSpeed;
	public float MoveSpeed;

	void Start() {
		rb = GetComponent<Rigidbody>();

	}

	void FixedUpdate() {
		if (Input.GetKey(KeyCode.A)) {
			rb.rotation *= Quaternion.AngleAxis(TurnSpeed * Time.deltaTime, transform.up);
            rb.angularVelocity = Vector3.zero;
		} else if (Input.GetKey(KeyCode.D)) {
			rb.rotation *= Quaternion.AngleAxis(-TurnSpeed * Time.deltaTime, transform.up);
            rb.angularVelocity = Vector3.zero;
		}


		if (Input.GetKey(KeyCode.W)) {
			rb.velocity = rb.rotation * Vector3.forward * MoveSpeed * Time.deltaTime;
		}
	}
}
