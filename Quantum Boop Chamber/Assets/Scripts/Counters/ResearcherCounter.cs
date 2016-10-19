using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class ResearcherCounter : MonoBehaviour {

	public Text researcherCountLabel;
	
	// Use this for initialization
	void Start () {
		if(researcherCountLabel != null) {
			researcherCountLabel.text = "0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(researcherCountLabel != null) {
			researcherCountLabel.text = String.Format("{0,5:N0}", JobManager.instance.getResearcherCount()) + "/" + String.Format("{0,5:N0}", JobManager.instance.getResearcherCap());
		}
	}
	
	public void onModify (int val) {
		JobManager.instance.modifyJob((int)Unit.Job.RESEARCHER, val);
	}
}
