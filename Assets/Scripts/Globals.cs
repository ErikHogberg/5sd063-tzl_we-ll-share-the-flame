using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts {
	public static class Globals {

		// Game-wide score
		private static float score = 0.2f;
		public static float Score {
			get { return score; }
		}
		public static float ScoreMultiplier = 0;

		// The current player
		public static WheelchairMoveScript Player;
		public static NozzleScript Nozzle;
		public static CameraFollowScript CameraScript;


		// UI
		public static NotificationWidgetScript NotificationPanel;
		public static FadeScript FadePanel;
		// public static Image GameOverPanel;

		public static GameObject PersitentMusic;

		public static void GameOver() {
			// if (GameOverPanel != null) {
			// 	GameOverPanel.gameObject.SetActive(true);
			// }

			// NOTE: Reset fields here

		}

		public static void AddScore(float points) {
			
			score += points;
		}

	}
}
