using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FoodCounter : MonoBehaviour {

	public Text foodCountLabel;
	
	// Use this for initialization
	void Start () {
		if(foodCountLabel != null) {
			foodCountLabel.text = "0/0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(foodCountLabel != null) {
			foodCountLabel.text = String.Format("{0,5:N0}", ResourceTracker.instance.getFood()) + "/" + String.Format("{0,5:N0}", ResourceTracker.instance.getFoodCap());
		}
	}
}
