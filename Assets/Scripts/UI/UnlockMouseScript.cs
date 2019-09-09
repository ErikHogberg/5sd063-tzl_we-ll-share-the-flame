using UnityEngine;

public class UnlockMouseScript : MonoBehaviour {

	void Start() {
		Cursor.lockState = CursorLockMode.None;
		Cursor.visible = true;
	}

}
