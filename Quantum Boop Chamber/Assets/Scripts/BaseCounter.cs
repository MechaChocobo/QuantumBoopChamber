using UnityEngine;
using System.Collections;

public class BaseCounter : MonoBehaviour {

	protected int iMaxVal, iMinVal, iCurVal, iValDelta, iTimerCycle;
	protected float fTimer;
		
	public int CurVal
	{
		get 
		{
			return iCurVal; 
		}
		set 
		{
			iCurVal = value; 
		}
	}
	
	// Use this for initialization
	public virtual void Start () {
		iMaxVal = 1000;
		iMinVal = 0;
		iCurVal = 0;
		iValDelta = 0;
		iTimerCycle = -1;
		fTimer = 0.0f;
	}
	
	// Update is called once per frame
	public virtual void Update () {
		if (iTimerCycle > 0 && iValDelta != 0) {
			fTimer += Time.deltaTime;
			if (fTimer > iTimerCycle) {
				fTimer = 0.0f;
				ModifyVal(iValDelta);
			}
		}		
	}
	
	// Updates current value by provided amount
	protected void ModifyVal (int val) {
		iCurVal += val;
		if (iCurVal > iMaxVal)
			iCurVal = 1000;
		else if (iCurVal < iMinVal)
			iCurVal = 0;
	}
	
}
