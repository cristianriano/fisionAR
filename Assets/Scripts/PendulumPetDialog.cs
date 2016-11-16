using UnityEngine;
using System.Collections;
using Vuforia;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections.Generic;

public class PendulumPetDialog : MonoBehaviour, ITrackableEventHandler {

	public GUIStyle guiStyle;

	int i = 0;

	// Boton configuracion
	public GameObject parametersUI;
	//Textos 
	public GameObject displayTexts;
	//Boton play
	public Button playButton;
	//Boton slow
	public Button slowButton;
	//Boton reset
	public Button resetButton;
	//Boton config
	public Button configButton;


	Dictionary<int,int> positions = new Dictionary<int,int>();

	private TrackableBehaviour mTrackableBehaviour;

	private bool mShowGUIButton = false;
	private Rect mButtonRect = new Rect(50,50,120,60);

	string[] text = new string[12] {"Hola, Soy Pucky y voy a explicarte cada una de las opciones que te brinda esta aplicacion para que puedas disfrutar del maravilloso mundo de la fisica",
		"En esta oportunidad analizaremos el movimiento que describe un pendulo simple",
		"Adema de esto podrás realizar este experimento como si te encontraras en diferentes lugares de nuestro sistema solar!",
		"En la parte superiror encontraras un boton que te ayudara a modificar cada uno de los parametros del experimento como la longitud de la cuerda del pendulo y la masa de la esfera",
		"En esta opcion tambien podras escoger la gravedad en la que deseas realzar el experimento (por ejm Gravedad de la tierra)", 
		"Una vez enfoques el codigo del experimento aparecera una información sobre el pendulo como longitud, masa, tiempo, gravedad, estos datos te seran de utilidad para completar la practica de manera satifactoria",
		"Por ultimo estos botones te serviran para:",
		"Ver en camara lenta el experimento",
		"Reproducir o pausar la animacion",
		"Reiniciar el experimento",
		"Eso es todo. Diviertete experimentando! Hasta pronto!",
		"chao" };

	void Start () {



		positions.Add (0, Screen.height/4);
		positions.Add (1, Screen.height/4);
		positions.Add (2, Screen.height/4);
		positions.Add (3, Screen.height/4);
		positions.Add (4, Screen.height/4);
		positions.Add (5, Screen.height/2 - 20);
		positions.Add (6, Screen.height/2 - 20);
		positions.Add (7, Screen.height/2 - 20);
		positions.Add (8, Screen.height/2 - 20);
		positions.Add (9, Screen.height/2 - 20);
		positions.Add (10, Screen.height/2 - 20);
		positions.Add (11, Screen.height/2 - 20);

		i = 0;
		parametersUI.SetActive (false);
		displayTexts.SetActive (false);
		playButton.gameObject.SetActive (false);
		slowButton.gameObject.SetActive (false);
		resetButton.gameObject.SetActive (false);
		configButton.gameObject.SetActive (false);


		mTrackableBehaviour = GetComponent<TrackableBehaviour>();
		if (mTrackableBehaviour) {
			mTrackableBehaviour.RegisterTrackableEventHandler(this);
		}
	}

	public void OnTrackableStateChanged(
		TrackableBehaviour.Status previousStatus,
		TrackableBehaviour.Status newStatus)
	{
		if (newStatus == TrackableBehaviour.Status.DETECTED ||
			newStatus == TrackableBehaviour.Status.TRACKED ||
			newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
		{
			mShowGUIButton = true;
			i = 0;
			parametersUI.SetActive (false);
			displayTexts.SetActive (false);
			playButton.gameObject.SetActive (false);
			slowButton.gameObject.SetActive (false);
			resetButton.gameObject.SetActive (false);
			configButton.gameObject.SetActive (false);

		}
		else
		{			
			mShowGUIButton = false;
			//Muetra todo
			parametersUI.SetActive (false);
			displayTexts.SetActive (true);
			playButton.gameObject.SetActive (true);
			slowButton.gameObject.SetActive (true);
			resetButton.gameObject.SetActive (true);
			configButton.gameObject.SetActive (true);		
		}
	}

	// Se encarga de mostrar el panel para la edicion de los parametros
	public void showParameters(int op){

		switch (op) {
		case 0:
			//Esconde todo
			parametersUI.SetActive (false);
			displayTexts.SetActive (false);
			playButton.gameObject.SetActive (false);
			slowButton.gameObject.SetActive (false);
			resetButton.gameObject.SetActive (false);
			configButton.gameObject.SetActive (false);
			break;
		case 1:
			//no hace nada
			break;
		case 2:
			//no hace nada
			break;
		case 3:
			// MUESTRA Boton de config
			configButton.gameObject.SetActive (true);
			break;
		case 4:
			//Muestra el panel de los paramteros
			configButton.gameObject.SetActive (false);
			parametersUI.SetActive (true);
			break;
		case 5:
			//Muestra el panel de datos
			parametersUI.SetActive (false);
			displayTexts.SetActive (true);
			break;
		case 6:
			// Esconde el panel de datos
			displayTexts.SetActive (false);
			// Muestra los 3 botones de slow/play/reset
			playButton.gameObject.SetActive (true);
			slowButton.gameObject.SetActive (true);
			resetButton.gameObject.SetActive (true);
			break;
		case 7:
			// Muestra slow
			playButton.gameObject.SetActive (false);
			resetButton.gameObject.SetActive (false);
			break;
		case 8:
			// Muestra play
			playButton.gameObject.SetActive (true);
			slowButton.gameObject.SetActive (false);
			break;
		case 9:
			// Muestra reset
			resetButton.gameObject.SetActive (true);
			playButton.gameObject.SetActive (false);
			break;
		case 10:
			// Oculta reset
			resetButton.gameObject.SetActive (false);
			break;
		case 11:
			mShowGUIButton = false;
			//Muetra todo
			parametersUI.SetActive (false);
			displayTexts.SetActive (true);
			playButton.gameObject.SetActive (true);
			slowButton.gameObject.SetActive (true);
			resetButton.gameObject.SetActive (true);
			configButton.gameObject.SetActive (true);
			break;
		}

	}

	GUIStyle style;

	void OnGUI() {

		guiStyle = new GUIStyle (GUI.skin.label);
		guiStyle.alignment = TextAnchor.MiddleCenter;
		//guiStyle.normal.textColor = Color.white;
		guiStyle.fontSize = 35;

		if (mShowGUIButton) {
			if (GUI.Button (new Rect (0, Screen.height - positions [i], Screen.width, 100), "")) {
				i++;
				showParameters (i);

			}

			GUI.Label (new Rect (0, Screen.height - positions [i], Screen.width, 100), text [i], guiStyle);
		}

	}
}
