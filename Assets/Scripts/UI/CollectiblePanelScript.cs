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
				break;
			case CollectibleType.Cat:
				cats += 1;
				break;
			case CollectibleType.Graffiti:
				graffiti += 1;
				break;
		}

		dirtyData = true;

	}

}
