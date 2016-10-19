using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu()]
public class Room : ScriptableObject {

	public string roomId; //Room's ID
	public string currState; //Current State of Room
	public List<string> linkedRooms; //List of Rooms linked to this room

	[System.Serializable]
	public struct State {
		public string stateName;
		public RoomState stateData;
	}

	public State[] data;

	public Dictionary<string, RoomState> roomStates;

	void Awake() {

	}

	void OnEnable() {
		roomStates = new Dictionary<string, RoomState>();
		foreach (State s in data){
			roomStates.Add(s.stateName, s.stateData);
		}
	}
	//Awake
	//OnDestroy
	//OnDisable
	//OnEnable
}

[System.Serializable]
public class RoomState {
	public Sprite roomImage; //Image is the reference to the room’s picture
	public string roomText; //Display Text tells the room text box what to display
	//Worker Slots are for individual workers to be assigned to the room.
	//Production Modifiers detail what the room does as far as base resources go
	//Button List defines the options the player has (such as buying upgrades or whatnot)
}
