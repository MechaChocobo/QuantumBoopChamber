using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class JobManager : MonoBehaviour {
	// Gameplay Constants

	//Jobs: Idle, Farmer, Caretaker, Researcher

	//Farmers produce food
	private static float STARTING_FARMER_FOOD_GEN = 0.2f; //With this, it takes 5 Farmers to feed 1 Pony, so 30 need to be assigned to farming

	//Caretakers modify hug production
	//Caretakers can tend to a max number of ponies
	//Adding extra caretakers should not modify hug production further, for now
	private static float STARTING_CARETAKER_HUG_MOD = 1.0f;
	private static float STARTING_HARVESTER_HUG_MOD = 20.0f; //Corresponds to a 20x increase in pony hug production
	private static float STARTING_PONY_PER_CARETAKER = 10.0f;

	//Researchers produce research (not used for anything yet)
	//Researcher consume more hugs
	//Researcher count limited by space available (not modifiable right now)
	private static float STARTING_RESEARCHER_SCI_GEN = 0.01f;
	private static float STARTING_RESEARCHER_HUG_CONSUME_MOD = 1.5f;
	private static float STARTING_RESEARCHER_CAP = 10f;

	// Singleton instance
	public static JobManager instance {get; private set;}
		
	private GameController gameState;
//	private UnitManager unitMgr;

	private float fIdle;
	private float fFarmer;
	private float fCaretaker;
	private float fResearcher;

	private float fFarmerGen;
	
	private float fCaretakerHugMod;
	private float fHarvesterCaretakerHugMod;
	private float fCaretakerPonyCap;
	
	private float fResearcherSciGen;
	private float fResearcherHugConsMod;
	private float fResearcherCap;

	/** Lifecycle Methods **/
	// Use this for initialization
	void Start() {
		//Sanity check on unit count

		fFarmer = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.FARMER).Count;
		fCaretaker = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.CARETAKER).Count;
		fResearcher = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.RESEARCHER).Count;
		fIdle = gameState.lstUnits.FindAll(x => (x.currentJob == (int)Unit.Job.IDLE) && (x.iSpecies == (int)Unit.Species.CHANGELING)).Count;
		
		fFarmerGen = STARTING_FARMER_FOOD_GEN;
		
		fCaretakerHugMod = STARTING_CARETAKER_HUG_MOD;
		fHarvesterCaretakerHugMod = fCaretakerHugMod * STARTING_HARVESTER_HUG_MOD;
		fCaretakerPonyCap = STARTING_PONY_PER_CARETAKER;
		
		fResearcherSciGen = STARTING_RESEARCHER_SCI_GEN;
		fResearcherHugConsMod = STARTING_RESEARCHER_HUG_CONSUME_MOD;
		fResearcherCap = STARTING_RESEARCHER_CAP;
		
	}
	
	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
		gameState = GameController.instance;
