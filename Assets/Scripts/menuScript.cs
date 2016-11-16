using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class menuScript : MonoBehaviour {

	public Canvas quitMenu;
	public Button start;
	public Button help;
	public Button exit;

	public GameObject helpMenu;
	public GameObject startMenu;
	public GameObject aboutMenu;
	public GameObject selectMenu;

	// Use this for initialization
	void Start () {

		quitMenu = quitMenu.GetComponent<Canvas>();
		start = start.GetComponent<Button>();
		help = help.GetComponent<Button>();
		exit = exit.GetComponent<Button>();

		quitMenu.enabled = false;
		helpMenu.SetActive (false);
		aboutMenu.SetActive (false);
		selectMenu.SetActive (false);

	}

	public void StartPress() {
		selectMenu.SetActive (true);
		startMenu.SetActive(false);
		//SceneManager.LoadScene (1);
	}

	public void returnPress() {
		selectMenu.SetActive (false);
		startMenu.SetActive(true);
		//SceneManager.LoadScene (1);
	}

	public void CollisionPress() {
		SceneManager.LoadScene (1);
	}

	public void PendulumPress() {
		SceneManager.LoadScene (2);
	}


	public void HelpPress() {
		// show the menu help
		helpMenu.SetActive(true);
		startMenu.SetActive(false);
	}

	public void ExitPress() {
		quitMenu.enabled = true;
		start.enabled = false;
		help.enabled = false;
		exit.enabled = false;
	}

	public void NoPress() {
		quitMenu.enabled = false;
		start.enabled = true;
		help.enabled = true;
		exit.enabled = true;
	}

	public void YesPress() {
		Application.Quit();
	}


}
