using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {

	[Serializable]
	public class ScoreEntry {
		public string Player;
		public int Score;

		public ScoreEntry (string player, int score) {
			Player = player;
			Score = score;
		}
	}

	[Serializable]
	public class Highscore {
		public List<ScoreEntry> ScoreList;
		// public List<string> NameList;

		public Highscore() {
			ScoreList = new List<ScoreEntry>();
			// NameList = new List<string>();
			Debug.Log("New highscore list");
		}

		public static Highscore Load(TextAsset SaveFile) {
			return JsonUtility.FromJson<Highscore>(SaveFile.text);
		}

		public string ToJson(bool pretty = true) {
			return JsonUtility.ToJson(this, pretty);
		}

		public void Save(TextAsset SaveFile) {

			File.WriteAllText(AssetDatabase.GetAssetPath(SaveFile), ToJson());
		}

		public void Sort() {
			ScoreList = ScoreList.OrderByDescending(x =>  x.Score).Take(10).ToList();
			// .Sort(delegate (ScoreEntry e1, ScoreEntry e2) { return e1.Score.CompareTo(e2.Score); });
		}

	}

	public static class Globals {

		// Game-wide score
		private static float score = 100f;
		public static int Score {
			get { return Mathf.FloorToInt(score); }
		}
		public static float ScoreMultiplier = 1;

		// The current player
		public static WheelchairMoveScript Player;
		public static NozzleScript Nozzle;
		public static CameraFollowScript CameraScript;


		// UI
		public static NotificationWidgetScript NotificationPanel;
		public static FadeScript FadePanel;
		// public static Image GameOverPanel;

		public static ScoreMultiplierWidgetScript ScoreMultiplierPanel;

		public static GameObject PersitentMusic;

		public static void GameOver() {
			// if (GameOverPanel != null) {
			// 	GameOverPanel.gameObject.SetActive(true);
			// }

			// NOTE: Reset fields here

		}

		public static void AddScore(float points, float multiplierIncrease) {
			ScoreMultiplier += multiplierIncrease;
			score += ScoreMultiplierPanel.AddPoints(points);
		}

		public static void ResetScore() {
			score = 0f;
		}

	}
}
