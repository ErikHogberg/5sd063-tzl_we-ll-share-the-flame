using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;
using UnityEngine.Experimental.Input;
using UnityEngine.XR;

public class PSMoveClientHandler : MonoBehaviour
{

	//[SerializeField]
	//private InputActionAsset controls;


	// Start is called before the first frame update
	void Start()
    {
		//int result = PSM_Initialize("localhost", "9512", 100);
		//{
		//	InputAction action = controls.TryGetActionMap("shooter").TryGetAction("yaw");
		//	action.performed += _ => {

		//		Debug.Log("yaw");
		//	};
		//	action.Enable();
		//}

		//var inputDevices = new List<UnityEngine.XR.InputDevice>();
		//InputDevices.GetDevices(inputDevices);
		//foreach (var device in inputDevices) {
		//	Debug.Log(string.Format("Device found with name '{0}' and role '{1}'",
		//			  device.name, device.role.ToString()));
		//}

	}

    // Update is called once per frame
    void Update()
    {
		if (Keyboard.current.xKey.wasPressedThisFrame) {
			Debug.Log("psmove count: " + psmove_count_connected());
		}
    }

	//[DllImport("PSMoveClient_CAPI")]
	//private static extern int PSM_Initialize(string host, string port, int timeout_ms);
	[DllImport("libpsmoveapi")]
	private static extern int psmove_count_connected();
}
