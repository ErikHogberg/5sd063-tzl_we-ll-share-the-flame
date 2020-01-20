using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.InputSystem;
// using UnityEngine.Experimental.Input;
using UnityEngine.SceneManagement;

public class PressToStartScript : MonoBehaviour
{

    public string Level;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current.digit1Key.wasPressedThisFrame)
        {
            StartLevel(Level);
        }
    }

    public void StartLevel(string level) {
		if (Globals.FadePanel != null) {
			Globals.FadePanel.StartLevelTransition(level);
		} else {
			SceneManager.LoadScene(level, LoadSceneMode.Single);
		}
	}
}
