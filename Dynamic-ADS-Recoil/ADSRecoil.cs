using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ADSRecoil : MonoBehaviour {
    public Transform cam;

    public WeaponRecoilPreset recoilPreset;
    public bool testing = true;
    public float testingShootDelay = .1f; //Used for testing purposes

    float randTimeSmooth;
    float randTimeMult;
    float intTime;
    float randX, randY;
    float actualRandX, actualRandY;
    float recoilTime;
    Vector3 origin;
    Vector3 mod;
    Quaternion originRot;
	// Use this for initialization
	void Start () {
        origin = transform.localPosition;
        originRot = transform.localRotation;
	}

    float shootTime;

    public void recoil(){
        recoilTime = 0;
        randTimeMult += recoilPreset.randAmountStep;
        randTimeMult = Mathf.Clamp(randTimeMult, 0, recoilPreset.randTimeMax);
    }

	void Update () {

        //FOR TESTING PURPOSES
        if (testing)
        {
            if (Input.GetAxis("Fire1") == 1)
            {
                if (shootTime < 0)
                {
                    shootTime = testingShootDelay;
                    recoil();
                }
            }
            else
            {
                randTimeMult = 0;
            }
        }

        transform.localRotation = originRot;
        intTime -= Time.deltaTime;
        shootTime -= Time.deltaTime;
        recoilTime += Time.deltaTime;

        //Randomness updates every randomStepInterval
        if(intTime < 0){
            intTime = recoilPreset.randomStepInterval;
            randX = Random.Range(-1f, 1f);
            randY = Random.Range(-1f, 1f);
        }

        float curveTime = Mathf.Clamp(recoilTime * recoilPreset.recoilSpeed, 0, 1);
        randTimeSmooth = Mathf.Lerp(randTimeSmooth, randTimeMult, Time.deltaTime * 10);

        //Z Axis Translation
        float recoilCurveEval = recoilPreset.recoilCurve.Evaluate(curveTime);
        mod = -Vector3.forward * recoilCurveEval * recoilPreset.zRecoil;
        //Y Axis Translation
        mod += Vector3.up * recoilPreset.yCurve.Evaluate(curveTime) * recoilPreset.yRecoil;

        //X and Y Translation
        actualRandX = Mathf.Lerp(actualRandX, randX, recoilPreset.randXYAdjustmentSpeed * Time.deltaTime) * recoilPreset.randXYAmount;
        actualRandY = Mathf.Lerp(actualRandY, randY, recoilPreset.randXYAdjustmentSpeed * Time.deltaTime) * recoilPreset.randXYAmount;
        mod += (Vector3.right*actualRandX + Vector3.up * actualRandY) * recoilCurveEval * recoilPreset.randXYAmount * randTimeMult;

        //Translation Composite with Y Axis Climb
        transform.localPosition = origin + mod + Vector3.up * randTimeSmooth * recoilPreset.climbMod;

        //Rotation - Maybe combine these into 1 "RotateAround?"
        float xRotCurveEval = recoilPreset.xRotCurve.Evaluate(curveTime);
        transform.RotateAround(transform.position, cam.forward,xRotCurveEval * recoilPreset.recoilRotation.z);
        transform.RotateAround(transform.position, cam.right, xRotCurveEval * recoilPreset.recoilRotation.x);
        transform.RotateAround(transform.position, cam.up, xRotCurveEval * recoilPreset.recoilRotation.y);

    }
}