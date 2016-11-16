using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PendulumDisplayScript : MonoBehaviour {

	public Text displayText;
	public Text constantText;

	private GameObject ball;
	private GameObject rope;

	private Rigidbody rb;
	private float maxSpeed = 0f;
	private float speed;
	private float angle;
	private float maxAngle = 0f;

	private PendulumController controller;

	void Start(){
		ball = this.transform.Find ("Ball").gameObject;
		rope = this.transform.Find ("Rope").gameObject;

		controller = GetComponent<PendulumController> ();

		rb = ball.GetComponent<Rigidbody> ();
	}

	void Update(){
		string text;

		if (controller.start)
			speed = rb.velocity.magnitude;
		else
			speed = controller.ballVelocity.magnitude;

		angle = rope.transform.rotation.eulerAngles.z;

		if (angle > 90)
			angle = angle - 360;

		/*text = "CONSTANTES" + "\r\n" + "Masa: " + rb.mass + "\r\n" + 
			"Longitud: " + rope.transform.localScale.y + "\r\n" + "Gravedad: " + Mathf.Abs(Physics.gravity.y);*/
		text = "CONSTANTES" + "\r\n" + "Masa: " + rb.mass + "\r\n" + 
			"Longitud: " + rope.transform.localScale.y + "\r\n" + controller.parameters.gravityLabel;
		constantText.text = text;

		if (speed > maxSpeed)
			maxSpeed = speed;
		if (angle > maxAngle)
			maxAngle = angle;

		if (controller.getReset ()) {
			speed = 0;
			angle = 0;
			clearMax();
		}

		text = "DATOS" + "\r\n" + "Velocidad: " + speed.ToString("F2") + "\r\n" + "Max: " + maxSpeed.ToString("F2") +
			"\r\n" + "Angulo: " + angle.ToString("F2") + "\r\n" + "Max: " + maxAngle.ToString("F2");

		displayText.text = text;
	}

	public void clearMax(){
		maxSpeed = 0;
		maxAngle = 0;
	}

	//Funciones de botones exit y next
	public void NextPress() {
		//GetComponent<WebPendulumGestureController> ().disconect ();
		SceneManager.LoadScene (1);
	}

	public void ExitPress() {
		SceneManager.LoadScene (0);
		//GetComponent<WebCollisionGestureController> ().disconect ();
	}
}
