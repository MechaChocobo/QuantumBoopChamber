using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class CaretakerCounter : MonoBehaviour {

	public Text caretakerCountLabel;
	
	// Use this for initialization
	void Start () {
		if(caretakerCountLabel != null) {
			caretakerCountLabel.text = "0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(caretakerCountLabel != null) {
			caretakerCountLabel.text = String.Format("{0,5:N0}", JobManager.instance.getCaretakerCount()) ;
		}
	}
	
	public void onModify (int val) {
		JobManager.instance.modifyCaretakerCount(val);
	}
}
