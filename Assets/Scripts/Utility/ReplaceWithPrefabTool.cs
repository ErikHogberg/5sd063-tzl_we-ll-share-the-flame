#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

// Source: https://gist.github.com/unity3dcollege/c1efea3f87d3775bee3e010e9c6d7648#file-replacewithprefab-cs
public class ReplaceWithPrefab : EditorWindow {
	[SerializeField] private GameObject prefab;

	[MenuItem("Tools/Replace With Prefab")]
	static void CreateReplaceWithPrefab() {
		EditorWindow.GetWindow<ReplaceWithPrefab>();
	}

	private void OnGUI() {
		prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", prefab, typeof(GameObject), false);

		if (GUILayout.Button("Replace")) {
			var selection = Selection.gameObjects;

			for (var i = selection.Length - 1; i >= 0; --i) {
				var selected = selection[i];
				var prefabType = PrefabUtility.GetPrefabAssetType(prefab);//.GetPrefabType(prefab);
								
				GameObject newObject;

                // if (prefabType == PrefabType.Prefab) {
                // 	newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
                // } else {
                // 	newObject = Instantiate(prefab);
                // 	newObject.name = prefab.name;
                // }

                if (prefabType == PrefabAssetType.NotAPrefab) {
                    newObject = Instantiate(prefab);
                    newObject.name = prefab.name;
                } else {
                    newObject = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
				}

                if (newObject == null) {
					Debug.LogError("Error instantiating prefab");
					break;
				}

				Undo.RegisterCreatedObjectUndo(newObject, "Replace With Prefabs");
				newObject.transform.parent = selected.transform.parent;
				newObject.transform.localPosition = selected.transform.localPosition;
				newObject.transform.localRotation = selected.transform.localRotation;
				newObject.transform.localScale = selected.transform.localScale;
				newObject.transform.SetSiblingIndex(selected.transform.GetSiblingIndex());
				Undo.DestroyObjectImmediate(selected);
			}
		}

		GUI.enabled = false;
		EditorGUILayout.LabelField("Selection count: " + Selection.objects.Length);
	}
}
#endif
