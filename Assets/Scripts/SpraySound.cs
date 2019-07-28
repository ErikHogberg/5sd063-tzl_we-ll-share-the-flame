using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpraySound : MonoBehaviour {
	[Header("Sound FX")]
	public AudioClip SFX_Spray_Start;
	public AudioClip SFX_Spray_Mid;
	public AudioClip SFX_Spray_End;
	public AudioSource AS_Spray;

	bool playing;

	void Start() {
		AS_Spray.clip = SFX_Spray_Start;
		playing = false;
	}

	void Update() {

		/*
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			PlaySound();
		}
		if (Input.GetKeyUp(KeyCode.Alpha1)) {
			StopSound();
		}
		// */
		
		//if (Input.GetKeyDown(KeyCode.T)) {
		//	StopCoroutine("playBoostSFX");
		//	AS_Boost.Stop();
		//	AS_Boost.loop = false;
		//	AS_Boost.clip = SFX_Boost_End;
		//	AS_Boost.Play();
		//	playing = false;
		//}
	}

	public void PlaySound() {
		if (playing == false) {
			StartCoroutine("playBoostSFX");
		}
	}

	public void StopSound() {
		if (playing == true) {
			StopCoroutine("playBoostSFX");
			AS_Spray.Stop();
			AS_Spray.loop = false;
			AS_Spray.clip = SFX_Spray_End;
			AS_Spray.Play();
			playing = false;
		}
	}

	IEnumerator playBoostSFX() {
		playing = true;
		AS_Spray.clip = SFX_Spray_Start;
		// Play the sound
		AS_Spray.Play();
		yield return new WaitForSeconds(AS_Spray.clip.length);
		AS_Spray.clip = SFX_Spray_Mid;
		AS_Spray.loop = true;
		AS_Spray.Play();
	}

}
