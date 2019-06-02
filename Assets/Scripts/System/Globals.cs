using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {

	[Serializable]
	public class ScoreEntry {
		public string Player;
		public int Score;

		public ScoreEntry(string player, int score) {
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

		public static Highscore Load() {
			if (File.Exists(Globals.SavePath)) {
				return JsonUtility.FromJson<Highscore>(File.ReadAllText(Globals.SavePath));
			} else {
				return new Highscore();
			}
		}

		public static Highscore Load(TextAsset SaveFile) {
			return JsonUtility.FromJson<Highscore>(SaveFile.text);
		}

		public string ToJson(bool pretty = true) {
			return JsonUtility.ToJson(this, pretty);
		}

		public void Save() {

			// TODO: save in build
			// File.WriteAllText(AssetDatabase.GetAssetPath(SaveFile), ToJson());
			// File.WriteAllText(Application.dataPath + "/Resources/" + SaveFile.name,ToJson());

			File.WriteAllText(Globals.SavePath, ToJson());
			Debug.Log("saved to: " + Globals.SavePath);

			// File.WriteAllText(Application.persistentDataPath + "/Resources/highscore.json",ToJson());
			// Debug.Log("saved to: " + Application.dataPath + "/Resources/" + SaveFile.name);
		}

		public void Sort() {
			ScoreList = ScoreList.OrderByDescending(x => x.Score).Take(10).ToList();
			// .Sort(delegate (ScoreEntry e1, ScoreEntry e2) { return e1.Score.CompareTo(e2.Score); });
		}

	}

	public static class Globals {

		// Game-wide score
		private static float score = 0f;
		public static int Score {
			get { return Mathf.FloorToInt(score); }
		}
		public static float ScoreMultiplier = 1;
		public static string SavePath = Application.dataPath + "/highscore.json";
		public static Highscore HighscoreList = Highscore.Load();//new Highscore();

		public static CollectiblePanelScript CollectiblePanel;

		// The current player
		public static WheelchairMoveScript Player;
		public static NozzleScript Nozzle;
		public static CameraFollowScript CameraScript;
		public static StartZoneScript StartZone;

		// Wiimote
		public static bool irToggle = false;


		// UI
		public static NotificationWidgetScript NotificationPanel;
		public static FadeScript FadePanel;
		public static TimeCounterWidgetScript TimerPanel;
		// public static Image GameOverPanel;

		public static ScoreMultiplierWidgetScript ScoreMultiplierPanel;

		// TODO: make separate script for assigning music when starting a scene
		public static AudioSource PersitentMusic;

		public static void GameOver() {
			// if (GameOverPanel != null) {
			// 	GameOverPanel.gameObject.SetActive(true);
			// }

			// NOTE: Reset fields here

		}

		public static void AddScore(float points, float multiplierIncrease) {
			ScoreMultiplier += multiplierIncrease;
			if (ScoreMultiplierPanel != null) {
				score += ScoreMultiplierPanel.AddPoints(points);
			} else {
				score += points;
				Debug.Log("No Score multiplier bar widget!");
			}
		}

		public static void ResetScore() {
			score = 0f;
		}

		public static void ToggleIr(){
			irToggle = !irToggle;
			Debug.Log("ir set to " + irToggle);
		}

	}
}
