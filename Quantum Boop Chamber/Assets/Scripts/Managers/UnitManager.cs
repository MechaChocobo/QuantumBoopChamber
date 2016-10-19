using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class UnitManager : MonoBehaviour {
	public static UnitManager instance {get; private set;} 
	private GameController gameState;


	private static float STARTING_LING_CAP = 1000.0f;
	private static float STARTING_PONE_CAP = 500.0f;

	public float changelingPop {get; private set;} // Current total changeling population
	public float changelingPopCapacity {get; private set;}

	public float ponyPop {get; private set;} // Current total pony population
	public float ponyPopCapacity {get; private set;}

	// Use this for initialization
	void Start () {
		changelingPop = gameState.lstUnits.FindAll(x => x.iSpecies == (int)Unit.Species.CHANGELING).Count;
		ponyPop = gameState.lstUnits.FindAll(x => x.iSpecies == (int)Unit.Species.PONY).Count;

		changelingPopCapacity = STARTING_LING_CAP;
		ponyPopCapacity = STARTING_PONE_CAP;
	}

	void Awake() {
		instance = this;
		gameState = GameController.instance;
	}
/*
	public float feedChangelings(float amt) {
		//modify reserves by amt
		//return remainder
		List<Changeling> lstChangelings = gameState.lstUnits.FindAll(x => x.iSpecies == (int)Unit.Species.CHANGELING);
		float remainder = amt;

		if (amt > 0) {
			//Add evently through reserves not currently at cap
			List<Changeling> lstNotFull = lstChangelings.FindAll(x => x.currentHugs != x.hugCapacity);
			if (lstNotFull.Count > 0) {
				//Go through and fill reserves
				float modAmt = remainder/lstNotFull.Count;
				for(int i = 0; i<lstNotFull.Count; i++) {
					Changeling currLing = lstNotFull[i];
					if (currLing.hugCapacity-currLing.currentHugs >= modAmt){
						currLing.currentHugs += modAmt;
						remainder -= modAmt;
					}
					else {
						float fillAmt = currLing.hugCapacity - currLing.currentHugs;
						currLing.currentHugs = currLing.hugCapacity;
						remainder -= (modAmt - fillAmt);
						if (i != (lstNotFull.Count - 1)) {
							
						}
					}
				}
			}
		}
		else if (amt < 0) {
			//reduce hugs from excess harvester reserve if present
			//drain evenly among reserves
			//Check for starved lings
			List<Changeling> lstNoReserve = lstChangelings.FindAll(x => x.currentHugs <= 0);
			if (lstNoReserve.Count == 0) {
				//starved lings
			}
			List<Changeling> lstWithReserve = lstChangelings.FindAll(x => x.currentHugs > 0);
			if (lstWithReserve.Count > 0) {
				//reduce reserve
			}
		}

		return remainder;
	}
*/
	// Update is called once per frame
	void Update () {

	}
}
