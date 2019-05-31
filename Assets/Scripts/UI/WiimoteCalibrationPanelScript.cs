using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.Experimental.Input;
using WiimoteApi;

public class WiimoteCalibrationPanelScript : MonoBehaviour
{

	public RectTransform IrDot;
	private Vector2 irDotSize;

	public RectTransform LeftDot;
	public RectTransform RightDot;
	public RectTransform CalibrateDot;

	private bool leftClicked = false;
	private bool rightClicked = false;

	[Tooltip("How close the IR cursor has to be to the center of the button to click")]
	public float ClickAccuracy = 1f;

	private Wiimote wiimote;


	void Start()
    {
        irDotSize = IrDot.anchorMax - IrDot.anchorMin;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

		wiimote = WiimoteManager.Wiimotes[0];


		int ret;
		do {
			ret = wiimote.ReadWiimoteData();

			if (wiimote.current_ext != ExtensionController.MOTIONPLUS) {
				wiimote.RequestIdentifyWiiMotionPlus(); // find/prepare wmp
				wiimote.SendDataReportMode(InputDataType.REPORT_BUTTONS_EXT8); // set data reporting to support wmp
				wiimote.ActivateWiiMotionPlus(); // force wmp
			} else {

			}
		} while (ret > 0);

		if (wiimote != null)
		{
			
			// left 0, down 0
			float[] pointer = wiimote.Ir.GetPointingPosition();
			// Debug.Log("ir loop");
			// if (pointer[0] > -1f && pointer[1] > -1f) {
			IrDot.anchorMax = new Vector2(pointer[0], pointer[1]) + irDotSize/2f;
			IrDot.anchorMin = new Vector2(pointer[0], pointer[1]) - irDotSize/2f;

			// Debug.Log("pointer: " + pointer[0] + ", " + pointer[1]);
			// ir_pointer.anchorMin = new Vector2(pointer[0], pointer[1]);
			// ir_pointer.anchorMax = new Vector2(pointer[0], pointer[1]);
		}


		bool hoverLeft = Vector2.Distance(GetDotCenter(IrDot), GetDotCenter(LeftDot)) < ClickAccuracy;
		bool hoverRight = Vector2.Distance(GetDotCenter(IrDot), GetDotCenter(RightDot)) < ClickAccuracy;
		bool hoverCalibrate = Vector2.Distance(GetDotCenter(IrDot), GetDotCenter(CalibrateDot)) < ClickAccuracy;

		if (keyboard.digit1Key.wasPressedThisFrame)
		{
			if (hoverLeft)
			{
				
			}
		}
    }

	private Vector2 GetDotCenter(RectTransform dot) {
		return dot.anchorMin + (dot.anchorMax/2f);
	}

	private void EndCalibration() {
		Globals.Player.DisableMovement = false;
		gameObject.SetActive(false);
	}

}
