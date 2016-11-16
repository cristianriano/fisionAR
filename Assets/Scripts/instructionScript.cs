using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class instructionScript : MonoBehaviour {

	public Button next;
	public Button exit;

	public Text textAR;
	public Text textNUI;

	public Image imageAR;
	public Image imageNUI;

	private int aux = 0;

	// Use this for initialization
	void Start () {

		next = next.GetComponent<Button>();
		exit = exit.GetComponent<Button>();

		textAR = textAR.GetComponent<Text>();
		textNUI = textNUI.GetComponent<Text>();

		imageAR = imageAR.GetComponent<Image>();
		imageNUI = imageNUI.GetComponent<Image>();

	}

	public void NextPress() {
		aux = (aux+1)%2;
		if (aux == 0) {
			textAR.enabled = true;	
			imageAR.enabled = true;	

			textNUI.enabled = false;	
			imageNUI.enabled = false;	

		} else {
			textAR.enabled = false;	
			imageAR.enabled = false;
			textNUI.enabled = true;	
			imageNUI.enabled = true;	
							
		}			
	}

	public void ExitPress() {
		SceneManager.LoadScene(0);
	}


}
