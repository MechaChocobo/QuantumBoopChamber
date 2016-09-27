using UnityEngine;
using System.Collections;

public class JobManager : MonoBehaviour {
	// Gameplay Constants
//Jobs: Idle, Farmer, Caretaker, Researcher

	private static float STARTING_TOTAL = 300.0f;
	private static float STARTING_FARMERS = 30.0f;
	private static float STARTING_CARETAKERS = 0.0f;
	private static float STARTING_RESEARCHERS = 0.0f;
	
	//Farmers produce food
	private static float STARTING_FARMER_FOOD_GEN = 0.2f; //With this, it takes 5 Farmers to feed 1 Pony, so 30 need to be assigned to farming

	//Caretakers modify boop production
	//Caretakers can tend to a max number of ponies
	//Adding extra caretakers should not modify boop production further, for now
	private static float STARTING_CARETAKER_BOOP_MOD = 1.2f; //Corresponds to a 20% increase in pony boop production
	private static float STARTING_PONY_PER_CARETAKER = 10.0f;

	//Researchers produce research (not used for anything yet)
	//Researcher consume more boops
	//Researcher count limited by space available (not modifiable right now)
	private static float STARTING_RESEARCHER_SCI_GEN = 0.01f;
	private static float STARTING_RESEARCHER_BOOP_CONSUME_MOD = 1.5f;
	private static float STARTING_RESEARCHER_CAP = 10f;

	// Singleton instance
	public static JobManager instance {get; private set;}
		
	private float fIdle;
	private float fFarmer;
	private float fCaretaker;
	private float fResearcher;

	private float fFarmerGen;
	
	private float fCaretakerBoopMod;
	private float fCaretakerPonyCap;
	
	private float fResearcherSciGen;
	private float fResearcherBoopConsMod;
	private float fResearcherCap;

	/** Lifecycle Methods **/
	// Use this for initialization
	void Start() {
		float workerPop = STARTING_TOTAL;

		//Sanity check on unit count
		if (STARTING_FARMERS + STARTING_CARETAKERS + STARTING_RESEARCHERS > workerPop) {
			Debug.Log("Someone configured starting values poorly. Fix it");
			//For now, assume no workers;
			fIdle = workerPop;
			fFarmer = 0.0f;
			fCaretaker = 0.0f;
			fResearcher = 0.0f;
		}
		else {
			fFarmer = STARTING_FARMERS;
			fCaretaker = STARTING_CARETAKERS;
			fResearcher = STARTING_RESEARCHERS;
			fIdle = workerPop - fFarmer - fCaretaker - fResearcher; //Can't be negative given previous if statement...still want to make a validator though
		}
		
		fFarmerGen = STARTING_FARMER_FOOD_GEN;
		
		fCaretakerBoopMod = STARTING_CARETAKER_BOOP_MOD;
		fCaretakerPonyCap = STARTING_PONY_PER_CARETAKER;
		
		fResearcherSciGen = STARTING_RESEARCHER_SCI_GEN;
		fResearcherBoopConsMod = STARTING_RESEARCHER_BOOP_CONSUME_MOD;
		fResearcherCap = STARTING_RESEARCHER_CAP;
		
	}
	
	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
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
		return fResearcherBoopConsMod;
	}
	public float getResearcherCap(){
		return fResearcherCap;
	}

	public float getCaretakerBoopModifier() {
		return fCaretakerBoopMod;
	}
	public float getCaretakerCount() {
		return fCaretaker;
	}	
	public float getCaretakerPonyCap() {
		return fCaretakerPonyCap;
	}
	
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
		//for now, we won't restrict caretaker count. resource manager can ensure caretaker count doesn't over-modify boop gen
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
	
	private void updateIdleCount() {
		fIdle = ResourceManager.instance.getChangelingPopulation() - fFarmer - fCaretaker -fResearcher;
		if (fIdle < 0){
			fIdle = 0.0f;
		}
	}
	
	
}
