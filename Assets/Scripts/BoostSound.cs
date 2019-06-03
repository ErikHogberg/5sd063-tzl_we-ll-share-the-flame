using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoostSound : MonoBehaviour
{
	[Header("Sound FX")]
	public AudioClip SFX_Boost_Start;
	public AudioClip SFX_Boost_Mid;
	public AudioClip SFX_Boost_End;
	public AudioSource AS_Boost;

	bool playing;

    void Start()
    {
        AS_Boost.clip = SFX_Boost_Start;
		playing = false;
    }

    void Update()
    {
		if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (playing == false)
            {
                StartCoroutine("playBoostSFX");
            }
        }
		if (playing == true && Input.GetKeyUp(KeyCode.Alpha2)) {
			StopCoroutine("playBoostSFX");
			AS_Boost.Stop();
			AS_Boost.loop = false;
			AS_Boost.clip = SFX_Boost_End;
			AS_Boost.Play();
			playing = false;
		}
		//if (Input.GetKeyDown(KeyCode.T)) {
		//	StopCoroutine("playBoostSFX");
		//	AS_Boost.Stop();
		//	AS_Boost.loop = false;
		//	AS_Boost.clip = SFX_Boost_End;
		//	AS_Boost.Play();
		//	playing = false;
		//}
	}

    IEnumerator playBoostSFX()
    {
        playing = true;
		AS_Boost.clip = SFX_Boost_Start;
		// Play the sound
		AS_Boost.Play();
        yield return new WaitForSeconds(AS_Boost.clip.length);
		AS_Boost.clip = SFX_Boost_Mid;
		AS_Boost.loop = true;
		AS_Boost.Play();
    }

}
