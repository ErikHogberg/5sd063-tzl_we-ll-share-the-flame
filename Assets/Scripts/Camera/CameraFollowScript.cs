using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
// using UnityEngine.Experimental.Input;
using UnityEngine.UI;

public class CameraFollowScript : MonoBehaviour {

	public GameObject Camera;
	public AnchorUpdateOrderScript AnchorUpdater;

	// [SerializeField]
	public InputActionAsset controls;

	public float Yaw = 0.0f;
	public float Pitch = 30.0f;
	public float Zoom = 10.0f;


	public float MouseTurnSpeedX = 4.0f;
	public float MouseTurnSpeedY = 2.0f;
	public float scrollSpeed = 1.1f;
	public Transform PositionAnchor;
	public Transform AngleAnchor;

	// public WheelchairMoveScript WheelchairScript;

	private Vector3 distanceOffset;

	public float YawOffset = 0f;
	public float PitchOffset = 0f;
	private float yawAimOffset = 1f;
	private float pitchAimOffset = 1f;
	public float YawAimOffsetScale = 1f;
	public float PitchAimOffsetScale = 1f;

	public bool AutoTurning = true;
	public float CameraTurnSpeed = 1.0f;
	public float CameraTurnSpeedScale = 1.0f;
	public float CameraTurnDeadZone = 1.0f;

	public Text InfoPane;

	void Start() {

		Globals.CameraScript = this;

		Vector3 distance = transform.position - PositionAnchor.transform.position;
		distanceOffset = distance;

		// InputAction action = controls.TryGetActionMap("shooter").TryGetAction("shoot");

		{
			InputAction action = controls.TryGetActionMap("debug").TryGetAction("zoom");
			action.performed += _ => {

				Debug.Log("zoom");
			};
			action.Enable();
		}

		//{
		//	// InputAction action = controls.TryGetActionMap("shooter").TryGetAction("shoot");
		//	// FIXME: debug action map not triggering
		//	InputAction action = controls.TryGetActionMap("debug").TryGetAction("zoom out");
		//	action.performed += _ => {
		//		Debug.Log("zoom out");
		//	};
		//	action.Enable();
		//}

	}

