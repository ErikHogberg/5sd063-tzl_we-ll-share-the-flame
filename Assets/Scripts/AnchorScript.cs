using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorScript : MonoBehaviour
{

	public GameObject AnchorObject;
	public bool KeepLocalPosition = false;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
		if (KeepLocalPosition)
		{
            transform.position = AnchorObject.transform.position + transform.localPosition;

        } else
		{
        	transform.position = AnchorObject.transform.position;
			
		}
    }
}
