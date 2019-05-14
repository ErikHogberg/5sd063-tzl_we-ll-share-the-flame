using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorScript : MonoBehaviour {

	public GameObject PositionAnchorObject;
	public GameObject AngleAnchorObject;
	// public Vector3 AngleAxis = Vector3.up;
	// public bool KeepLocalPosition = false;
	// public bool AnchorParentRotation = false;
	private Vector3 worldPositionOffset;
	public bool CalledManually = false;


	// Start is called before the first frame update
	void Start() {
		worldPositionOffset = transform.localPosition;
	}

	// Update is called once per frame
	void LateUpdate() {
		// TODO: set execution order for chained anchors/camera follow

		if (!CalledManually) {
			UpdateAnchor();
		}

	}

	public void UpdateAnchor() {
		if (PositionAnchorObject != null) {
			//   if (KeepLocalPosition)
			//   {
			//     // FIXME
			//     transform.position = PositionAnchorObject.transform.position + worldPositionOffset;
			//   } else {
			transform.position = PositionAnchorObject.transform.position;
			//   }
		}

		if (AngleAnchorObject != null) {
			//   transform.localRotation = AngleAnchorObject.transform.localRotation;

			transform.localRotation = Quaternion.AngleAxis(AngleAnchorObject.transform.localRotation.eulerAngles.y, Vector3.up);
		}
	}
}
