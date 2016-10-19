using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ResourceManager : MonoBehaviour {
	// Gameplay Constants
	private static float STARTING_PONE_HUG_GEN = 0.23726852f; // Pony production is 0.23 hugs/sec

	private static float STARTING_HUG_DELTA = 0.0f;
	private static float STARTING_FOOD_DELTA = 0.0f;

	//boops are 1/16 of a hug
//	private static float HUGS_TO_BOOP_RATIO = 16.0f;

	private static float STARTING_HUG_CAP = 5000.0f;	
	private static float STARTING_FOOD_CAP = 500.0f;

	private static float BASE_HUG_CONSUMPTION_PER_LING = 0.09490741f; // Changeling consumption is 0.09 hugs/sec
//	private static float BASE_HUG_CONSUMPTION_MULTIPLIER = 1.0f;
	private static float BASE_FOOD_CONSUMPTION_PER_PONE = 1.0f;
//	private static float BASE_FOOD_CONSUMPTION_MULTIPLIER = 1.0f;

	private static float STARTING_HUG_PER_CLICK = 1.0f;
	
	// Singleton instance
	public static ResourceManager instance {get; private set;} 

	private GameController gameState;

	private float hugDelta;
	private float hugCapacity;
//	private float hugConsumptionMultiplier;
	
	private float hugPerClick;
	private float hugPerPony;
	public float hugConsPerChangeling {get; private set;}

	private float foodDelta;
	private float foodCapacity;
//	private float foodConsumptionMultiplier;
	
	private float foodPerWorker;

	private bool isPostStart; //Used to run a cleanup after all other start functions have been called
	private float foodConsPerPony;

	/** Lifecycle Methods **/
	// Use this for initialization
	void Start() {

		foodCapacity = STARTING_FOOD_CAP;
		hugCapacity = STARTING_HUG_CAP;
		foodDelta = STARTING_FOOD_DELTA;
		hugDelta = STARTING_HUG_DELTA;
//		foodConsumptionMultiplier = BASE_FOOD_CONSUMPTION_MULTIPLIER;
//		hugConsumptionMultiplier = BASE_HUG_CONSUMPTION_MULTIPLIER;

		hugPerClick = STARTING_HUG_PER_CLICK;
		hugPerPony = STARTING_PONE_HUG_GEN;		
		hugConsPerChangeling = BASE_HUG_CONSUMPTION_PER_LING;

		isPostStart = false;
		foodConsPerPony = BASE_FOOD_CONSUMPTION_PER_PONE;
	}
	
	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
		gameState = GameController.instance;
	}

	// Update is called once per frame
	void Update() {

		if (!isPostStart) {
			reCalcHugDelta();
			reCalcFoodDelta();
			isPostStart = true;
		}

		UnitManager unitMgr = UnitManager.instance;
		LogManager logger = LogManager.instance;
		float timeDelta = Time.deltaTime;

		logger.WriteToLog("Resource Statistics");
//		bool ponyDeath = false;
//		bool changelingDeath = false;

		gameState.objResources.food += foodDelta * timeDelta;
		if (gameState.objResources.food <= 0.0f) {
			gameState.objResources.food = 0.0f;
//			ponyDeath = true;
		}
		else if (gameState.objResources.food > foodCapacity) {
			gameState.objResources.food = foodCapacity;
		}

		if (foodDelta > 0) {
			logger.AppendToLog("\nGenerating extra food at " + foodDelta + " food per second.");
		}
		else if(foodDelta < 0) {
			logger.AppendToLog("\nNot generating enough food at " + foodDelta + " food per second.");
		}
		else {
			logger.AppendToLog("\nFood is stable.");
		}

/*
		if (ponyDeath && ponyPop > 0) {
			ponyPop -= 1;
			logger.AppendToLog("\nA pony died!");
		}
*/

		gameState.objResources.hug += hugDelta * timeDelta;
		if (gameState.objResources.hug <= 0.0f) {
			gameState.objResources.hug = 0.0f;
//			changelingDeath = true;
		}
		else if (gameState.objResources.hug > hugCapacity) {
			gameState.objResources.hug = hugCapacity;
		}

		if (hugDelta > 0) {
			logger.AppendToLog("\nGenerating extra hugs at " + hugDelta + " hugs per second.");
		}
		else if(hugDelta < 0) {
			logger.AppendToLog("\nNot generating enough hugs at " + hugDelta + " hugs per second.");
		}
		else {
			logger.AppendToLog("\nHugs are stable.");
		}

/*
		if (changelingDeath && changelingPop > 0) {
			changelingPop -= 1;
			logger.AppendToLog("\nA changeling died!");
			JobManager.instance.starveUnits(1);
		}
*/		
		logger.AppendToLog("\nUnit Count: " + gameState.lstUnits.Count);
		logger.AppendToLog("\nPopulation: \tChangelings: " + unitMgr.changelingPop + " Ponies: " + unitMgr.ponyPop);
		logger.AppendToLog("\nJobs: \tIdle: " + JobManager.instance.getIdleCount() + " Farmers: " + JobManager.instance.getFarmerCount() + " Caretakers: " + JobManager.instance.getCaretakerCount());
	}

	/** Accessors **/
