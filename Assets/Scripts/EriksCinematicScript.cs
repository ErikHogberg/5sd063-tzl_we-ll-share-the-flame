using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CameraSpot {
	public GameObject AnchorObject;
	public float StartYaw;
	public float StartPitch;
	public float Speed;
	public float Time;
}

public class EriksCinematicScript : MonoBehaviour {

	// [SerializeField]
	public List<CameraSpot> CameraSpots;

	void Start() {		

	}

	void Update() {

	}
}
