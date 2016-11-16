using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RightBallScript : MonoBehaviour {

	private AudioSource crashAudio;

	void Start(){
		crashAudio = GetComponent<AudioSource> ();
	}

	void OnCollisionEnter(Collision collision){
		if (!collision.gameObject.CompareTag ("Floor")) {
			crashAudio.Play ();
		}
	}
}