	void LateUpdate() {

		string infoText = "";

		if (AnchorUpdater != null) {
			AnchorUpdater.UpdateAnchors();
		}

		if (Keyboard.current.rightShiftKey.isPressed) {

			//Mouse.current.position.x.ReadValue()

			//offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeedX, Vector3.up) * offset;
			// offset = Quaternion.AngleAxis(Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime, Vector3.up) * offset;
			Yaw += Mouse.current.delta.x.ReadValue() * MouseTurnSpeedX * Time.deltaTime;

			//offset = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeedY, transform.right) * offset;
			// offset = Quaternion.AngleAxis(Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime, transform.right) * offset;
			Pitch += Mouse.current.delta.y.ReadValue() * MouseTurnSpeedY * Time.deltaTime;
			//offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up);
		}

		// if ((Keyboard.current.iKey.isPressed) && Vector3.Magnitude(offset) > 0.1f) {
		if ((Keyboard.current.iKey.isPressed) && Zoom > 0.1f) {
			// offset *= 1.0f - scrollSpeed * Time.deltaTime;
			Zoom *= 1.0f - scrollSpeed * Time.deltaTime;
		}
		if (Keyboard.current.oKey.isPressed) {
			// offset *= 1.0f + scrollSpeed * Time.deltaTime;
			Zoom *= 1.0f + scrollSpeed * Time.deltaTime;
		}

		// float scroll = Mouse.current. .mouseScrollDelta.x;

		// if ( (scroll > 0f && Vector3.Magnitude(offset) > 0.1f) ||scroll < 0f) {
		// 	Debug.Log("scroll");
		//     offset *= 1.0f + (scrollSpeed * scroll * Time.deltaTime);
		// } 

		// Vector2 scroll = Mouse.current.scroll.ReadValue();
		// offset *= 1.0f + scroll.y;

		// Turning camera
		if (AutoTurning) {

			float wheelchairSpeed = 1f;
			// wheelchairSpeed = Globals.Player.Speed;
			wheelchairSpeed = Mathf.Abs(Globals.Player.leftWheelSpeed + Globals.Player.rightWheelSpeed);

			// TODO: quickly move the camera behind the player when they face the camera

			Vector2 cameraFacing = new Vector2(transform.forward.x, transform.forward.z);
			Vector2 playerFacing = new Vector2(AngleAnchor.transform.forward.x, AngleAnchor.transform.forward.z);

			// Camera.transform.rotation.ToAngleAxis(out float cameraAngle, out Vector3 cameraAxis);
			// transform.rotation.ToAngleAxis(out float playerAngle, out Vector3 playerAxis);

			float angleDelta = Vector2.SignedAngle(cameraFacing, playerFacing);
			float turnSpeed = CameraTurnSpeed + CameraTurnSpeedScale * wheelchairSpeed;
			float tempSpeed = turnSpeed * Time.deltaTime;
			infoText += "speed: " + tempSpeed + "\n";
			infoText += "Yaw: " + Yaw + "\n";
			infoText += "Pitch: " + Pitch + "\n";
			infoText += "delta: " + angleDelta + "\n";

			float playerAngle = Vector2.SignedAngle(Vector2.up, playerFacing);
			infoText += "player angle: " + playerAngle + "\n";


			if (angleDelta < -CameraTurnDeadZone) {
				Turn(tempSpeed);

				cameraFacing = new Vector2(transform.forward.x, transform.forward.z);
				playerFacing = new Vector2(AngleAnchor.transform.forward.x, AngleAnchor.transform.forward.z);

				if (Vector2.SignedAngle(cameraFacing, playerFacing) + YawOffset + Mathf.Abs(yawAimOffset) > CameraTurnDeadZone) {
					Yaw = -CameraTurnDeadZone - 180f - playerAngle;
					// Yaw = CameraTurnDeadZone + 180f;
					// UpdateOffset();
				}
			} else if (angleDelta > CameraTurnDeadZone) {
				Turn(-tempSpeed);

				cameraFacing = new Vector2(transform.forward.x, transform.forward.z);
				playerFacing = new Vector2(AngleAnchor.transform.forward.x, AngleAnchor.transform.forward.z);

				if (Vector2.SignedAngle(cameraFacing, playerFacing) - YawOffset - Mathf.Abs(yawAimOffset) < -CameraTurnDeadZone) {
					Yaw = CameraTurnDeadZone + 180f - playerAngle;
					// UpdateOffset();
				}
			}

		}

		UpdateOffset();

		if (InfoPane != null) {
			InfoPane.text = infoText;
		}
	}

	public void Turn(float AngleDelta) {

		// offset = Quaternion.AngleAxis(AngleDelta, Vector3.up) * offset;
		Yaw += AngleDelta;
		UpdateOffset();

	}

	private void UpdateOffset() {

		distanceOffset =
		Quaternion.AngleAxis(Yaw, Vector3.up)
			* Quaternion.AngleAxis(Pitch, Vector3.left)
			 * Vector3.forward * Zoom
		 ;

		transform.position = PositionAnchor.position + distanceOffset;
		transform.LookAt(PositionAnchor.position);

		if (Camera != null) {
			Camera.transform.transform.localRotation = Quaternion.identity
				* Quaternion.AngleAxis(YawOffset + yawAimOffset, transform.InverseTransformVector(Vector3.up))
				* Quaternion.AngleAxis(PitchOffset + pitchAimOffset, Vector3.left)
			 ;
		}

	}

	// TODO: deadzone for nozzle camera aim
	// IDEA: lerp nozzle camera aim to reduce jitter
	public void UpdateAimOffset(float yaw, float pitch) {
		yawAimOffset = Mathf.Sin(yaw * Mathf.Deg2Rad) * YawAimOffsetScale;
		pitchAimOffset = Mathf.Sin(pitch * Mathf.Deg2Rad) * PitchAimOffsetScale;
	}

}
