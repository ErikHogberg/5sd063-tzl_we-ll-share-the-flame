using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;

public class BounceScript : MonoBehaviour {

	// public GameObject WobblingObject;

	[Tooltip("Length in seconds of each animation for yaw, pitch and roll axis respectively")]
	public Vector3 AnimationTime = new Vector3(1f, 1f, 1f);
	[Tooltip("How many degress to wobble yaw, pitch and roll respectively")]
	public Vector3 BounceAmount = new Vector3(90f, 90f, 90f);
	[Tooltip("How much to offset wobble in percentage of whole animation (0 to 1) for yaw, pitch and roll respectively")]
	public Vector3 BounceTimeOffset = new Vector3(0f, 0f, 0f);

	public AnimationCurve WidthAnimation;
	public AnimationCurve DepthAnimation;
	public AnimationCurve HeightAnimation;

	private Timer widthAnimationTimer;
	private Timer depthAnimationTimer;
	private Timer heightAnimationTimer;

	private float widthFlip = 1f;
	private float depthFlip = 1f;
	private float heightFlip = 1f;

	private Vector3 initLocalPosition;

	void Start() {
		initLocalPosition = transform.localPosition;
		widthAnimationTimer = new Timer(AnimationTime.x);
		depthAnimationTimer = new Timer(AnimationTime.y);
		heightAnimationTimer = new Timer(AnimationTime.z);
	}


	void Update() {
		if (widthAnimationTimer.Update()) {
			widthAnimationTimer.Restart(AnimationTime.x);
			widthFlip *= -1f;
		}
		if (depthAnimationTimer.Update()) {
			depthAnimationTimer.Restart(AnimationTime.y);
			depthFlip *= -1f;
		}
		if (heightAnimationTimer.Update()) {
			heightAnimationTimer.Restart(AnimationTime.z);
			heightFlip *= -1f;
		}

		float widthTimerProgress = widthAnimationTimer.TimeLeft() / AnimationTime.x;
		float heightTimerProgress = depthAnimationTimer.TimeLeft() / AnimationTime.y;
		float depthTimerProgress = heightAnimationTimer.TimeLeft() / AnimationTime.z;

		float x = WidthAnimation.Evaluate(
			(widthTimerProgress + BounceTimeOffset.x * 0.5f) * 2f
		) * BounceAmount.x * widthFlip;
		float z = DepthAnimation.Evaluate(
			(heightTimerProgress + BounceTimeOffset.y * 0.5f) * 2f
		) * BounceAmount.y * depthFlip;
		float y = HeightAnimation.Evaluate(
			(depthTimerProgress + BounceTimeOffset.z * 0.5f) * 2f
		) * BounceAmount.z * heightFlip;

		if (widthTimerProgress < BounceTimeOffset.x * 0.5f
		|| widthTimerProgress < 1f - BounceTimeOffset.x * 0.5f) {
			x *= -1f;
		}
		if (heightTimerProgress < BounceTimeOffset.y * 0.5f
		|| heightTimerProgress < 1f - BounceTimeOffset.y * 0.5f) {
			z *= -1f;
		}
		if (depthTimerProgress < BounceTimeOffset.z * 0.5f
		|| depthTimerProgress < 1f - BounceTimeOffset.z * 0.5f) {
			y *= -1f;
		}

		// Quaternion rotation = Quaternion.Euler(
		// 	z,
		// 	x,
		// 	y
		// );

		transform.localPosition = initLocalPosition + new Vector3(
			x,y,z
		);

		// if (WobblingObject != null) {
		// 	WobblingObject.transform.localRotation = rotation;
		// } else {
		// transform.localRotation = rotation;
		// }

	}
}
