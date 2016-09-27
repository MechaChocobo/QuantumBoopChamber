﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class BoopCounter : MonoBehaviour {

	public Text boopCountLabel;
	
	// Use this for initialization
	void Start () {
		if(boopCountLabel != null) {
			boopCountLabel.text = "0/0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(boopCountLabel != null) {
			boopCountLabel.text = String.Format("{0,5:N0}", GameController.instance.objResources.boop) + "/" + String.Format("{0,5:N0}", ResourceManager.instance.getBoopCap());
		}
	}

	public void BoopClick () {
		LogManager.instance.WriteToLog("You booped a changeling!");
		ResourceManager.instance.boopClick();
	}
}