using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;

[System.Serializable]
public class CameraSpot {
	public GameObject AnchorObject;
	public Vector3 StartOrientation;
	public float StartZoom;
	public Vector3 Speed;
	public float ZoomSpeed;
	public float Time;

	// public AnimationCurve MovementCurve;
}

public class EriksCinematicScript : MonoBehaviour {

	public List<CameraSpot> CameraSpots;
	private int currentCameraSpotIndex = 0;

	private CameraSpot currentCameraSpot {
		get { return CameraSpots[currentCameraSpotIndex]; }
	}

	private Timer timer;

	private float yaw;
	private float pitch;
	private float roll;
	private float zoom;


	void Start() {
		timer = new Timer();
		SetCameraSpot();
	}

	void Update() {

		yaw += currentCameraSpot.Speed.x * Time.deltaTime;
		pitch += currentCameraSpot.Speed.y * Time.deltaTime;
		roll += currentCameraSpot.Speed.z * Time.deltaTime;
		zoom += currentCameraSpot.ZoomSpeed * Time.deltaTime;

		if (timer.Update()) {
			currentCameraSpotIndex++;
			if (currentCameraSpotIndex >= CameraSpots.Count) {
				currentCameraSpotIndex = 0;
			}
			SetCameraSpot();
		}

		UpdateOffset();

	}

	private void SetCameraSpot() {
		timer.Restart(currentCameraSpot.Time);
		yaw = currentCameraSpot.StartOrientation.x;
		pitch = currentCameraSpot.StartOrientation.y;
		roll = currentCameraSpot.StartOrientation.z;
		zoom = currentCameraSpot.StartZoom;
	}

	private void UpdateOffset() {
		Vector3 offset = Quaternion.identity
			* Quaternion.AngleAxis(yaw, Vector3.up)
			* Quaternion.AngleAxis(pitch, Vector3.left)
			* Quaternion.AngleAxis(roll, Vector3.forward)
			 * Vector3.forward * zoom
		 ;

		transform.position = currentCameraSpot.AnchorObject.transform.position + offset;
		transform.LookAt(currentCameraSpot.AnchorObject.transform.position);
	}

}
