using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class IdleWorkerCounter : MonoBehaviour {

	public Text idleCountLabel;
	
	// Use this for initialization
	void Start () {
		if(idleCountLabel != null) {
			idleCountLabel.text = "0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(idleCountLabel != null) {
			idleCountLabel.text = String.Format("{0,5:N0}", JobManager.instance.getIdleCount()) ;
		}
	}
}
