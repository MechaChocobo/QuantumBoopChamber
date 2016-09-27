using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class FarmerCounter : MonoBehaviour {

	public Text farmerCountLabel;
	
	// Use this for initialization
	void Start () {
		if(farmerCountLabel != null) {
			farmerCountLabel.text = "0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(farmerCountLabel != null) {
			farmerCountLabel.text = String.Format("{0,5:N0}", JobManager.instance.getFarmerCount()) ;
		}
	}
	
	public void onModify (int val) {
		JobManager.instance.modifyFarmerCount(val);
	}
}
