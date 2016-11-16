using System;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using Newtonsoft.Json.Linq;
using LeapModels;

public class WebGestureDetection{

	public readonly string URL = "ws://10.130.5.109:6437/v6.json";

	public readonly int NOT_CONNECTED = 0;
	public readonly int SYNC_MODE = 1;
	public readonly int ASYNC_MODE = 2;
	public int connectionMode;
	public bool isConnected = false;

	private WebSocket socket;

	public List<Hand> hands = null;
	public float lastFrame = -1;
	public string lastMessage = null;

	// Velocidad de los gestos
	public float swipeSpeed = 700;
	public float raiseOpenPalmSpeed = 300;
	public float dropOpenPalmSpeed = 800;
	public float allHandSwipeSpeed = 300;
	public float allHandPinchSpeed = 50;
	public float twoFingersSwipeSpeed = 700;

	public WebGestureDetection(){
		socket = new WebSocket (URL);
		socket.OnOpen += (sender, e) => {
			onOpenEvent(sender, e);
		};

		socket.OnMessage += (sender, e) => {
			onMessageEvent(sender, e);
		};

		socket.OnError += (sender, e) => {
			onErrorEvent(sender, e);
		};

		socket.OnClose += (sender, e) => {
			onCloseEvent(sender, e);
		};

		connectionMode = NOT_CONNECTED;
	}

	public void connect(){
		socket.Connect ();
		connectionMode = SYNC_MODE;
	}

	public void connectAsync(){
		socket.ConnectAsync ();
		connectionMode = ASYNC_MODE;
	}

	private void onOpenEvent(object sender, EventArgs e){
		Debug.Log ("Websocket abierto");
	}

	private void onMessageEvent(object sender, MessageEventArgs e){
		//Debug.Log (e.Data);
		lastMessage = e.Data;
		//processMessage (e.Data);
	}

	private void onErrorEvent(object sender, ErrorEventArgs e){
		Debug.Log ("Error: " + e.Message);
		Debug.Log ("Excepcion: " + e.Exception);
	}

	private void onCloseEvent(object sender, CloseEventArgs e){
		Debug.Log ("Conexion terminada");
		Debug.Log ("Razon: " + e.Reason);
		connectionMode = NOT_CONNECTED;
	}

	public void disconect(){
		if (connectionMode == SYNC_MODE)
			socket.Close ();
		else if (connectionMode == ASYNC_MODE)
			socket.CloseAsync ();
		connectionMode = NOT_CONNECTED;
	}

	public void processMessage(String s){
		if (s == null)
			return;
		JObject msj = JObject.Parse (s);
		// Setea parametros al conectarse por primera vez
		if (!isConnected) {
			if ((bool)msj ["event"] ["state"] ["streaming"]) {
				isConnected = true;
				sendMessage ("{\"optimizeHMD\": false}", reciveAction);
				sendMessage ("{\"enableGestures\": false}", reciveAction);
				sendMessage ("{\"background\": true}", reciveAction);
				sendMessage ("{\"focused\": true}", reciveAction);
				Debug.Log ("Streaming Conectado");
			}
			// Analiza los datos de un frame
		} else {
			// id del frame
			lastFrame = (float) msj["id"];

			// Procesar y acomodar la posicion de las manos
			JArray handsArray = (JArray)msj["hands"];
			hands = new List<Hand> ();

			Hand leftHand = null;
			Hand rightHand = null;
			Hand tmpHand = null;

			foreach (JObject handObject in handsArray) {
				tmpHand = handObject.ToObject<Hand> ();

				if (tmpHand.type.Equals ("right")) {
					rightHand = tmpHand;
					tmpHand.isRight = true;
					tmpHand.isLeft = false;
					tmpHand.fingers = new List<Pointable> ();
				} else {
					leftHand = tmpHand;
					tmpHand.isRight = false;
					tmpHand.isLeft = true;
					tmpHand.fingers = new List<Pointable> ();
				}

				hands.Add (tmpHand);
			}


			JArray fingersArray = (JArray)msj["pointables"];
			Pointable tmpFinger = null;

			foreach (JObject fingerObject in fingersArray) {
				tmpFinger = fingerObject.ToObject<Pointable> ();

				if(leftHand != null){
					if (tmpFinger.handId == leftHand.id)
						leftHand.fingers.Add (tmpFinger);
				}

				if(rightHand != null){
					if (tmpFinger.handId == rightHand.id)
						rightHand.fingers.Add (tmpFinger);
				}
					
			}

		}
	}