//		unitMgr = UnitManager.instance;
	}
	
	// Update is called once per frame
	void Update() {
//
	}

	/** Accessors **/
	public float getIdleCount() {
		return fIdle;
	}
	
	public float getFarmerCount() {
		return fFarmer;
	}
	public float getFarmerFoodGen() {
		return fFarmerGen;
	}
	
	public float getResearcherCount() {
		return fResearcher;
	}	
	public float getResearcherSciGen() {
		return fResearcherSciGen;
	}
	public float getResearcherConsumptionMod() {
		return fResearcherHugConsMod;
	}
	public float getResearcherCap(){
		return fResearcherCap;
	}

	public float getCaretakerHugModifier() {
		return fCaretakerHugMod;
	}
	public float getHarvesterCaretakerHugModifier() {
		return fHarvesterCaretakerHugMod;
	}

	public float getCaretakerCount() {
		return fCaretaker;
	}	
	public float getCaretakerPonyCap() {
		return fCaretakerPonyCap;
	}

	//Modify worker count of specified job
	public void modifyJob(int job, float val) {
		//Only change if the job exists and there is a value to change by
		if (Unit.Job.IsDefined(typeof(Unit.Job), job) && val != 0) {
			//if val is negative, we're making them idle
			if (val < 0) {
				//Get list of units with that job, make one idle, update Idle count
				List<Unit> jobUnits = gameState.lstUnits.FindAll(x => x.currentJob == job);
				if (jobUnits.Count > 0) {
					//[TODO: update this to find the least effective worker to remove]
					jobUnits[0].currentJob = (int)Unit.Job.IDLE;
					Debug.Log("Removed: "+job);
					onJobChange();
				}
				else {
					Debug.Log("No workers of specified job");
				}
			}
			else {
				//Get list of idle units, make one of that job, update job count
				List<Unit> idleUnits = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.IDLE);
				if (idleUnits.Count > 0){
					//[TODO: update this to find the most effective worker to add]
					if(job == (int)Unit.Job.CARETAKER) {
						//check for harvester first
						List<Unit> idleHarvesters = idleUnits.FindAll(x => x.iSubSpecies == (int)Unit.ChangelingSubSpecies.HARVESTER);
						if (idleHarvesters.Count > 0) {
							idleHarvesters[0].currentJob = job;
							Debug.Log("Added harvester as caretaker");
						}
						else {
							Debug.Log("Added other as caretaker"); //[TODO: There should be some alert when a non-harvester is made a caretaker]
							idleUnits[0].currentJob = job;
						}
					}
					else if (job == (int)Unit.Job.RESEARCHER) {
						//confirm we aren't at max researchers
						if(fResearcher < fResearcherCap) {
							idleUnits[0].currentJob = job;
						}
					}
					else {
						idleUnits[0].currentJob = job;
					}
					Debug.Log("Added: "+job);
					onJobChange();
				}
				else {
					Debug.Log("No idle workers");
				}
			}
		}
		else {
			if(val != 0)
				Debug.Log("Invalid Job to modify");
			else
				Debug.Log("Can't modify by 0");
		}
	}

	public void onJobChange(){
		updateJobCount();
		ResourceManager.instance.reCalcFoodDelta();
		ResourceManager.instance.reCalcHugDelta();
	}
/*
	public void modifyFarmerCount(float val) {
		if (val > fIdle) 
			val = fIdle;

		fFarmer += val;
		if (fFarmer < 0) {
			fFarmer = 0.0f;
		}
		updateIdleCount();
	}
	public void modifyCaretakerCount (float val) {
		if (val > fIdle) 
			val = fIdle;
	
		fCaretaker += val;
		if (fCaretaker < 0) {
			fCaretaker = 0.0f;
		}
		//for now, we won't restrict caretaker count. resource manager can ensure caretaker count doesn't over-modify hug gen
		updateIdleCount();
	}
	public void modifyResearcherCount (float val) {
		if (val > fIdle) 
			val = fIdle;

		fResearcher += val;
		if (fResearcher < 0) {
			fResearcher = 0.0f;
		}
		if (fResearcher > fResearcherCap) {
			fResearcher = fResearcherCap;
		}
		updateIdleCount();
	}
*/
	public void starveUnits(float count) {
		//Kill unit in reverse priority order. Current Priority: Idle, Researcher, Caretaker, Farmer
		float toStarve = count;
		float starved = 0;
		while (starved < toStarve){
			if (fIdle > 0) {
				if (fIdle < toStarve) {
					toStarve -= fIdle;
					fIdle = 0.0f;
				}
				else {
					fIdle -= toStarve;
					toStarve = 0.0f;
				}
				continue;
			}
			else if (fResearcher > 0) {
				if (fResearcher < toStarve) {
					toStarve -= fResearcher;
					fResearcher = 0.0f;
				}
				else {
					fResearcher -= toStarve;
					toStarve = 0.0f;
				}
				continue;
			}
			else if (fCaretaker > 0){
				if (fCaretaker < toStarve) {
					toStarve -= fCaretaker;
					fCaretaker = 0.0f;
				}
				else {
					fCaretaker -= toStarve;
					toStarve = 0.0f;
				}
			}
			else if (fFarmer > 0) {
				if (fFarmer < toStarve) {
					toStarve -= fFarmer;
					fFarmer = 0.0f;
				}
				else {
					fFarmer -= toStarve;
					toStarve = 0.0f;
				}
				continue;
			}
			else {
				Debug.Log("No one left to starve.");
				break;
			}
		}
	}

	public void updateJobCount() {
		fFarmer = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.FARMER).Count;
		fCaretaker = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.CARETAKER).Count;
		fResearcher = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.RESEARCHER).Count;
		fIdle = gameState.lstUnits.FindAll(x => (x.currentJob == (int)Unit.Job.IDLE) && (x.iSpecies == (int)Unit.Species.CHANGELING)).Count;
	}
}
