using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JumpTargetSetting {
	Relative,
	Absolute,
	Reset
}

public class RampScript : MonoBehaviour {

	// TODO: min/max/set movement speed
	// IDEA: set sump target location (as game object)
	// TODO: custom jump time

	// public bool RelativeHeight = false;
	[Tooltip("How the target height is applied, relative to start height, world height, or initial player Y position on level start")]	
	public JumpTargetSetting TargetHeightRelativity = JumpTargetSetting.Absolute;
	[Tooltip("The Y position to land on")]
	public float TargetHeight = 0f;
	[Tooltip("How the jump height is applied, relative to start height, world height, or initial player Y position on level start")]	
	public JumpTargetSetting JumpHeightRelativity = JumpTargetSetting.Relative;
	[Tooltip("The Y position for the top height of the jump arc")]
	public float JumpHeight = 0f;
	public bool SkipUp = false;

	public bool SetSpeed = false;
	public float Speed = 1f;

	public bool SetTime = false;
	public float Time = 1f;
	public bool AlignPlayer = false;
	public bool JumpNormallyIfWrongWay = true;

	public Vector3 StuntAxis = new Vector3(1f, 0f, 0f);
	public float StuntAngle = 360f;
	public bool StuntPingPong = false;
	public float ScoreWorth = 100f;
	public float ScoreMultiplierIncrease = 0.1f;


}
