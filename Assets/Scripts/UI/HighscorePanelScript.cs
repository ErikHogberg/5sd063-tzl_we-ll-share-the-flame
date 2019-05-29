using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class HighscorePanelScript : MonoBehaviour {

	private Highscore HighscoreList;
	public TextAsset SaveFile;
	public InputField NameInput;
	// public Text NameInputText;
	public Button SubmitButton;

	void Start() {
		// HighscoreList = Highscore.Load(SaveFile);
		HighscoreList = Globals.HighscoreList;
		if (HighscoreList == null) {
			HighscoreList = new Highscore();
		}

		SubmitButton.onClick.AddListener(SubmitScore);

		UpdateList();

	}

	public void SubmitScore() {
		int score = Globals.Score;
		if (score > 0) {
			HighscoreList.ScoreList.Add(new ScoreEntry(NameInput.textComponent.text, score));
			// HighscoreList.NameList.Add(NameInput.text);
			Globals.ResetScore();
			HighscoreList.Sort();

			UpdateList();

			Debug.Log("Submitted score");
			SaveHighScore();
		}
	}

	public void UpdateList() {
		string scoreList = "";
		// if (HighscoreList.ScoreList.Count != HighscoreList.NameList.Count) {
		//Debug.LogError("uneven score lists!");
		// }

		// TODO: better formatting for consistent name horizontal alignment
		foreach (ScoreEntry scoreEntry in HighscoreList.ScoreList) {

			scoreList +=
			scoreEntry.Player + " - " + scoreEntry.Score + "\n";
		}

		GetComponent<Text>().text = scoreList;
	}

	public void SaveHighScore() {
		HighscoreList.Save();
		Debug.Log("Saved score");
	}

	private void OnDestroy() {
		// SaveHighScore();
	}

}
