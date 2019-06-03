using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineScript : MonoBehaviour
{

    public GameObject End;
    public GameObject Line;
    public float Speed = 1f;
	public float EndJumpSpeed = 1f;
	public float EndJumpTime = 1f;
    public JumpTargetSetting TargetHeightRelativity= JumpTargetSetting.Reset;
    public float TargetHeight = 0f;
    public bool SkipUp = false;
	public float ScoreWorth = 100f;
	public float ScoreMultiplierIncrease = 0.1f;


    private void Start() {
        
        // move line pivot to start
        Line.transform.position = transform.position;

        // rotate line towards end
        Line.transform.LookAt(End.transform);

        // stretch line to end
        Vector3 scale = Line.transform.localScale;
		scale.z *= Vector3.Distance(transform.position, End.transform.position) / Line.transform.lossyScale.z;
        Line.transform.localScale = scale;

		Line.transform.position = transform.position + new Vector3(0f,2.1f,0f);

	}

}
