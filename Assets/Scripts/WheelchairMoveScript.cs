using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;

public class WheelchairMoveScript : MonoBehaviour
{

	public GameObject LeftWheel;
	public GameObject RightWheel;

	public float Speed = 1.0f;
	public float Acceleration = 1.0f;
	public float Damping = 0.3f;
	public float TopSpeed = 1.0f;

	private float leftWheelSpeed = 0.0f;
	private float rightWheelSpeed = 0.0f;

	void Start()
	{

	}

	void Update()
	{
		var keyboard = Keyboard.current;

		float leftWheelDir = 0.0f;
		if (keyboard.wKey.isPressed)
		{
			leftWheelDir = Acceleration;
		}
		else if (keyboard.sKey.isPressed)
		{
			leftWheelDir = -Acceleration;
		}

		float rightWheelDir = 0.0f;
		if (keyboard.eKey.isPressed)
		{
			rightWheelDir = Acceleration;
		}
		else if (keyboard.dKey.isPressed)
		{
			rightWheelDir = -Acceleration;
		}

		leftWheelSpeed += leftWheelDir * Speed * Time.deltaTime;
		rightWheelSpeed += rightWheelDir * Speed * Time.deltaTime;

		/*
		if (leftWheelSpeed >= 0.0f)
		{
			leftWheelSpeed -= Damping;
			// TODO: snap to 0
		}
		else
		{
			leftWheelSpeed += Damping;
		}

		if (rightWheelSpeed >= 0.0f)
		{
			rightWheelSpeed -= Damping;
		}
		else
		{
			rightWheelSpeed += Damping;
		}
		 */

		leftWheelSpeed = Mathf.MoveTowards(leftWheelSpeed, 0.0f, Damping * Time.deltaTime);
		rightWheelSpeed = Mathf.MoveTowards(rightWheelSpeed, 0.0f, Damping * Time.deltaTime);

		// TODO: limit top speed

		// TODO: rotate
		transform.Rotate(transform.up, leftWheelSpeed - rightWheelSpeed);

		transform.position += transform.forward * Time.deltaTime * (leftWheelSpeed + rightWheelSpeed);

	}
}
