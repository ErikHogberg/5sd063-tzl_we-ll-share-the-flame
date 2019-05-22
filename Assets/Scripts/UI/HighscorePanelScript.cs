using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class HighscorePanelScript : MonoBehaviour {

	private Highscore HighscoreList;
	public TextAsset SaveFile;
	public Text NameInput;
	public Button SubmitButton;

	void Start() {
		HighscoreList = Highscore.Load(SaveFile);
		if (HighscoreList == null) {
			HighscoreList = new Highscore();
		}

		SubmitButton.onClick.AddListener(SubmitScore);

		UpdateList();

	}

	public void SubmitScore() {
		int score = Globals.Score;
		// if (score > 0) {
		HighscoreList.ScoreList.Add(new ScoreEntry(NameInput.text, score));
		// HighscoreList.NameList.Add(NameInput.text);
		// }
		Globals.ResetScore();
		HighscoreList.Sort();

		UpdateList();

		Debug.Log("Submitted score");
		SaveHighScore();
	}

	public void UpdateList() {
		string scoreList = "";
		// if (HighscoreList.ScoreList.Count != HighscoreList.NameList.Count) {
		//Debug.LogError("uneven score lists!");
		// }

		// for (int i = 0; i < HighscoreList.ScoreList.Count; i++) {
		for (int i = HighscoreList.ScoreList.Count - 1; i >= 0; i--) {
			ScoreEntry scoreEntry = HighscoreList.ScoreList[i];

			// foreach (ScoreEntry scoreEntry in HighscoreList.ScoreList) {

			scoreList +=
			//HighscoreList.NameList[i] + " " + 
			// HighscoreList.ScoreList[i]
			scoreEntry.Player + ": " + scoreEntry.Score + "\n";
		}

		GetComponent<Text>().text = scoreList;
	}

	public void SaveHighScore() {
		HighscoreList.Save(SaveFile);
		Debug.Log("Saved score");
	}

	private void OnDestroy() {
		SaveHighScore();
	}

}