	private void reciveAction(bool completed){
		if (completed)
			Debug.Log ("Enviado");
		else
			Debug.Log ("No se pudo enviar");
	}

	public void sendMessage(String msj, System.Action<bool> action){
		if (connectionMode == SYNC_MODE)
			socket.Send (msj);
		else if (connectionMode == ASYNC_MODE)
			socket.SendAsync (msj, action);
	}


	//---------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------
	//------------------------------------------------------------GESTOS---------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------
	//---------------------------------------------------------------------------------------------------------------------------------


	//------------------------------------------------------------TWO FINGERS SWIPE----------------------------------------------------

	// Verifica un gesto de tipo two fingers swipe
	public bool checkTwoFingersSwipe(Hand hand, System.Action leftAction, System.Action rightAction){
		int n = 0;
		// Direccion del swipe
		bool leftSwipe = false;

		foreach (Pointable finger in hand.fingers) {

			// Verifica si el dedo es indice (1) o medio (2)
			if (finger.type == 1 || finger.type == 2) {

				float fingerVelocityX = finger.tipVelocity[0];

				// Detecta si el dedo esta estirado
				if (stretchedFinger (finger, -0.8f))
					n++;
				else
					return false;

				// Velocidad sobre eje X (izq y der) minima para ser considerado swipe
				if (fingerVelocityX < -twoFingersSwipeSpeed) {
					n++;
					leftSwipe = true;
				} else if (fingerVelocityX > twoFingersSwipeSpeed) {
					n++;
					leftSwipe = false;
				} 
				else
					return false;

			} 
			// Dedos anular y meñique
			else if (finger.type == 3 || finger.type == 4) {
				// Verifica que estos dedos esten flexionados (el pulgar no importa para el gesto)
				if (flexedFinger (finger))
					n++;
				else
					return false;
			}

		}
		// Una vez analizado cada dedo si cumple con las condiciones para considerarse un gesto valido
		if (n >= 6) {
			if (leftSwipe) {
				leftAction ();
			} else {
				rightAction ();
			}
			return true;
		}
		return false;
	}

	//------------------------------------------------------------ALL HAND PINCH-------------------------------------------------------

	// Verifica un gesto de tipo All Hand Pinch
	public bool checkAllHandPinch(Hand hand, System.Action action){
		int n = 0;

		foreach (Pointable finger in hand.fingers) {
			if (stretchedFinger (finger, -0.7f)) {
				n++;
			} else
				return false;

			if (finger.type == 0) {
				if (finger.tipVelocity[1] > allHandPinchSpeed)
					n++;
				else 
					return false;
			} else if (finger.type == 1 || finger.type == 2 || finger.type == 3){
				if (finger.tipVelocity[1] < -(allHandPinchSpeed))
					n++;
				else
					return false;
			}
		}

		if (n >= 9) {
			action ();
			return true;
		}

		return false;
	}

	//------------------------------------------------------------ALL HAND SWIPE-------------------------------------------------------

	// Verifica un gesto de tipo All Hand Swipe
	public bool checkAllHandSwipe(Hand hand, System.Action leftAction, System.Action rightAction){
		int n = 0;
		bool leftSwipe = false;

		foreach (Pointable finger in hand.fingers) {
			if (finger.type == 0)
				continue;
			else {
				if (stretchedFinger (finger, -0.5f)) {
					n++;
				} else
					return false;
			}
		}

		float leftAdjust = 0;
		float rightAdjust = 0;
		// La mano debe estar de lado
		if (hand.isLeft) {
			if (hand.palmNormal[0] > 0.9) {
				n++;
				leftAdjust = 300;
			}
			else
				return false;
		} 
		else {
			if (hand.palmNormal[0] < -0.9) {
				n++;
				rightAdjust = 300;
			}
			else
				return false;
		}

		// Verifica velocidad minima para ser considerado un All Hand Swipe
		if (hand.palmVelocity[0] > (allHandSwipeSpeed + rightAdjust)) {
			n++;
			leftSwipe = false;
		} else if (hand.palmVelocity[0] < -(allHandSwipeSpeed + leftAdjust)) {
			n++;
			leftSwipe = true;
		} else
			return false;

		if (hand.palmVelocity[1] < 100 && hand.palmVelocity[2] < 100)
			n++;

		if (n >= 7) {
			if (leftSwipe)
				leftAction ();
			else
				rightAction ();
			return true;
		}

		return false;
	}

