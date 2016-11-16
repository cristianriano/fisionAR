using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class CollisionDisplayScript : MonoBehaviour {

	public Text leftParametersText;
	public Text rightParametersText;

	private Rigidbody leftRb;
	private Rigidbody rightRb;

	private float leftSpeed, rightSpeed;
	private float leftMass, rightMass;
	private float leftMaxSpeed, rightMaxSpeed;
	private float height;

	private CollisionController collisionController;

	void Start(){
		// Rigid body de las pelota
		leftRb = this.transform.Find ("Left Ball").gameObject.GetComponent<Rigidbody> ();;
		rightRb = this.transform.Find ("Right Ball").gameObject.GetComponent<Rigidbody> ();

		// Setea la velocidad maxima a cero
		leftMaxSpeed = rightMaxSpeed = 0;

		collisionController = GetComponent<CollisionController> ();
	}

	void Update(){
		string tmp;

		// Altura a la que arranca la pelota
		height = collisionController.parameters.height;

		// Magnitud de las velocidades
		if (collisionController.start) {
			leftSpeed = leftRb.velocity.magnitude;
			rightSpeed = rightRb.velocity.magnitude;
		} else {
			leftSpeed = collisionController.leftBallVelocity.magnitude;
			rightSpeed = collisionController.rightBallVelocity.magnitude;
		}


		// Masa de las pelotas
		leftMass = leftRb.mass;
		rightMass = rightRb.mass;

		// Almacena la velocidad maxima experimentada por las pelotas
		if (leftSpeed > leftMaxSpeed) {
			leftMaxSpeed = leftSpeed;
		}
		if (rightSpeed > rightMaxSpeed) {
			rightMaxSpeed = rightSpeed;
		}

		// Acomoda el texto
		// Izquiera
		tmp = "Vinotinto" + "\r\n" + "Masa: " + leftMass.ToString() + "\r\n" + "Altura: " + height.ToString("F2") + "\r\n"
			+"Velocidad: " + leftSpeed.ToString("F2") + "\r\n" + "Max: " + leftMaxSpeed.ToString("F2");
		leftParametersText.text = tmp;

		// Derecha
		tmp = "Amarilla" + "\r\n" + "Masa: " + rightMass.ToString() + "\r\n" + "Velocidad: " + rightSpeed.ToString("F2") + "\r\n" 
			+ "Max: " + rightMaxSpeed.ToString("F2");
		rightParametersText.text = tmp;
	}

	public void clearMax(){
		GetComponent<WebCollisionGestureController> ().disconect ();
		leftMaxSpeed = rightMaxSpeed = 0;
	}


	//Funciones de botones exit y next
	public void NextPress() {
		GetComponent<WebCollisionGestureController> ().disconect ();
		SceneManager.LoadScene (2);
	}

	public void ExitPress() {
		SceneManager.LoadScene (0);
	}
}
