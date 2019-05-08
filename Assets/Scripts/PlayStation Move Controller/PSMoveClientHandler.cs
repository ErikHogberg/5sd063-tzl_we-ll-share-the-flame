using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using System;

public class PSMoveClientHandler : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		int result = PSM_Initialize("localhost", "9512", 100);
		//int result2 = PSM_Initialize("localhost", "9512", 100);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

	[DllImport("PSMoveClient_CAPI")]
	private static extern int PSM_Initialize(string host, string port, int timeout_ms);
}
