using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ShipManager : MonoBehaviour {

	// Singleton instance
	public static ShipManager instance {get; private set;} 
	// Use this for initialization
	void Start () {
	
	}

	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
	}


	// Update is called once per frame
	void Update () {
	
	}

	public void toggleRoom(String roomID) {
		GameObject room = GameObject.Find(roomID);
		if (room != null) {
			Button roomButton = room.GetComponentInChildren<Button>();
			Image roomImage = room.GetComponent<Image>();
			if (roomButton && roomImage) {
				Color newColor;
				bool newState;
				if (roomButton.IsInteractable()) {
					newState = false;
					newColor = new Color(0,0,0);
				}
				else {
					newState = true;
					newColor = new Color(0,0,0,0);
				}
				room.GetComponent<Image>().color = newColor;
				roomButton.interactable = newState;
			}
		}
		else
			Debug.Log("Room " + roomID + " doesn't exist.");
	}

	public void onRoomButtonClick(GameObject roomRoot) {
		//The name of the GameObject is the roomID
		if(roomRoot) {
			String id = roomRoot.name;
			Debug.Log("CLICKED DA BUTTON for Room " + id);
		}
	}
}
