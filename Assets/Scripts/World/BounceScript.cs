using System.Collections;
using System.Collections.Generic;
using Assets.Scripts.Utilities;
using UnityEngine;

public class BounceScript : MonoBehaviour {

	// public GameObject WobblingObject;

	[Tooltip("Length in seconds of each animation for width, height and depth position respectively")]
	public Vector3 AnimationTime = new Vector3(1f, 1f, 1f);
	[Tooltip("How much to bounce width-, height- and depth-wise respectively")]
	public Vector3 BounceAmount = new Vector3(90f, 90f, 90f);
	[Tooltip("How much to offset bounce in percentage of whole animation (0 to 1) for width, height and depth respectively")]
	public Vector3 BounceTimeOffset = new Vector3(0f, 0f, 0f);

	public AnimationCurve WidthAnimation;
	public AnimationCurve HeightAnimation;
	public AnimationCurve DepthAnimation;
	public bool FlipRepeatWidthAnimation = true;
	public bool FlipRepeatHeightAnimation = true;
	public bool FlipRepeatDepthAnimation = true;

	private Timer widthAnimationTimer;
	private Timer heightAnimationTimer;
	private Timer depthAnimationTimer;

	private float widthFlip = 1f;
	private float heightFlip = 1f;
	private float depthFlip = 1f;

	private Vector3 initLocalPosition;

	void Start() {
		initLocalPosition = transform.localPosition;
		widthAnimationTimer = new Timer(AnimationTime.x);
		heightAnimationTimer = new Timer(AnimationTime.y);
		depthAnimationTimer = new Timer(AnimationTime.z);
	}


	void Update() {

		if (widthAnimationTimer.Update()) {
			widthAnimationTimer.Restart(AnimationTime.x);
			if (FlipRepeatWidthAnimation) {
				widthFlip *= -1f;
			}
		}
		if (heightAnimationTimer.Update()) {
			heightAnimationTimer.Restart(AnimationTime.y);
			if (FlipRepeatHeightAnimation) {
				heightFlip *= -1f;
			}
		}
		if (depthAnimationTimer.Update()) {
			depthAnimationTimer.Restart(AnimationTime.z);
			if (FlipRepeatDepthAnimation) {
				depthFlip *= -1f;
			}
		}

		float widthTimerProgress = widthAnimationTimer.TimeLeft() / AnimationTime.x;
		float heightTimerProgress = heightAnimationTimer.TimeLeft() / AnimationTime.y;
		float depthTimerProgress = depthAnimationTimer.TimeLeft() / AnimationTime.z;

		float x = WidthAnimation.Evaluate(
			(widthTimerProgress + BounceTimeOffset.x * 0.5f) * 2f
		) * BounceAmount.x * widthFlip;
		float y = HeightAnimation.Evaluate(
			(heightTimerProgress + BounceTimeOffset.y * 0.5f) * 2f
		) * BounceAmount.y * heightFlip;
		float z = DepthAnimation.Evaluate(
			(depthTimerProgress + BounceTimeOffset.z * 0.5f) * 2f
		) * BounceAmount.z * depthFlip;

		if (FlipRepeatWidthAnimation
		&& (widthTimerProgress < BounceTimeOffset.x * 0.5f
		|| widthTimerProgress < 1f - BounceTimeOffset.x * 0.5f)) {
			x *= -1f;
		}
		if (FlipRepeatHeightAnimation
		&& (heightTimerProgress < BounceTimeOffset.y * 0.5f
		|| heightTimerProgress < 1f - BounceTimeOffset.y * 0.5f)) {
			y *= -1f;
		}
		if (FlipRepeatDepthAnimation
		&& (depthTimerProgress < BounceTimeOffset.z * 0.5f
		|| depthTimerProgress < 1f - BounceTimeOffset.z * 0.5f)) {
			z *= -1f;
		}

		transform.localPosition = initLocalPosition + new Vector3(x, y, z);

	}
}
