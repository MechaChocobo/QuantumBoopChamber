using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class PonyCounter : MonoBehaviour {

	public Text ponyCountLabel;
	
	// Use this for initialization
	void Start () {
		if(ponyCountLabel != null) {
			ponyCountLabel.text = "0/0";
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(ponyCountLabel != null) {
			ponyCountLabel.text = String.Format("{0,5:N0}", ResourceManager.instance.getPonyPopulation()) + "/" + String.Format("{0,5:N0}", ResourceManager.instance.getPonyPopulationCap());
		}
	}
}
