using UnityEngine;
using System.Collections.Generic;
using LeapModels;

// Clase q contiene las velocidades minimas para los gestos
[System.Serializable]
public class Speeds{
	// Velocidad de los gestos
	public float swipe;
	public float raiseOpenPalm;
	public float dropOpenPalm;
	public float allHandSwipe;
	public float allHandPinch;
	public float twoFingersSwipe;
}

[RequireComponent(typeof(AudioSource))]
public class WebCollisionGestureController : MonoBehaviour{

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

	private CollisionController collisionController;
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
		collisionController = GetComponent<CollisionController> ();
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
					connection.processMessage (connection.lastMessage);
					foreach (Hand hand in connection.hands) {
						//--------------------------------------------MANO IZQUIERDA---------------------------------------------
						if (hand.isLeft) {
							if (connection.checkRaiseOpenPalm (hand, leftHandRaiseOpenPalmAction)) {
								lastGestureTime = Time.time;
								continue;
							} else if (connection.checkDropOpenPalm (hand, leftHandDropOpenPalmAction)) {
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

	private void leftHandRaiseOpenPalmAction(){
		Debug.Log ("Raise Open Palm Left Hand");
		if (collisionController.parameters.leftMass == collisionController.limits.maxMass)
			errorAudio.Play ();
		else
			collisionController.changeLeftMass (1);
	}

	private void leftHandDropOpenPalmAction(){
		Debug.Log ("Drop Open Palm Left Hand");
		if (collisionController.parameters.leftMass == collisionController.limits.minMass)
			errorAudio.Play ();
		else
			collisionController.changeLeftMass (-1);
	}

	private void twoFingersSwipeAction(){
		Debug.Log ("Two fingers swipe ");
		collisionController.resetAction ();
	}

	private void allHandPinchAction(){
		Debug.Log ("All hand pinch ");
		collisionController.playAction ();
	}

	private void allHandSwipeLeftAction(){
		Debug.Log ("All hand swipe left");
	}

	private void allHandSwipeRightAction(){
		Debug.Log ("All hand swipe right");
	}

	private void dropOpenPalmAction(){
		Debug.Log ("Drop Open Palm");
		if (collisionController.parameters.rightMass == collisionController.limits.minMass)
			errorAudio.Play ();
		else
			collisionController.changeRightMass (-1);

	}

	private void raiseOpenPalmAction(){
		Debug.Log ("Raise Open Palm");
		if (collisionController.parameters.rightMass == collisionController.limits.maxMass)
			errorAudio.Play ();
		else
			collisionController.changeRightMass (1);
	}

	private void leftSwipeAction(){
		Debug.Log ("Left Swipe");
		if (collisionController.parameters.height == collisionController.limits.maxHight)
			errorAudio.Play ();
		else
			collisionController.changeHeight (1);
	}

	private void rightSwipeAction(){
		Debug.Log ("Right Swipe");
		if (collisionController.parameters.height == collisionController.limits.minHeight)
			errorAudio.Play ();
		else
			collisionController.changeHeight (-1);
	}

	public void disconect(){
		if (connection.isConnected)
			connection.disconect ();
	}
}