	//------------------------------------------------------------SWIPE----------------------------------------------------------------

	// Verifica un gesto de tipo swipe
	public bool checkSwipe(Hand hand, System.Action leftAction, System.Action rightAction){
		// Condiciones para que el gesto efectuado sea considerado swipe n = 5
		int n = 0;
		// Direccion del swipe
		bool leftSwipe = false;

		foreach (Pointable finger in hand.fingers) {

			// Verifica si el dedo es indice
			if (finger.type == 1) {

				float fingerVelocityX = finger.tipVelocity[0];

				// Detecta si el dedo esta estirado
				if (stretchedFinger (finger, -0.8f))
					n++;
				else
					return false;

				// Velocidad sobre eje X (izq y der) minima para ser considerado swipe
				if (fingerVelocityX < -swipeSpeed) {
					n++;
					leftSwipe = true;
				} else if (fingerVelocityX > swipeSpeed) {
					n++;
					leftSwipe = false;
				} 
				else
					return false;

			} 
			// Dedos medio, anular y meñique
			else if (finger.type == 2 || finger.type == 3 || finger.type == 4){
				// Verifica que estos dedos esten flexionados (el pulgar no importa para el gesto)
				if (flexedFinger (finger))
					n++;
				else
					return false;
			}

		}
		// Una vez analizado cada dedo si cumple con las condiciones para considerarse un gesto valido
		if (n >= 5) {
			if (leftSwipe) {
				leftAction ();
			} else {
				rightAction ();
			}
			return true;
		}
		return false;
	}

	//------------------------------------------------------------DROP OPEN PALM-------------------------------------------------------

	// Verifica un gesto de tipo Drop Open Palm
	public bool checkDropOpenPalm(Hand hand, System.Action action){
		int n = 0;

		// Palma de la mano apuntando hacia abajo
		if (hand.palmNormal[1] < -0.9)
			n++;
		else
			return false;

		// Verifica que todos los dedos esten extendidos
		foreach (Pointable finger in hand.fingers) {
			if (finger.type == 0)
				continue;
			else {
				if (stretchedFinger (finger, -0.8f)) {
					n++;
				} else
					return false;
			}
		}

		if (hand.palmVelocity[1] < -dropOpenPalmSpeed)
			n++;



		// Gesto cumple con las condiciones de un Drop Open Palm
		if (n >= 6) {
			action ();
			return true;
		}

		return false;
	}

	//------------------------------------------------------------RAISE OPEN PALM-------------------------------------------------------

	// Verifica un gesto de tipo Raise Open Palm
	public bool checkRaiseOpenPalm(Hand hand, System.Action action){
		int n = 0;

		// Verifica palma abierta hacia arriba
		if (hand.palmNormal[1] > 0.9) 
			n++;
		else
			return false;

		// Verifica que todos los dedos esten extendidos
		foreach (Pointable finger in hand.fingers) {
			if (finger.type == 0)
				continue;
			else {
				if (stretchedFinger (finger, -0.58f)) {
					n++;
				} else
					return false;
			}
		}

		if (hand.palmVelocity[1] > raiseOpenPalmSpeed)
			n++;

		// Gesto cumple con las condiciones de un Raise Open Palm
		if (n >= 6) {
			action ();
			return true;
		}

		return false;
	}

	//------------------------------------------------------------STRETCHED/FLEXED FINGER---------------------------------------------

	// Verifica si el dedo esta flexionado (a lo largo del eje Z)
	public bool flexedFinger(Pointable finger){
		float fingerDirectionZ = finger.direction[2];
		if (fingerDirectionZ > 0.48)
			return true;
		return false;
	}

	// Verifica si el dedo esta estirado en comparacion con un minimo, pues depende de la posicion el dedo se ve mas estirado (a lo largo del eje Z)
	public bool stretchedFinger(Pointable finger, float min){
		float fingerDirectionZ = finger.direction[2];
		if (fingerDirectionZ < min)
			return true;
		return false;
	}
		
}

