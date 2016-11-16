using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class PendulumParameters{
	public float mass = 5, length = 5, gravity = 9.8f;
	public int gravityIndex = 0;
	public string gravityLabel = "TIERRA";
}

[System.Serializable]
public class PendulumLimits{
	public float maxLength, minLength, minMass, maxMass, minGravityIndex, maxGravityIndex;
}

public class PendulumController : MonoBehaviour {

	public bool start;
	public bool slow;
	public bool reset;
	public bool configurate;

	private float ceroTime = 0f, pauseTime = 0f, playTime = 0f, totalPauseTime = 0f;

	private Rigidbody ballRb;
	private Rigidbody ropeRb;

	private GameObject rope;
	private GameObject ball;
	private GameObject ceiling;

	public Button playButton;

	public Text timeText;
	public float slowScale = 0.2f;
	public float startPush = 50;
	public GameObject parametersUI;
	public GameObject canvas;
	public PendulumParameters parameters;
	public PendulumLimits limits;

	private GameObject displayText;
	private GameObject constantsText;

	private Dictionary<int, float> gravities;

	public Vector3 ballVelocity;
	private Vector3 ropeVelocity;

	void Start () {
		start = false;
		slow = false;
		reset = true;
		configurate = false;

		rope = this.transform.Find ("Rope").gameObject;
		ball = this.transform.Find ("Ball").gameObject;
		ceiling = this.transform.Find ("Ceiling").gameObject;

		ballRb = ball.GetComponent<Rigidbody> ();
		ropeRb = rope.GetComponent<Rigidbody> ();

		displayText = canvas.transform.Find ("Display Text").gameObject;
		constantsText = canvas.transform.Find ("Constants Text").gameObject;

		// Se cargan las gravedades disponibles
		gravities = new Dictionary<int, float> ();
		gravities.Add(0, 9.8f);
		gravities.Add(1, 3.72f);
		gravities.Add(2, 8.82f);
		gravities.Add(3, 3.72f);
		gravities.Add(4, 23.13f);
		gravities.Add(5, 9.01f);
		gravities.Add(6, 8.72f);
		gravities.Add(7, 10.97f);

		ballVelocity = Vector3.zero;

		// Forzar landscape
		Screen.orientation = ScreenOrientation.Landscape;
		Time.timeScale = 1;
	}

	// Lectura de entradas
	void Update(){
		// Actualiza el reloj a mostrar
		updateTimeText ();
		// RESET
		// Tecla R
		if (Input.GetKeyDown (KeyCode.R)) {
			resetAction();
		} 
		// PLAY/PAUSE
		// Tecla P
		else if (Input.GetKeyDown (KeyCode.P)) {
			playAction();
		} 
		// CAMARA LENTA
		// Tecla S
		else if(Input.GetKeyDown (KeyCode.S)) {
			slowAction();
		}

		if(reset)
			GetComponent<PendulumDisplayScript> ().clearMax();
	}

	// Setea los parametros actuales guardados en el objeto publico parameters
	public void updateParameters(){
		clampParameters ();

		// Destruir los joint de la pelota y el techo
		Destroy (rope.GetComponent<HingeJoint> ());
		Destroy (ball.GetComponent<FixedJoint> ());

		resetPositions ();

		// Setear masa de la bola, posicion y tamaño de la cuerda
		ballRb.mass = parameters.mass;

		float x = this.transform.position.x;
		float y = this.transform.position.y;
		float z = this.transform.position.z;

		rope.transform.localPosition = new Vector3(x, y - parameters.length, z);
		rope.transform.localScale = new Vector3(0.05f, parameters.length, 0.05f);

		// Posicion de la pelota
		float ballPos = - ((parameters.length * 2) + (ball.transform.localScale.y)/2);
		ball.transform.localPosition = new Vector3(x, y + ballPos, z);

		// Setear gravedad
		Physics.gravity = new Vector3(0, - parameters.gravity, 0);

		// Crear el joint de la cuerda
		HingeJoint joint = rope.AddComponent<HingeJoint> ();
		joint.connectedBody = ceiling.GetComponent<Rigidbody> ();
		joint.anchor = new Vector3 (0, 1, 0);
		joint.axis = new Vector3 (0, 0, 1);
		JointLimits limits = joint.limits;
		limits.min = -10f;
		limits.max = 10f;
		joint.limits = limits;
		joint.useLimits = true;

		// Crear el joint de la pelota
		FixedJoint ballJoint = ball.AddComponent<FixedJoint> ();
		ballJoint.connectedBody = ropeRb;
	}

	// Limita los valores
	private void clampParameters(){
		parameters.mass = Mathf.Clamp (parameters.mass, limits.minMass, limits.maxMass);
		//parameters.gravity = Mathf.Clamp (parameters.gravity, 1, 20);
		parameters.length = Mathf.Clamp (parameters.length, limits.minLength, limits.maxLength);
	}

	private void resetPositions(){
		rope.transform.eulerAngles = Vector3.zero;
		ropeRb.velocity = Vector3.zero;
		ballRb.velocity = Vector3.zero;
	}

