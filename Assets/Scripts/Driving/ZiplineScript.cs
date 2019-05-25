using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZiplineScript : MonoBehaviour
{

    public GameObject End;
    public GameObject Line;
    public float Speed = 1f;


    private void Start() {
        
        // move line pivot to start
        Line.transform.position = transform.position;
        // rotate line towards end
        Line.transform.LookAt(End.transform);

        // stretch line to end
        Vector3 scale = Line.transform.localScale;
        scale.z = Vector3.Distance(transform.position, End.transform.position);
        Line.transform.localScale = scale;
    }

}
