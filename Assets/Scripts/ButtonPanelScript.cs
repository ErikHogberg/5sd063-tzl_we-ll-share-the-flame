using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Input;
using UnityEngine.UI;

public enum DebugKey
{
	W,
	S,
	E,
	D
}

public class ButtonPanelScript : MonoBehaviour
{

	public DebugKey debugKey;
	private UnityEngine.Experimental.Input.Controls.KeyControl key;

	private Color defaultColor;
	public Color DownColor;

	private Image image;
		
	// Start is called before the first frame update
	void Start()
	{
		image = GetComponent<Image>();
		defaultColor = image.color;

		switch (debugKey)
		{
			case DebugKey.W:
				key = Keyboard.current.wKey;
				break;
			case DebugKey.S:
				key = Keyboard.current.sKey;
				break;
			case DebugKey.E:
				key = Keyboard.current.eKey;
				break;
			case DebugKey.D:
				key = Keyboard.current.dKey;
				break;
			default:
				break;
		}
	}

	// Update is called once per frame
	void Update()
	{


		if (key.isPressed)
		{
			image.color = DownColor;
		}
		else
		{
			image.color = defaultColor;
		}

	}
}