/*
	public float getFoodConsumption() {
		return ponyPop * foodConsPerPony * foodConsumptionMultiplier; 
	}

	public float getFoodProduction() {
		JobManager jManager = JobManager.instance;
		return jManager.getFarmerCount() * jManager.getFarmerFoodGen();
	}

	public float getBoopConsumption() {
		JobManager jManager = JobManager.instance;
		float fNonResearcherCount = jManager.getFarmerCount() + jManager.getCaretakerCount() + jManager.getIdleCount();
		float fResearcherCount = jManager.getResearcherCount();
		
		float fNonResearcherConsumption = fNonResearcherCount * boopConsPerChangeling;
		float fReasearcherConsumption = fResearcherCount * boopConsPerChangeling * jManager.getResearcherConsumptionMod();
		
		return (fNonResearcherConsumption + fReasearcherConsumption) * boopConsumptionMultiplier;
	}

	public float getBoopProduction() {
		JobManager jManager = JobManager.instance;
		
		float fCaretakerCount = jManager.getCaretakerCount();
		float fCaretakerPonyCap = jManager.getCaretakerPonyCap();
		float fCaretakerBoopProdMod = jManager.getCaretakerBoopModifier();
		
		float fMaxBoostedPonies = fCaretakerCount * fCaretakerPonyCap;
		float fBoostedPonies = (fMaxBoostedPonies > ponyPop) ? ponyPop : fMaxBoostedPonies;
		float fUnboostedPonies = ponyPop - fBoostedPonies;
		
		
		return (fUnboostedPonies * boopPerPony) + (fBoostedPonies * boopPerPony * fCaretakerBoopProdMod);
	}	
*/	
	public float getHugCap() {
		return hugCapacity;
	}
	
	public float getFoodCap() {
		return foodCapacity;
	}

	public void reCalcFoodDelta() {
		//Check farmer count, get food produced, then get pony count and reduce by amount consumed
		float foodConsumption = UnitManager.instance.ponyPop * foodConsPerPony;
		float foodProduction = 0;

		if (JobManager.instance.getFarmerCount() > 0) {
			//Currently all farmers produce at the same rate, so just get the product
			foodProduction = JobManager.instance.getFarmerCount() * JobManager.instance.getFarmerFoodGen();
		}

		foodDelta = foodProduction - foodConsumption;
	}

	public void reCalcHugDelta() {
		//Get caretaker count, get hugs produced, then get changeling count, and reduce by amount consumed
		float hugConsumption = UnitManager.instance.changelingPop * hugConsPerChangeling;
		float hugProduction  = 0.0f;

		//Get number of ponies tended to by harvesters
		if (JobManager.instance.getCaretakerCount() > 0) {
			List<Unit> lstCaretakers = gameState.lstUnits.FindAll(x => x.currentJob == (int)Unit.Job.CARETAKER);
			int harvesterCnt = lstCaretakers.FindAll(x => x.iSubSpecies == (int)Unit.ChangelingSubSpecies.HARVESTER).Count;
			float boostMod = JobManager.instance.getHarvesterCaretakerHugModifier();
			float unBoostedMod = JobManager.instance.getCaretakerHugModifier();

			//Get tended ponies
			float maxTendedPonies = lstCaretakers.Count * JobManager.instance.getCaretakerPonyCap();
			float tendedPonies = (maxTendedPonies > UnitManager.instance.ponyPop) ? UnitManager.instance.ponyPop : maxTendedPonies;

			//Get amount boosted
			float maxBoostedPonies = harvesterCnt * JobManager.instance.getCaretakerPonyCap();
			float boostedPonies = (maxBoostedPonies > tendedPonies) ? tendedPonies : maxBoostedPonies;
			float unBoostedPonies = 0.0f;

			//Get remaining unboosted
			if (tendedPonies > boostedPonies) {
				unBoostedPonies = tendedPonies - boostedPonies;
			}

			hugProduction = (unBoostedPonies * hugPerPony * unBoostedMod) + (boostedPonies * hugPerPony * boostMod);
		}
		hugDelta = hugProduction - hugConsumption;
	}

	public void hugClick(){
		GameController.instance.objResources.hug += hugPerClick;
	}
}
