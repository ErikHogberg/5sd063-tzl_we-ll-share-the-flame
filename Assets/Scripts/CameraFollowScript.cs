using UnityEngine;
using System.Collections;
using UnityEngine.Experimental.Input;

public class CameraFollowScript : MonoBehaviour
{

    public float turnSpeedX = 4.0f;
    public float turnSpeedY = 2.0f;
    public Transform player;

    private Vector3 offset;

    void Start()
    {
        offset = new Vector3(player.position.x, player.position.y + 8.0f, player.position.z + 7.0f);
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

        transform.position = player.position + offset;
        transform.LookAt(player.position);
    }
}