using UnityEngine;
using UnityEngine.UI;
using System.Collections;

// Clase q contiene los parametros editables de cada lado
[System.Serializable]
public class CollisionParameters{
	public float leftMass, rightMass, height;
}

[System.Serializable]
public class CollisionLimits{
	public float maxHight, minHeight, minMass, maxMass;
}

[RequireComponent(typeof(AudioSource))]
public class CollisionController : MonoBehaviour {

	// Banderas de start, slow motion y reset
	public bool start;
	public bool slow;
	public bool reset;
	public bool configurate;

	// Tipo de colision
	private bool elastic;

	// Vincula al rigidbody de las pelotas
	private Rigidbody leftBall;
	private Rigidbody rightBall;

	// Ratio de la camara lenta
	public float slowSpeed = 0.2f;

	// Cambiar el estado del boton de play/pause
	public Button playButton;

	// La interfaz de edicion
	public GameObject parametersUI;
	public GameObject displayTexts;

	// Clase con los parametros del experimento
	public CollisionParameters parameters;
	// Clase con los limites
	public CollisionLimits limits;

	private GameObject floor;
	private AudioSource audioRoll;

	//Vectores para almocenar el movimiento de las cosas antes de pausar
	public Vector3 leftBallVelocity = Vector3.zero;
	public Vector3 rightBallVelocity = Vector3.zero;

	// Inicializar variables
	void Start () {

		// Pelotas
		leftBall = this.transform.Find ("Left Ball").gameObject.GetComponent<Rigidbody> ();
		rightBall = this.transform.Find ("Right Ball").gameObject.GetComponent<Rigidbody> ();

		floor = this.transform.Find ("Floor").gameObject;

		start = false;
		slow = false;
		reset = true;
		configurate = false;

		elastic = true;

		audioRoll = leftBall.gameObject.GetComponent<AudioSource> ();

		updateParameters ();

		// Forzar landscape
		Screen.orientation = ScreenOrientation.Landscape;
		Time.timeScale = 1;

	}

	void Update () {
		if (reset) {
			resetAction ();
		}

		// RESET
		// Tecla R o 2 toques simultaneos
		if (Input.GetKeyDown (KeyCode.R)) {
			resetAction();
		} 
		// PLAY/PAUSE
		// Tecla P o un toque
		else if (Input.GetKeyDown (KeyCode.P)){
			playAction();
		} 
		// SLOW MOTION
		// Tecla S
		else if (Input.GetKeyDown (KeyCode.S)) {
			slowAction ();
		}

		floor.GetComponent<Renderer> ().enabled = false;
	}

	// Dejan caer la pelota
	private void startCollision(){
		leftBall.constraints = RigidbodyConstraints.None;
		rightBall.constraints = RigidbodyConstraints.None;
	}

	// Acomoda las pelotas en sus posiciones iniciales y congela su movimiento
	private void setCollision(){
		// Acomoda la posicion de las pelotas
		leftBall.transform.localPosition = new Vector3 (getX(parameters.height), parameters.height, 0);
		rightBall.transform.localPosition = new Vector3(0, 1, 0);

		leftBall.transform.rotation = Quaternion.identity;
		rightBall.transform.rotation = Quaternion.identity;

		leftBall.constraints = RigidbodyConstraints.FreezeAll;
		rightBall.constraints = RigidbodyConstraints.FreezeAll;
	}

	// Setea parametros
	private void updateParameters(){
		// Limita parametros
		clamp ();

		// Setea masas
		leftBall.mass = parameters.leftMass;
		rightBall.mass = parameters.rightMass;

		float bounciness = 0;
		if (elastic)
			bounciness = 1;

		leftBall.gameObject.GetComponent<SphereCollider> ().material.bounciness = bounciness;
		rightBall.gameObject.GetComponent<SphereCollider> ().material.bounciness = bounciness;

		setCollision();
	}

	// Limita los parametros configurables
	private void clamp(){
		// Masas
		parameters.leftMass = Mathf.Clamp (parameters.leftMass, limits.minMass, limits.maxMass);
		parameters.rightMass = Mathf.Clamp (parameters.rightMass, limits.minMass, limits.maxMass);

		// Altura
		parameters.height = Mathf.Clamp(parameters.height, limits.minHeight, limits.maxHight);
	}

