using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;

public class LogManager : MonoBehaviour {

	public Text logText;

	// Singleton instance
	public static LogManager instance {get; private set;}

	// Use this for initialization
	void Start () {
		if(logText != null) {
			WriteToLog("Welcome to the Emerald Hive. Or what's left of it, at least.\nThis is a prototype for the features we plan to make for the actual game.\n"); 
			AppendToLog("For now, there isn't much to do, but go ahead and fiddle with the UI.\n\nIf anything is terribly broken, it is almost certainly the Chocobo's fault. Go yell at him. Otherwise he won't know to fix it.");
		}
	}

	/*
	// Update is called once per frame
	void Update () {
		if(logText != null) {
			String strLog = "Current Production/Consumption Rates\n";
			ResourceManager rTracker = ResourceManager.instance;
			strLog += "(rates are per second)\n";
			strLog = strLog + rTracker.getPonyPopulation() + " ponies are consuming " + rTracker.getFoodConsumption() + " food, and producing " + rTracker.getBoopProduction() + " boops.\n";
			strLog = strLog + rTracker.getChangelingPopulation() + " changelings are consuming " + rTracker.getBoopConsumption() + " boops, and producing " + rTracker.getFoodProduction() + " food.\n";			
			
			logText.text = strLog;
		}
	}
	*/

	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
	}

	//Clears text box and inserts new text
	public void WriteToLog(String strText) {
		logText.text = strText;
	}

	//Adds text to text box. It is the Appending caller's responsibility to ensure a newline.
	public void AppendToLog(String strText) {
		logText.text += strText;
	}
}
