using System.Collections;
using System.Collections.Generic;
using Assets.Scripts;
using UnityEngine;
using UnityEngine.UI;

public class CollectiblePanelScript : MonoBehaviour {

	public Text ElementalText;
	public Text CatText;
	public Text GraffitiText;

	private bool dirtyData = true;

	private int totalElementals = 0;
	private int totalCats = 0;
	private int totalGraffiti = 0;

	private int elementals = 0;
	private int cats = 0;
	private int graffiti = 0;

	public float ElementalScoreWorth = 1f;
	public float CatScoreWorth = 1f;
	public float GraffitiScoreWorth = 1f;

	public float ElementalScoreMultiplierIncrease = 1f;
	public float CatScoreMultiplierIncrease = 1f;
	public float GraffitiScoreWMultiplierIncrease = 1f;


	void Start() {
		Globals.CollectiblePanel = this;
	}

	void Update() {
		if (dirtyData) {

			ElementalText.text = "" + elementals + "/" + totalElementals;
			CatText.text = "" + cats + "/" + totalCats;
			GraffitiText.text = "" + graffiti + "/" + totalGraffiti;

			dirtyData = false;
		}
	}

	public void AddTotalCollectibles(CollectibleType type) {
		switch (type) {
			case CollectibleType.BigBlueElemental:
				totalElementals += 1;
				break;
			case CollectibleType.Cat:
				totalCats += 1;
				break;
			case CollectibleType.Graffiti:
				totalGraffiti += 1;
				break;
		}

		dirtyData = true;

	}

	public void AddCollectibles(CollectibleType type) {
		switch (type) {
			case CollectibleType.BigBlueElemental:
				elementals += 1;
				Globals.AddScore(ElementalScoreWorth * elementals, ElementalScoreMultiplierIncrease);
				if (Globals.CollectibleNotificationPanel != null) {
					if (elementals < totalElementals) {
						Globals.CollectibleNotificationPanel.Notify("" + elementals + "/" + totalElementals + " big blue elementals!");
					} else {
						Globals.CollectibleNotificationPanel.Notify("All big blue elementals defeated! Massive bonus!");
					}
				}
				break;
			case CollectibleType.Cat:
				cats += 1;
				Globals.AddScore(CatScoreWorth * cats, CatScoreMultiplierIncrease);
				if (Globals.CollectibleNotificationPanel != null) {
					if (cats < totalCats) {
						Globals.CollectibleNotificationPanel.Notify("" + cats + "/" + totalCats + " cats saved!");
					} else {
						Globals.CollectibleNotificationPanel.Notify("All cats saved! Massive bonus!");
					}
				}
				break;
			case CollectibleType.Graffiti:
				graffiti += 1;
				Globals.AddScore(GraffitiScoreWorth * elementals, GraffitiScoreWMultiplierIncrease);
				if (Globals.CollectibleNotificationPanel != null) {
					if (graffiti < totalGraffiti) {
						Globals.CollectibleNotificationPanel.Notify("" + graffiti + "/" + totalGraffiti + " graffiti cleaned!");
					} else {
						Globals.CollectibleNotificationPanel.Notify("All graffiti cleaned! Massive bonus!");
					}
				}
				break;
		}

		dirtyData = true;

	}

}
