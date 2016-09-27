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
		int startingWorkers = 150;
		int startingHarvesters = 90;
		int startingScouts = 60;
		int startingUni = 2, startingEarth = 2, startingPeg = 2;

		int itr;
		for (itr = 0; itr < startingWorkers; itr++){
			lstUnits.Add(new Changeling((int)Unit.ChangelingSubSpecies.WORKER));
		}
		for (itr = 0; itr < startingHarvesters; itr++){
			lstUnits.Add(new Changeling((int)Unit.ChangelingSubSpecies.HARVESTER));
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

		//Get all managers
		serializer.Serialize(objResources,"resources.xml");
		serializer.Serialize(lstUnits, "units.xml");
//		Debug.Log(serializer.Deserialize("test.xml"));
		//Call their save function
		//Serialize and store


	}

	public void Load() {
		SharpSerializer serializer = new SharpSerializer(); //Currently outputting XML. Pass true to use bin mode

		objResources = (BaseResources)serializer.Deserialize("resources.xml");
		lstUnits = (List<Unit>)serializer.Deserialize("units.xml");
	}
}

public class BaseResources {
	// Gameplay Constants
	private static float STARTING_FOOD = 50.0f;
	private static float STARTING_BOOPS = 1000.0f;

	public float boop {get; set;}
	public float food {get; set;}

	public BaseResources(){
		boop = STARTING_BOOPS;
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

	public String name {get; set;}

	public int iSpecies {get; set;}
	public int iSubSpecies {get; set;}

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
		InitStats();
		InitName();
	}

	public Pony(int subSpecies) {
		iSpecies = (int)Species.PONY;
		if (PonySubSpecies.IsDefined(typeof(PonySubSpecies),subSpecies))
			iSubSpecies = subSpecies;
		else
			Debug.Log("Invalid Pony Subspecies");
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

	//For when we randomly generate species specific names
	public void InitName() {
		base.InitName();
	}
}

public class Changeling : Unit {
	//{DRONE, WORKER, HARVESTER, SCOUT, SOLDIER, ROYAL, QUEEN};
	private int[] droneStats 		= {16, 16, 16, 2, 2, 2};
	private int[] workerStats 		= {10, 10, 12, 11, 10, 8};
	private int[] harvesterStats 	= {10, 10, 12, 10, 10, 9};
	private int[] scoutStats 		= {10, 11, 12, 10, 10, 8};
	private int[] soldierStats 		= {11, 10, 12, 10, 10, 8};
	private int[] royalStats 		= {10, 10, 12, 10, 11, 8};
	private int[] queenStats 		= {12, 11, 12, 14, 12, 12};

	//Create Random Changeling
	public Changeling(){
		System.Random rng = new System.Random();
		iSpecies = (int)Species.CHANGELING;
		iSubSpecies = rng.Next(Enum.GetNames(typeof(ChangelingSubSpecies)).Length);	
		InitStats();
		InitName();
	}
		
	public Changeling(bool hasQueen) : this() {
		if (!hasQueen) {
			System.Random rng = new System.Random();
			iSubSpecies = rng.Next((int)ChangelingSubSpecies.WORKER, (int)ChangelingSubSpecies.SCOUT);	
			InitStats();
			InitName();
		}
	}

	public Changeling(int subSpecies) {
		iSpecies = (int)Species.CHANGELING;
		if (ChangelingSubSpecies.IsDefined(typeof(ChangelingSubSpecies),subSpecies)){
			iSubSpecies = subSpecies;
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

	//For when we randomly generate species specific names
	public void InitName() {
		base.InitName();
	}
}
