using UnityEngine;
using System.Collections;

public class ResourceManager : MonoBehaviour {
	// Gameplay Constants
	private static float STARTING_LINGS = 300.0f;
	private static float STARTING_PONES = 6.0f;

	private static float STARTING_PONE_BOOP_GEN = 50.0f;

	//boops are 1/16 of a hug
	private static float BOOP_TO_HUGS_RATIO = 0.0625f;

	private static float STARTING_LING_CAP = 1000.0f;
	private static float STARTING_PONE_CAP = 500.0f;

	private static float STARTING_BOOP_CAP_PER_LING = 100.0f;
	private static float STARTING_BOOP_CAP_PER_PONE = 1000.0f;
	
	private static float STARTING_FOOD_CAP = 500.0f;

	private static float BASE_BOOP_CONSUMPTION_PER_LING = 1.0f; // 1 population eats 1 food per 1 second.
	private static float BASE_BOOP_CONSUMPTION_MULTIPLIER = 1.0f;
	private static float BASE_FOOD_CONSUMPTION_PER_PONE = 1.0f;
	private static float BASE_FOOD_CONSUMPTION_MULTIPLIER = 1.0f;

	private static float STARTING_BOOP_PER_CLICK = 1.0f;
	
	// Singleton instance
	public static ResourceManager instance {get; private set;} 

	private float boopCapacity;
	private float boopConsumptionMultiplier;
	
	private float boopPerClick;
	private float boopPerPony;
	private float boopConsPerChangeling;
	private float boopCapFactorChangeling;
	private float boopCapFactorPony;
	
	private float foodCapacity;
	private float foodConsumptionMultiplier;
	
	private float foodPerWorker;
	private float foodConsPerPony;

	private float changelingPop; // Current total changeling population
	private float changelingPopCapacity;
	
	private float ponyPop; // Current total pony population
	private float ponyPopCapacity;
	

	/** Lifecycle Methods **/
	// Use this for initialization
	void Start() {
		changelingPop = STARTING_LINGS;
		ponyPop = STARTING_PONES;
		changelingPopCapacity = STARTING_LING_CAP;
		ponyPopCapacity = STARTING_PONE_CAP;

		foodCapacity = STARTING_FOOD_CAP;
		boopCapFactorChangeling = STARTING_BOOP_CAP_PER_LING;
		boopCapFactorPony = STARTING_BOOP_CAP_PER_PONE;
		boopCapacity = (changelingPop * boopCapFactorChangeling) + (ponyPop * boopCapFactorPony);
		foodConsumptionMultiplier = BASE_FOOD_CONSUMPTION_MULTIPLIER;
		boopConsumptionMultiplier = BASE_BOOP_CONSUMPTION_MULTIPLIER;

		boopPerClick = STARTING_BOOP_PER_CLICK;
		boopPerPony = STARTING_PONE_BOOP_GEN;		
		boopConsPerChangeling = BASE_BOOP_CONSUMPTION_PER_LING;
		
		foodConsPerPony = BASE_FOOD_CONSUMPTION_PER_PONE;
	}
	
	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
	}
	
	// Update is called once per frame
	void Update() {
		GameController gameState = GameController.instance;
		LogManager logger = LogManager.instance;
		logger.WriteToLog("Resource Statistics");
		bool ponyDeath = false;
		bool changelingDeath = false;

		float foodProduction = getFoodProduction() * Time.deltaTime; 
		float foodConsumption = getFoodConsumption() * Time.deltaTime;
		float foodDelta = foodProduction - foodConsumption;

		gameState.objResources.food += foodDelta;
		if (gameState.objResources.food <= 0.0f) {
			gameState.objResources.food = 0.0f;
			ponyDeath = true;
		}
		else if (gameState.objResources.food > foodCapacity) {
			gameState.objResources.food = foodCapacity;
		}

		if (foodDelta > 0) {
			logger.AppendToLog("\nGenerating extra food.");
		}
		else if(foodDelta < 0) {
			logger.AppendToLog("\nNot generating enough food.");
		}
		else {
			logger.AppendToLog("\nFood is stable.");
		}

		if (ponyDeath && ponyPop > 0) {
			ponyPop -= 1;
			logger.AppendToLog("\nA pony died!");
		}

		float boopProduction = getBoopProduction() * Time.deltaTime;
		float boopConsumption = getBoopConsumption() * Time.deltaTime;
		float boopDelta = boopProduction - boopConsumption;

		gameState.objResources.boop += boopDelta;
		if (gameState.objResources.boop <= 0.0f) {
			gameState.objResources.boop = 0.0f;
			changelingDeath = true;
		}
		else if (gameState.objResources.boop > boopCapacity) {
			gameState.objResources.boop = boopCapacity;
		}

		if (boopDelta > 0) {
			logger.AppendToLog("\nGenerating extra boops.");
		}
		else if(boopDelta < 0) {
			logger.AppendToLog("\nNot generating enough boops.");
		}
		else {
			logger.AppendToLog("\nBoops are stable.");
		}

		if (changelingDeath && changelingPop > 0) {
			changelingPop -= 1;
			logger.AppendToLog("\nA changeling died!");
			JobManager.instance.starveUnits(1);
		}
		logger.AppendToLog("\nUnit Count: " + GameController.instance.lstUnits.Count);
	}

	/** Accessors **/
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
	
	public float getBoopCap() {
		return (changelingPop * boopCapFactorChangeling) + (ponyPop * boopCapFactorPony);
	}
	
	public float getFoodCap() {
		return foodCapacity;
	}

	// POPULATION
	public float getChangelingPopulation() {
		return changelingPop;
	}

	public float getPonyPopulation() {
		return ponyPop;
	}
	
	public float getChangelingPopulationCap() {
		return changelingPopCapacity;
	}

	public float getPonyPopulationCap() {
		return ponyPopCapacity;
	}
	
	public void boopClick(){
		GameController.instance.objResources.boop += boopPerClick;
	}
}
