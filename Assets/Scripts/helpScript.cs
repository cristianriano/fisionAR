using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class helpScript : MonoBehaviour {

	public Button pendulum;
	public Button collision;
	public Button instruction;
	public Button about;
	public Button exit;

	public GameObject helpMenu;
	public GameObject startMenu;
	public GameObject aboutMenu;

	// Use this for initialization
	void Start () {


		pendulum = pendulum.GetComponent<Button>();
		collision = collision.GetComponent<Button>();
		instruction = instruction.GetComponent<Button>();
		about = about.GetComponent<Button>();
		exit = exit.GetComponent<Button>();

		helpMenu.SetActive(true);
		aboutMenu.SetActive (false);
		//startMenu.SetActive(faltrue);

	}

	public void CollisionPress() {
		Handheld.PlayFullScreenMovie ("Colision.mp4", Color.black, FullScreenMovieControlMode.Minimal);

	}

	public void PendulumPress() {
		Handheld.PlayFullScreenMovie ("Pendulo.mp4", Color.black, FullScreenMovieControlMode.Minimal);
		//Handheld.PlayFullScreenMovie ("video.mp4", Color.black, FullScreenMovieControlMode.CancelOnInput);
		//print ("Move Scene");
	}


	public void InstructionPress() {
		SceneManager.LoadScene (3);
	}

	public void AboutPress() {
		helpMenu.SetActive(false);
		aboutMenu.SetActive(true);
	}

	public void ExitAboutPress() {
		helpMenu.SetActive(true);
		aboutMenu.SetActive(false);
	}

	public void ExitHelpPress() {
		helpMenu.SetActive(false);
		startMenu.SetActive(true);
	}


}
