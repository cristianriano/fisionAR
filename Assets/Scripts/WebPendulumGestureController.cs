using UnityEngine;
using LeapModels;

public class WebPendulumGestureController : MonoBehaviour {

	private WebGestureDetection connection;

	private float lastConnectionAttempt;
	public float retryConnectionTime = 0.5f;
	public int maxConnectionAttempts;
	private int connectionAttempts;

	public Speeds speeds;

	// Tiempo entre gestos (para q no coja dos veces el mismo gesto)
	public float timeBetweenGestures;
	// El id del ultimo Frame que se analizo
	private float lastFrame;
	// El tiempo en que se efectuo el ultimo gesto valido o detectado
	private float lastGestureTime;

	private PendulumController pendulumController;
	private AudioSource errorAudio;

	void Start(){
		connection = new WebGestureDetection ();
		connection.connectAsync ();

		lastConnectionAttempt = 0;
		connectionAttempts = 0;

		lastFrame = -1;
		lastGestureTime = 0;

		connection.swipeSpeed = speeds.swipe;
		connection.raiseOpenPalmSpeed = speeds.raiseOpenPalm;
		connection.dropOpenPalmSpeed = speeds.dropOpenPalm;
		connection.allHandSwipeSpeed = speeds.allHandSwipe;
		connection.allHandPinchSpeed = speeds.allHandPinch;
		connection.twoFingersSwipeSpeed = speeds.twoFingersSwipe;

		errorAudio = GetComponent<AudioSource> ();
		pendulumController = GetComponent<PendulumController> ();
	}

	void Update(){
		if (!connection.isConnected && ((Time.time - lastConnectionAttempt) > retryConnectionTime) && connectionAttempts < maxConnectionAttempts) {
			lastConnectionAttempt = Time.time;
			connectionAttempts++;
			connection.connectAsync ();
		}

		if (connection.isConnected) {
			// Verifica que no se analice dos veces la misma frame
			if (connection.lastFrame > lastFrame) {
				lastFrame = connection.lastFrame;

				if ((Time.time - lastGestureTime) > timeBetweenGestures) {
					errorAudio.Stop ();
					foreach (Hand hand in connection.hands) {
						//--------------------------------------------MANO IZQUIERDA---------------------------------------------
						if (hand.isLeft) {
							if (connection.checkRaiseOpenPalm (hand, raiseOpenPalmAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkDropOpenPalm (hand, dropOpenPalmAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkSwipe (hand, leftHandLeftSwipeAction, leftHandRightSwipeAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkTwoFingersSwipe (hand, twoFingersSwipeAction, twoFingersSwipeAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkAllHandPinch (hand, allHandPinchAction)) {
								lastGestureTime = Time.time;
								continue;
							} 
						}
						//--------------------------------------------MANO DERECHA---------------------------------------------
						else if (hand.isRight) {
							if (connection.checkRaiseOpenPalm (hand, raiseOpenPalmAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkDropOpenPalm (hand, dropOpenPalmAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkSwipe (hand, leftSwipeAction, rightSwipeAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkTwoFingersSwipe (hand, twoFingersSwipeAction, twoFingersSwipeAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkAllHandSwipe (hand, allHandSwipeLeftAction, allHandSwipeRightAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkAllHandPinch (hand, allHandPinchAction)) {
								lastGestureTime = Time.time;
								continue;
							}
						}
					}
				}
			}
		}
	}

	private void twoFingersSwipeAction(){
		Debug.Log ("Two fingers swipe ");
		pendulumController.resetAction ();
	}

	private void allHandPinchAction(){
		Debug.Log ("All hand pinch ");
		pendulumController.playAction ();
	}

	private void allHandSwipeLeftAction(){
		Debug.Log ("All hand swipe left");
	}

	private void allHandSwipeRightAction(){
		Debug.Log ("All hand swipe right");
	}

	private void dropOpenPalmAction(){
		Debug.Log ("Drop Open Palm");
		if (pendulumController.parameters.mass == pendulumController.limits.minMass)
			errorAudio.Play ();
		else
			pendulumController.changeMass (-5);

	}

	private void raiseOpenPalmAction(){
		Debug.Log ("Raise Open Palm");
		if (pendulumController.parameters.mass == pendulumController.limits.maxMass)
			errorAudio.Play ();
		else
			pendulumController.changeMass (5);
	}

	private void leftSwipeAction(){
		Debug.Log ("Right Hand Left Swipe");
		if (pendulumController.parameters.length == pendulumController.limits.minLength)
			errorAudio.Play ();
		else
			pendulumController.changeLength (-1);
	}

	private void rightSwipeAction(){
		Debug.Log ("Right Hand Right Swipe");
		if (pendulumController.parameters.length == pendulumController.limits.maxLength)
			errorAudio.Play ();
		else
			pendulumController.changeLength (1);
	}

	private void leftHandLeftSwipeAction(){
		Debug.Log ("Left Hand Left Swipe");
		if (pendulumController.parameters.gravityIndex == pendulumController.limits.minGravityIndex)
			errorAudio.Play ();
		else
			pendulumController.changeGravity (-1);
	}

	private void leftHandRightSwipeAction(){
		Debug.Log ("Left Hand Right Swipe");
		if (pendulumController.parameters.gravityIndex == pendulumController.limits.maxGravityIndex)
			errorAudio.Play ();
		else
			pendulumController.changeGravity (1);
	}

	public void disconect(){
		if (connection.isConnected)
			connection.disconect ();
	}
}
