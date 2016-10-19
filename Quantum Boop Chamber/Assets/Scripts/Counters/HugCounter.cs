using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class HugCounter : MonoBehaviour {

	public Text hugCountLabel;
	
	// Use this for initialization
	void Start () {
		if(hugCountLabel != null) {
			hugCountLabel.text = "0/0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(hugCountLabel != null) {
			hugCountLabel.text = String.Format("{0,5:N0}", GameController.instance.objResources.hug) + "/" + String.Format("{0,5:N0}", ResourceManager.instance.getHugCap());
		}
	}

	public void BoopClick () {
		LogManager.instance.WriteToLog("You booped a changeling!");
		ResourceManager.instance.hugClick();
	}
}
