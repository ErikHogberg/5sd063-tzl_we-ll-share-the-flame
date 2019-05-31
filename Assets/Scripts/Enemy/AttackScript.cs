using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
	private Animator animator;

	void Start() {
		animator = GetComponent<Animator>();
	}

    void OnTriggerEnter(Collider collider) {
		if(collider.CompareTag("Player")) {
			animator.SetBool("Attack", true);
		} else {
			animator.SetBool("Attack", false);
		}
	}

	void OnTriggerExit(Collider collider) {
		animator.SetBool("Attack", false);
	}
}
