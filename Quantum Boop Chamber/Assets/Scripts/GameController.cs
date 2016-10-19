using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Polenter.Serialization;

public class GameController : MonoBehaviour {

	// Singleton instance
	public static GameController instance {get; private set;}
	public BaseResources objResources;
	public List<Unit> lstUnits;

	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
	}

	void Start() {
		objResources = new BaseResources();
		lstUnits = new List<Unit>();
		CreateStartingUnits();	
	}

	void CreateStartingUnits(){ 
		//Create Starting Units using current debug counts
		int startingWorkers = 150;
		int startingHarvesters = 90;
		int startingScouts = 60;
		int startingUni = 2, startingEarth = 2, startingPeg = 2;

		int startingFarmers = 30, startingCaretakers = 1;

		int itr;
		for (itr = 0; itr < startingWorkers; itr++){
			Changeling newLing = new Changeling((int)Unit.ChangelingSubSpecies.WORKER);
			if(itr < startingFarmers) {
				newLing.currentJob = (int)Unit.Job.FARMER;
			}
			lstUnits.Add(newLing);
		}
		for (itr = 0; itr < startingHarvesters; itr++){
			Changeling newLing = new Changeling((int)Unit.ChangelingSubSpecies.HARVESTER);
			if(itr < startingCaretakers) {
				newLing.currentJob = (int)Unit.Job.CARETAKER;
			}
			lstUnits.Add(newLing);
		}
		for (itr = 0; itr < startingScouts; itr++){
			lstUnits.Add(new Changeling((int)Unit.ChangelingSubSpecies.SCOUT));
		}
		for (itr = 0; itr < startingUni; itr++){
			lstUnits.Add(new Pony((int)Unit.PonySubSpecies.UNICORN));
		}
		for (itr = 0; itr < startingEarth; itr++){
			lstUnits.Add(new Pony((int)Unit.PonySubSpecies.EARTH));
		}
		for (itr = 0; itr < startingPeg; itr++){
			lstUnits.Add(new Pony((int)Unit.PonySubSpecies.PEGASUS));
		}
	}

	public void Save() {
		SharpSerializer serializer = new SharpSerializer(); //Currently outputting XML. Pass true to use bin mode
		Dictionary<String, System.Object> saveData = new Dictionary<String, System.Object>();
		saveData.Add("resources",objResources);
		saveData.Add("units",lstUnits);
		//Get all managers
		serializer.Serialize(saveData,"save.xml");
	}

	public void Load() {
		SharpSerializer serializer = new SharpSerializer(); //Currently outputting XML. Pass true to use bin mode

		Dictionary<String, System.Object> saveData = (Dictionary<String,System.Object>)serializer.Deserialize("save.xml");
		objResources = (BaseResources)saveData["resources"];
		lstUnits = (List<Unit>)saveData["units"];
		JobManager.instance.updateJobCount();
		ResourceManager.instance.reCalcHugDelta();
		ResourceManager.instance.reCalcFoodDelta();
	}
}

public class BaseResources {
	// Gameplay Constants
	private static float STARTING_FOOD = 50.0f;
	private static float STARTING_HUGS = 10.0f;

	public float hug {get; set;}
	public float food {get; set;}

	public BaseResources(){
		hug = STARTING_HUGS;
		food = STARTING_FOOD;
	}
}

class Ship {
	
}

class Units {

}

public class Unit {
	public enum Species {PONY, CHANGELING};
	public enum PonySubSpecies {UNICORN, EARTH, PEGASUS};
	public enum ChangelingSubSpecies {DRONE, WORKER, HARVESTER, SCOUT, SOLDIER, ROYAL, QUEEN};

	public enum Job {IDLE, FARMER, CARETAKER, RESEARCHER};

	public String name {get; set;}

	public int iSpecies {get; set;}
	public int iSubSpecies {get; set;}

	public int currentJob {get; set;}

	public int strength {get; set;}
	public int dexterity {get; set;}
	public int constitution {get; set;}
	public int intelligence {get; set;}
	public int wisdom {get; set;}
	public int charisma {get; set;}

	public Unit() {
	}
	public void InitName() {
		switch (iSpecies) {
			case (int)Species.CHANGELING:
				name = "Changeling"+GameController.instance.lstUnits.Count;
				break;
			case (int)Species.PONY:
				name = "Pony"+GameController.instance.lstUnits.Count;
				break;
			default:
				name = "broken";
				Debug.Log("Borked");
				break;
		}
	}
}

public class Pony : Unit {
	//Starting base for each subspecies
	private int[] pegStats 		= {10, 12, 8, 10, 12, 10};
	private int[] uniStats 		= {8, 10, 10, 12, 12, 10};
	private int[] earthStats 	= {12, 10, 12, 8, 10, 10};

	//Create Random Pony
	public Pony() {
		System.Random rng = new System.Random();
		iSpecies = (int)Species.PONY;
		iSubSpecies = rng.Next(Enum.GetNames(typeof(PonySubSpecies)).Length);
		currentJob = (int)Job.IDLE;
		InitStats();
		InitName();
	}

