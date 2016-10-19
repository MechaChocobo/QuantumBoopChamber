using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class RoomControl : MonoBehaviour {

	private Animator anim;
	public Image image;
	public Text text;


	public Room[] roomList;
	private Dictionary<string, Room> rooms;

	public bool isOpen
	{
		get { return anim.GetBool("isOpen"); }
		set { anim.SetBool("isOpen", value);}
	}

	// Use this for initialization
	void Start () {

	}

	void Awake() {
		anim = GetComponent<Animator>();
		rooms = new Dictionary<string, Room>();

		foreach (Room r in roomList) {
			rooms.Add(r.roomId, r);
		}
	}

	// Update is called once per frame
	void Update () {

	}

	public void populateRoomData(string Id) {
		if (rooms.ContainsKey(Id)){
			Room r;
			rooms.TryGetValue(Id, out r);
			RoomState rs;
			if (r.roomStates.TryGetValue(r.currState, out rs))
			{
				image.sprite = rs.roomImage;
				text.text = rs.roomText;
			}
		}
	}
}