	public void playAction(){
		// Impulso inicial
		if(reset){
			ballRb.AddForce(-transform.right * startPush, ForceMode.Acceleration);
			reset = false;
		}
		// PAUSE
		if(start){
			pausePhysics ();
			start = false;
		}
		// PLAY
		else{
			start = true;
			playPhysics ();
			if (slow)
				Time.timeScale = slowScale;
		}
		updateButtonState ();
	}

	private void pausePhysics(){
		ballVelocity = ballRb.velocity;
		ropeVelocity = ropeRb.velocity;

		ballRb.Sleep ();
		ropeRb.Sleep ();
	}

	private void playPhysics(){
		ballRb.WakeUp ();
		ropeRb.WakeUp ();

		ballRb.velocity = ballVelocity;
		ropeRb.velocity = ropeVelocity;
	}

	private void updateButtonState(){
		if (start)
			playButton.image.sprite = Resources.Load<Sprite> ("Buttons/pause_button");
		else
			playButton.image.sprite = Resources.Load<Sprite> ("Buttons/play_button");
	}

	public void resetAction(){
		//ballVelocity = Vector3.zero;
		//ropeVelocity = Vector3.zero;

		reset = true;
		start = false;

		updateParameters();
		GetComponent<PendulumDisplayScript> ().clearMax ();
		updateButtonState ();
	}

	public void slowAction(){
		// Activar camara lenta
		if(slow){
			slow = false;
			Time.timeScale = 1;
		}
		//Desactivar
		else{
			slow = true;
			Time.timeScale = slowScale;
		}
	}

	// Se encarga de mostrar el panel para la edicion de los parametros
	public void showParametersUI(){
		// ESCONDE EL PANEL
		if (configurate) {
			configurate = false;
			parametersUI.SetActive (false);
			gameObject.SetActive (true);

			displayText.SetActive(true);
			timeText.gameObject.SetActive(true);
			constantsText.SetActive(true);
		} 
		// MUESTRA EL PANEL
		else {
			configurate = true;
			parametersUI.SetActive (true);
			gameObject.SetActive (false);

			displayText.SetActive(false);
			timeText.gameObject.SetActive(false);
			constantsText.SetActive(false);

			// Carga los valores actuales del experimento
			loadValues();
		}
	}

	private void loadValues(){
		parametersUI.transform.Find ("Mass Field").GetComponent<InputField> ().text = parameters.mass.ToString ();
		parametersUI.transform.Find ("Length Field").GetComponent<InputField> ().text = parameters.length.ToString ();
		parametersUI.transform.Find ("Gravity Box").GetComponent<Dropdown> ().value = parameters.gravityIndex;
	}

	// Implementa los valores asignados por medio de la interfaz
	public void setValues(){

		// Setear masa
		parameters.mass = float.Parse(parametersUI.transform.Find ("Mass Field").GetComponent<InputField> ().text);

		// Setear longitud de la cuerda
		parameters.length = float.Parse(parametersUI.transform.Find ("Length Field").GetComponent<InputField> ().text);

		// Setear gravedad
		parameters.gravityIndex = parametersUI.transform.Find ("Gravity Box").GetComponent<Dropdown> ().value;
		parameters.gravityLabel = getLabel (parameters.gravityIndex);
		parameters.gravity = gravities[parameters.gravityIndex];

		showParametersUI ();

		resetAction ();
	}

	// Actualiza el texto del reloj
	private void updateTimeText(){
		// Si se reseteo y no ha arrancado mantiene en cero
		if (reset) {
			// Marca el tiempo hasta el que estuvo en reset
			ceroTime = Time.time;
			timeText.text = "00:00:00";
			// Resetea banderas
			playTime = pauseTime = totalPauseTime = 0f;
		} else {
			// Aumentar reloj
			if (start) {
				// Resta el tiempo cero desde que empezo el experimento
				// Resta la suma total de todas las pausas
				float time = Time.time - ceroTime - totalPauseTime;
				int minutes, seconds;
				float miliseconds;

				// Luego de cada pausa suma el tiempo que esta duro
				if(pauseTime != 0f){
					totalPauseTime = totalPauseTime + (pauseTime - playTime);
					pauseTime = 0f;
				}

				miliseconds = (time*1000)%1000;
				seconds = (int) time%60;
				minutes = (int) time/60;
				// Da formato al texto
				timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00") + ":" + miliseconds.ToString("000");
				playTime = Time.time;
			}
			else{
				pauseTime = Time.time;
			}
		}
	}

	private string getLabel(int index){
		if (index == 0)
			return "TIERRA";
		else if (index == 1)
			return "MERCURIO";
		else if (index == 2)
			return "VENUS";
		else if (index == 3)
			return "MARTE";
		else if (index == 4)
			return "JUPITER";
		else if (index == 5)
			return "SATURNO";
		else if (index == 6)
			return "URANO";
		else 
			return "NEPTUNO";
	}

	public bool getReset(){
		return reset;
	}

	public void changeMass(float n){
		parameters.mass = parameters.mass + n;
		resetAction ();
	}

	public void changeLength(float n){
		parameters.length = parameters.length + n;
		resetAction ();
	}

	public void changeGravity(int n){
		parameters.gravityIndex = parameters.gravityIndex + n;
		parameters.gravityLabel = getLabel (parameters.gravityIndex);
		parameters.gravity = gravities[parameters.gravityIndex];
		resetAction ();
	}
}