	public void playAction(){
		if(reset){
			updateParameters();
			startCollision ();
			reset = false;
			audioRoll.Play ();
		}
		if (start) {
			pausePhysics ();
			start = false;
			audioRoll.Pause ();
		} else {
			audioRoll.Play ();
			start = true;
			playPhysics ();
			if (slow)
				Time.timeScale = slowSpeed;
		}
		updateButtonState ();
	}

	private void pausePhysics(){
		leftBallVelocity = leftBall.velocity;
		rightBallVelocity = rightBall.velocity;

		leftBall.Sleep ();
		rightBall.Sleep ();
	}

	private void playPhysics(){
		leftBall.WakeUp ();
		rightBall.WakeUp ();

		leftBall.velocity = leftBallVelocity;
		rightBall.velocity = rightBallVelocity;
	}

	private void updateButtonState(){
		if (start)
			playButton.image.sprite = Resources.Load<Sprite> ("Buttons/pause_button");
		else
			playButton.image.sprite = Resources.Load<Sprite> ("Buttons/play_button");
	}

	public void slowAction(){
		if(slow){
			slow = false;
			Time.timeScale = 1;
		}
		else{
			slow = true;
			Time.timeScale = slowSpeed;
		}
	}

	public void resetAction(){
		leftBallVelocity = rightBallVelocity = Vector3.zero;
		updateParameters ();
		clearMax ();
		start = false;
		reset = true;
		updateButtonState ();
		audioRoll.Stop ();
	}

	// Posicion en X de la pelota izquierda con base en su altura
	private float getX(float y){
		if (y == 6)
			return -18.4f;
		else if (y == 7)
			return -19.2f;
		else if (y == 8)
			return -20f;
		else if (y == 9)
			return -20.7f;
		else if (y == 10)
			return -21.3f;
		else if (y == 11)
			return -21.7f;
		else if (y == 12)
			return -22f;
		else
			return -22.5f;
	}


	// Se encarga de mostrar el panel para la edicion de los parametros
	public void showParametersUI(){
		// ESCONDE EL PANEL
		if (configurate) {
			configurate = false;
			parametersUI.SetActive (false);
			gameObject.SetActive (true);
			displayTexts.SetActive(true);
		} 
		// MUESTRA EL PANEL
		else {
			configurate = true;
			parametersUI.SetActive (true);
			gameObject.SetActive (false);
			displayTexts.SetActive(false);

			// Carga los valores actuales del experimento
			loadValues();
		}
	}

	public void setValues(){
		// Setear masas
		parameters.leftMass = float.Parse(parametersUI.transform.Find ("Left Mass Field").GetComponent<InputField> ().text);
		parameters.rightMass = float.Parse(parametersUI.transform.Find ("Right Mass Field").GetComponent<InputField> ().text);

		//Setear altura
		parameters.height = parametersUI.transform.Find ("Height Box").GetComponent<Dropdown> ().value + 6;

		// Setear tipo de colision
		elastic = parametersUI.transform.Find ("Elastic Checkbox").GetComponent<Toggle> ().isOn;

		showParametersUI ();

		resetAction ();
	}

	private void loadValues(){
		parametersUI.transform.Find ("Left Mass Field").GetComponent<InputField> ().text = parameters.leftMass.ToString();
		parametersUI.transform.Find ("Right Mass Field").GetComponent<InputField> ().text = parameters.rightMass.ToString();

		parametersUI.transform.Find ("Elastic Checkbox").GetComponent<Toggle> ().isOn = elastic;

		// Altura actual
		int index = Mathf.RoundToInt(parameters.height) - 6;
		// Menu de opciones
		Dropdown box = parametersUI.transform.Find ("Height Box").GetComponent<Dropdown> ();
		box.value = index;
	}

	public void changeLeftMass(float n){
		parameters.leftMass = parameters.leftMass + n;
		resetAction ();
	}

	public void changeRightMass(float n){
		parameters.rightMass = parameters.rightMass + n;
		resetAction ();
	}

	public void changeHeight(float n){
		parameters.height = parameters.height + n;
		resetAction ();
	}

	private void clearMax(){
		GetComponent<CollisionDisplayScript> ().clearMax ();
	}
}
