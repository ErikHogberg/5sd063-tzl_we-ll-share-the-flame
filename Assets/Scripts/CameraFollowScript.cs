using UnityEngine;
using UnityEngine.Experimental.Input;

public class CameraFollowScript : MonoBehaviour
{

    [SerializeField]
    private InputActionAsset controls;

    public float turnSpeedX = 4.0f;
    public float turnSpeedY = 2.0f;
    public float scrollSpeed = 1.1f;
    public Transform PositionAnchor;
    public Transform AngleAnchor;

    private Vector3 offset;
	// public Vector2 Distance = new Vector2(8.0f, 7.0f);

	public bool AutoTurning = true;
    public float CameraTurnSpeed = 1.0f;
    public float CameraTurnSpeedScale = 1.0f;
    public float CameraTurnDeadZone = 1.0f;

    void Start() {
        Vector3 distance = transform.position - PositionAnchor.transform.position;
        offset = distance;
		
        // InputAction action = controls.TryGetActionMap("shooter").TryGetAction("shoot");

        {
            // FIXME: debug action map not triggering
			InputAction action = controls.TryGetActionMap("debug").TryGetAction("zoom in");
            action.performed += _ => { offset *= scrollSpeed; Debug.Log("zoom in"); };
		}

		{
            // InputAction action = controls.TryGetActionMap("shooter").TryGetAction("shoot");
			// FIXME: debug action map not triggering
            InputAction action = controls.TryGetActionMap("debug").TryGetAction("zoomout");
    	    action.performed += _ => { offset *= 2.0f - scrollSpeed; Debug.Log("zoom out"); };
		}

    }

    void LateUpdate()
    {
        if (Mouse.current.rightButton.isPressed)
        {

            //Mouse.current.position.x.ReadValue()

            //offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeedX, Vector3.up) * offset;
            offset = Quaternion.AngleAxis(Mouse.current.delta.x.ReadValue() * turnSpeedX * Time.deltaTime, Vector3.up) * offset;
            //offset = Quaternion.AngleAxis(Input.GetAxis("Mouse Y") * turnSpeedY, transform.right) * offset;
            offset = Quaternion.AngleAxis(Mouse.current.delta.y.ReadValue() * turnSpeedY * Time.deltaTime, transform.right) * offset;
            //offset = Quaternion.AngleAxis(Input.GetAxis("Mouse X") * turnSpeed, Vector3.up);
        }

		if ((Keyboard.current.iKey.isPressed ) && Vector3.Magnitude(offset) > 0.1f ) {
			offset *= 1.0f - scrollSpeed * Time.deltaTime;
		}
		if (Keyboard.current.oKey.isPressed ) {
			offset *= 1.0f + scrollSpeed * Time.deltaTime;
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

            // TODO: quickly move the camera behind the player when they face the camera

            Vector2 cameraFacing = new Vector2(transform.forward.x, transform.forward.z);
            Vector2 playerFacing = new Vector2(AngleAnchor.transform.forward.x, AngleAnchor.transform.forward.z);

            // Camera.transform.rotation.ToAngleAxis(out float cameraAngle, out Vector3 cameraAxis);
            // transform.rotation.ToAngleAxis(out float playerAngle, out Vector3 playerAxis);

            float angleDelta = Vector2.SignedAngle(cameraFacing, playerFacing);
            float turnSpeed = CameraTurnSpeed + CameraTurnSpeedScale;// * speed;
            if (angleDelta < -CameraTurnDeadZone) {
                // if (turnSpeed > angleDelta) {
                //     CameraScript.Turn(angleDelta);
                // } else 
                {
                    Turn(turnSpeed * Time.deltaTime);
                }
            } else if (angleDelta > CameraTurnDeadZone) {
                // CameraScript.Turn(-turnSpeed * Time.deltaTime);

                // if (turnSpeed < angleDelta)
                // {
                //     CameraScript.Turn(angleDelta);
                // }
                // else
                {
                    Turn(-turnSpeed * Time.deltaTime);
                }

            }

        }

		// TODO: restrict pitch

		transform.position = PositionAnchor.position + offset;
        transform.LookAt(PositionAnchor.position);
    }

    public void Turn(float AngleDelta) {
            
            offset = Quaternion.AngleAxis(AngleDelta, Vector3.up) * offset;
            
            transform.position = PositionAnchor.position + offset;
            transform.LookAt(PositionAnchor.position);
    }

}
