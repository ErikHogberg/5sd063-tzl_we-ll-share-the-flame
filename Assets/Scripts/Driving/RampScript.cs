using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampScript : MonoBehaviour {

	// TODO: min/max/set movement speed
	// IDEA: set sump target location (as game object)
	// TODO: custom jump time

	public bool RelativeHeight = false;
	public float TargetHeight = 0f;
	public float JumpHeight = 0f;
	public bool SkipUp = false;

	public bool SetSpeed = false;
	public float Speed = 1f;

	public bool SetTime = false;
	public float Time = 1f;
	public bool AlignPlayer = false;

	public Vector3 StuntAxis = new Vector3(1f, 0f, 0f);
	public float StuntAngle = 360f;
	public bool StuntPingPong = false;


}
