// Source: https://answers.unity.com/questions/694371/how-to-search-gameobject-in-scene-by-tag-name.html
using UnityEngine;
using System.Collections;
using UnityEditor;

public class SelectByTagTool : MonoBehaviour {

	private static string SelectedTag = "Building";

	[MenuItem("Helpers/Select By Tag")]
	public static void SelectObjectsWithTag() {
		GameObject[] objects = GameObject.FindGameObjectsWithTag(SelectedTag);
		Selection.objects = objects;
	}
}
