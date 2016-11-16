using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class LeftBallScript : MonoBehaviour {

	private AudioSource rollAudio;

	void Start(){
		rollAudio = GetComponent<AudioSource> ();
	}

	void OnCollisionEnter(Collision collision){
		if (collision.gameObject.CompareTag ("Right")) {
			rollAudio.Stop ();
		}
	}
}
