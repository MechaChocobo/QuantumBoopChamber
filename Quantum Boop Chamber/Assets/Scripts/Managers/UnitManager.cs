using UnityEngine;
using System.Collections;

public class UnitManager : MonoBehaviour {
	public static UnitManager instance {get; private set;} 

	// Use this for initialization
	void Start () {
	//	GameController.instance.lstUnits.Add(new Unit());
	}

	void Awake() {
		instance = this;
	}

	// Update is called once per frame
	void Update () {

	}
}
