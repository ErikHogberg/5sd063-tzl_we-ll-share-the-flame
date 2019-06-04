using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using Assets.Scripts.Utilities;
using UnityEngine;


public class IntroCameraScript : MonoBehaviour {

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

	private bool notInit = true;

	void Start() {
		timer = new Timer();
		SetCameraSpot();
	}

	void Update() {

		if (notInit) {

			Globals.CameraScript.enabled = false;
			Globals.Player.DisableMovement = true;

			notInit = false;
		}

		yaw += currentCameraSpot.Speed.x * Time.deltaTime;
		pitch += currentCameraSpot.Speed.y * Time.deltaTime;
		roll += currentCameraSpot.Speed.z * Time.deltaTime;
		zoom += currentCameraSpot.ZoomSpeed * Time.deltaTime;

		if (timer.Update()) {
			currentCameraSpotIndex++;
			if (currentCameraSpotIndex >= CameraSpots.Count) {
				// currentCameraSpotIndex = 0;
				Globals.CameraScript.enabled = true;
				Globals.Player.DisableMovement = false;
				this.enabled = false;
				if (Globals.CollectibleNotificationPanel != null) {
					Globals.CollectibleNotificationPanel.Notify("Leave the circle to start!");
				}
				return;
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
