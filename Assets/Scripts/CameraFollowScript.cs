using UnityEngine;
using UnityEngine.Experimental.Input;

public class CameraFollowScript : MonoBehaviour
{

    [SerializeField]
    private InputActionAsset controls;

    public float turnSpeedX = 4.0f;
    public float turnSpeedY = 2.0f;
    public float scrollSpeed = 1.1f;
    public Transform player;

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
		
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


		// Vector2 scroll = Mouse.current.scroll.ReadValue();
        // offset *= 1.0f + scroll.y;

        transform.position = player.position + offset;
        transform.LookAt(player.position);
    }

    public void Turn(float AngleDelta) {
            
            offset = Quaternion.AngleAxis(AngleDelta, Vector3.up) * offset;
            
            transform.position = player.position + offset;
            transform.LookAt(player.position);
    }

}
