using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class HighscorePanelScript : MonoBehaviour {

	private Highscore HighscoreList;
	public TextAsset SaveFile;
	public Text NameInput;

	void Start() {
		HighscoreList = Highscore.Load(SaveFile);
		if (HighscoreList == null) {
			HighscoreList = new Highscore();
		}

		HighscoreList.ScoreList.Add(Mathf.FloorToInt(Globals.Score));
		HighscoreList.NameList.Add(NameInput.text);

		string scoreList = "";
		if (HighscoreList.ScoreList.Count != HighscoreList.NameList.Count) {
			Debug.LogError("uneven score lists!");
		}

		for (int i = 0; i < HighscoreList.ScoreList.Count; i++) {
			scoreList += HighscoreList.NameList[i] + " " + HighscoreList.ScoreList[i] + "\n";
		}

		GetComponent<Text>().text = scoreList;

	}

	void Update() {

	}

	public void SaveHighScore() {
		HighscoreList.Save(SaveFile);
	}

	private void OnDestroy() {
		SaveHighScore();
	}

}