	public Pony(int subSpecies) {
		iSpecies = (int)Species.PONY;
		if (PonySubSpecies.IsDefined(typeof(PonySubSpecies),subSpecies))
			iSubSpecies = subSpecies;
		else
			Debug.Log("Invalid Pony Subspecies");
		currentJob = (int)Job.IDLE;
		InitStats();
		InitName();
	}

	public void InitStats(){ 
		int[] baseStats = {10,10,10,10,10,10};
		switch (iSubSpecies){
			case (int)PonySubSpecies.EARTH :
				baseStats = earthStats;
				break;
			case (int)PonySubSpecies.PEGASUS :
				baseStats = pegStats;
				break;
			case (int)PonySubSpecies.UNICORN :
				baseStats = uniStats;
				break;
			default:
				Debug.Log("InitStats switch failed");
				break;
		}
		//Right now all members of a subspecies have the same stats
		strength = baseStats[0];
		dexterity = baseStats[1];
		constitution = baseStats[2];
		intelligence = baseStats[3];
		wisdom = baseStats[4];
		charisma = baseStats[5];
	}	
}

public class Changeling : Unit {
	private static float HARVESTER_HUG_CAP_MOD = 12.0f;
//	private static float HARVESTER_HUG_GEN_MOD = 20.0f;

//	private static float STARTING_HUG_AMT = 8200.0f; //One day's worth
	private static float STARTING_HUG_CAP = 24600.0f; //3 days' worth
//	private static float STARTING_HUG_GEN = 1.0f;
//	private static float STARTING_MAX_CUDDLES = 10.0f; //Cuddles are units of current, in hugs/sec

	//{DRONE, WORKER, HARVESTER, SCOUT, SOLDIER, ROYAL, QUEEN};
	private int[] droneStats 		= {16, 16, 16, 2, 2, 2};
	private int[] workerStats 		= {10, 10, 12, 11, 10, 8};
	private int[] harvesterStats 	= {10, 10, 12, 10, 10, 9}; //have 20x hug multiplier, 12x hug capacity
	private int[] scoutStats 		= {10, 11, 12, 10, 10, 8};
	private int[] soldierStats 		= {11, 10, 12, 10, 10, 8};
	private int[] royalStats 		= {10, 10, 12, 10, 11, 8};
	private int[] queenStats 		= {12, 11, 12, 14, 12, 12};

	public float hugCapacity {get; set;}
	public float hugGen {get; set;}

	//Create Random Changeling
	public Changeling(){
		System.Random rng = new System.Random();
		iSpecies = (int)Species.CHANGELING;
		iSubSpecies = rng.Next(Enum.GetNames(typeof(ChangelingSubSpecies)).Length);	
		currentJob = (int)Job.IDLE;

		InitHugLevels();
		InitStats();
		InitName();
	}
		
	public Changeling(bool hasQueen) : this() {
		if (!hasQueen) {
			System.Random rng = new System.Random();
			iSubSpecies = rng.Next((int)ChangelingSubSpecies.WORKER, (int)ChangelingSubSpecies.SCOUT);	
			currentJob = (int)Job.IDLE;

			InitHugLevels();
			InitStats();
			InitName();
		}
	}

	public Changeling(int subSpecies) {
		iSpecies = (int)Species.CHANGELING;
		if (ChangelingSubSpecies.IsDefined(typeof(ChangelingSubSpecies),subSpecies)){
			iSubSpecies = subSpecies;
			currentJob = (int)Job.IDLE;

			InitHugLevels();
			InitStats();
			InitName();
		}
		else
			Debug.Log("Invalid Changeling Subspecies");
	}

	public void InitStats(){
		int[] baseStats = {10,10,10,10,10,10};
		switch (iSubSpecies){
			case (int)ChangelingSubSpecies.DRONE :
				baseStats = droneStats;
				break;
			case (int)ChangelingSubSpecies.HARVESTER :
				baseStats = harvesterStats;
				break;
			case (int)ChangelingSubSpecies.QUEEN :
				baseStats = queenStats;
				break;
			case (int)ChangelingSubSpecies.ROYAL :
				baseStats = royalStats;
				break;
			case (int)ChangelingSubSpecies.SCOUT :
				baseStats = scoutStats;
				break;
			case (int)ChangelingSubSpecies.SOLDIER :
				baseStats = soldierStats;
				break;
			case (int)ChangelingSubSpecies.WORKER :
				baseStats = workerStats;
				break;
			default:
				Debug.Log("InitStats switch failed");
				break;
		}
		//Right now all members of a subspecies have the same stats
		strength = baseStats[0];
		dexterity = baseStats[1];
		constitution = baseStats[2];
		intelligence = baseStats[3];
		wisdom = baseStats[4];
		charisma = baseStats[5];
	}

	public void InitHugLevels() {
		hugCapacity = STARTING_HUG_CAP;
//		hugGen = STARTING_HUG_GEN;

		if (iSubSpecies == (int)ChangelingSubSpecies.HARVESTER) {
			hugCapacity *= HARVESTER_HUG_CAP_MOD;
//			hugGen *= HARVESTER_HUG_GEN_MOD;
		}
	}
}
