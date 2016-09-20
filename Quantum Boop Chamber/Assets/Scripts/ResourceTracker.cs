using UnityEngine;
using System.Collections;

public class ResourceTracker : MonoBehaviour {
	// Gameplay Constants
	private static float STARTING_LINGS = 300.0f;
	private static float STARTING_PONES = 6.0f;
	private static float STARTING_FOOD = 50.0f;
	private static float STARTING_BOOPS = 1000.0f;
	
	private static float STARTING_PONE_BOOP_GEN = 50.0f;
	private static float STARTING_WORKER_FOOD_GEN = 0.1f;
	
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
	
//	private static float BASE_FOOD_TO_NEXT_POP = 20.0f;
//	private static float BASE_FOOD_TO_NEXT_POP_MULTIPLIER = 0.2f;

	// Singleton instance
	public static ResourceTracker instance {get; private set;}

	private float boop;
	private float boopVelocity;
	private float boopCapacity;
	private float boopConsumptionMultiplier;
	
	private float boopPerClick;
	private float boopPerPony;
	
	private float food; // Current total food
	private float foodVelocity; // Rate of food production
	private float foodCapacity;
	private float foodConsumptionMultiplier;
	
	private float foodPerWorker;

	private float changelingPop; // Current total changeling population
	private float changelingPopVelocity; // Rate of changeling popluation production
	private float changelingPopCapacity;
	
	private float ponyPop; // Current total pony population
	private float ponyPopVelocity; // Rate of pony popluation production
	private float ponyPopCapacity;
	
//	private float foodNextPop;
//	private float foodNextPopMultiplier;


	/** Lifecycle Methods **/
	// Use this for initialization
	void Start() {
		changelingPop = STARTING_LINGS;
		ponyPop = STARTING_PONES;
		changelingPopVelocity = 0.0f;
		ponyPopVelocity = 0.0f;		
		changelingPopCapacity = STARTING_LING_CAP;
		ponyPopCapacity = STARTING_PONE_CAP;
		
		food = STARTING_FOOD;
		boop = STARTING_BOOPS;
		foodVelocity = 0.0f;
		boopVelocity = 0.0f;
		foodCapacity = STARTING_FOOD_CAP;
		boopCapacity = (changelingPop * STARTING_BOOP_CAP_PER_LING) + (ponyPop * STARTING_BOOP_CAP_PER_PONE);
		foodConsumptionMultiplier = BASE_FOOD_CONSUMPTION_MULTIPLIER;
		boopConsumptionMultiplier = BASE_BOOP_CONSUMPTION_MULTIPLIER;

		boopPerClick = STARTING_BOOP_PER_CLICK;
		boopPerPony = STARTING_PONE_BOOP_GEN;
		
		foodPerWorker = STARTING_WORKER_FOOD_GEN;

//		foodNextPop = BASE_FOOD_TO_NEXT_POP;
//		foodNextPopMultiplier = BASE_FOOD_TO_NEXT_POP_MULTIPLIER;
	}
	
	void Awake() {
		// Update our singleton to the current active instance
		instance = this;
	}
	
	// Update is called once per frame
	void Update() {
		bool ponyDeath = false;
		bool changelingDeath = false;
		// Apply food production velocity
//		food += BuildingTracker.instance.getFoodProduction() * Time.deltaTime;
		food += getChangelingPopulation() * foodPerWorker * Time.deltaTime; //Change to use workers rather than population once jobs are implemented

		// Calculate current consumption & Eat
		float foodConsumption = getFoodConsumption() * foodConsumptionMultiplier * Time.deltaTime;
		food -= foodConsumption;
		if (food <= 0.0f) {
			food = 0.0f;
			ponyDeath = true;
		}
		else if (food > foodCapacity) {
			food = foodCapacity;
		}

		if (ponyDeath && ponyPop > 0) {
			ponyPop -= 1 * Time.deltaTime;
		}
		
		boop += boopPerPony * ponyPop * Time.deltaTime;
		
		float boopConsumption = getBoopConsumption() * boopConsumptionMultiplier * Time.deltaTime;
		boop -= boopConsumption;
		if (boop <= 0.0f) {
			boop = 0.0f;
			changelingDeath = true;
		}
		else if (boop > boopCapacity) {
			boop = boopCapacity;
		}
		
		if (changelingDeath && changelingPop > 0) {
			changelingPop -= 1 * Time.deltaTime;
		}
		
		// Determine if it's time for a new population member
/*		
		while (food >= foodNextPop) {
			population += 1;
			foodNextPop += Mathf.Max(1.0f, foodNextPop * foodNextPopMultiplier); // Always require at least 1 more food than last time.
		}

		population += BuildingTracker.instance.getPopulationProduction() * Time.deltaTime;
*/
	}

	/** Accessors **/
	public float getFoodConsumption() {
		return ponyPop * BASE_FOOD_CONSUMPTION_PER_PONE;
	}

	public float getBoopConsumption() {
		return changelingPop * BASE_BOOP_CONSUMPTION_PER_LING;
	}
	
	// FOOD
	public float getFood() {
		return food;
	}

	public float getBoop() {
		return boop;
	}
	
	public float getBoopCap() {
		return boopCapacity;
	}
	
	public float getFoodCap() {
		return foodCapacity;
	}
/*	
	public bool applyFoodImpulse(float food) {
		// Food is not allowed to dip below 0 due to a Food Impulse event.
		if (this.food + food >= 0.0f) {
			this.food += food;
			return true;
		}
		return false;
	}

	public void applyFoodVelocity(float foodVel) {
		foodVelocity += foodVel;
	}
*/
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
/*	
	public bool applyPopulationImpulse(float pop) {
		// Population is not allowed to dip below 0 due to a Population Impulse event.
		if (this.population + pop >= 0.0f) {
			this.population += pop;
			return true;
		}
		return false;
	}

	public void applyPopulationVelocity(float popVel) {
		populationVelocity += popVel;
	}

	// FOOD NEXT POP
	public float getFoodUntilNextPop() {
		return foodNextPop;
	}
	*/
	
	public void boopClick(){
		boop += boopPerClick;
	}
}
