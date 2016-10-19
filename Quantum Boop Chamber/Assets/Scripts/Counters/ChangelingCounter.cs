using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ChangelingCounter : MonoBehaviour {

	public Text changelingCountLabel;
	
	// Use this for initialization
	void Start () {
		if(changelingCountLabel != null) {
			changelingCountLabel.text = "0/0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(changelingCountLabel != null) {
			changelingCountLabel.text = String.Format("{0,5:N0}", UnitManager.instance.changelingPop) + "/" + String.Format("{0,5:N0}", UnitManager.instance.changelingPopCapacity);
		}
	}
}
