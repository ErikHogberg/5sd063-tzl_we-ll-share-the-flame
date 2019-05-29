using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;

public class WobbleScript : MonoBehaviour {

	// public GameObject WobblingObject;

	[Tooltip("Length in seconds of each animation for yaw, pitch and roll axis respectively")]
	public Vector3 AnimationTime = new Vector3(1f, 1f, 1f);
	[Tooltip("How many degress to wobble yaw, pitch and roll respectively")]
	public Vector3 WobbleAmount = new Vector3(90f, 90f, 90f);
	[Tooltip("How much to offset wobble in percentage of whole animation (0 to 1) for yaw, pitch and roll respectively")]
	public Vector3 WobbleTimeOffset = new Vector3(0f, 0f, 0f);

	public AnimationCurve YawAnimation;
	public AnimationCurve PitchAnimation;
	public AnimationCurve RollAnimation;

	private Timer yawAnimationTimer;
	private Timer pitchAnimationTimer;
	private Timer rollAnimationTimer;

	private float yawFlip = 1f;
	private float pitchFlip = 1f;
	private float rollFlip = 1f;


	void Start() {
		yawAnimationTimer = new Timer(AnimationTime.x);
		// yawAnimationTimer.Extend(-WobbleTimeOffset.x);

		pitchAnimationTimer = new Timer(AnimationTime.y);
		// pitchAnimationTimer.Extend(-WobbleTimeOffset.y);

		rollAnimationTimer = new Timer(AnimationTime.z);
		// rollAnimationTimer.Extend(-WobbleTimeOffset.z);
	}


	void Update() {
		if (yawAnimationTimer.Update()) {
			yawAnimationTimer.Restart(AnimationTime.x);
			yawFlip *= -1f;
		}
		if (pitchAnimationTimer.Update()) {
			pitchAnimationTimer.Restart(AnimationTime.y);
			pitchFlip *= -1f;
		}
		if (rollAnimationTimer.Update()) {
			rollAnimationTimer.Restart(AnimationTime.z);
			rollFlip *= -1f;
		}

		float yawTimerProgress = yawAnimationTimer.TimeLeft() / AnimationTime.x;
		float pitchTimerProgress = pitchAnimationTimer.TimeLeft() / AnimationTime.y;
		float rollTimerProgress = rollAnimationTimer.TimeLeft() / AnimationTime.z;

		float yaw = YawAnimation.Evaluate(
			(yawTimerProgress + WobbleTimeOffset.x * 0.5f) * 2f
		) * WobbleAmount.x * yawFlip;
		float pitch = PitchAnimation.Evaluate(
			(pitchTimerProgress + WobbleTimeOffset.y * 0.5f) * 2f
		) * WobbleAmount.y * pitchFlip;
		float roll = RollAnimation.Evaluate(
			(rollTimerProgress + WobbleTimeOffset.z * 0.5f) * 2f
		) * WobbleAmount.z * rollFlip;

		if (yawTimerProgress < WobbleTimeOffset.x * 0.5f
		|| yawTimerProgress < 1f - WobbleTimeOffset.x * 0.5f) {
			yaw *= -1f;
		}
		if (pitchTimerProgress < WobbleTimeOffset.y * 0.5f
		|| pitchTimerProgress < 1f - WobbleTimeOffset.y * 0.5f) {
			pitch *= -1f;
		}
		if (rollTimerProgress < WobbleTimeOffset.z * 0.5f
		|| rollTimerProgress < 1f - WobbleTimeOffset.z * 0.5f) {
			roll *= -1f;
		}

		Quaternion rotation = Quaternion.Euler(
			pitch,
			yaw,
			roll
		);

		// if (WobblingObject != null) {
		// 	WobblingObject.transform.localRotation = rotation;
		// } else {
		transform.localRotation = rotation;
		// }

	}
}
