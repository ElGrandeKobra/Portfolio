using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayCrafting;

public class RayCrafter_Example : MonoBehaviour {

    public LayerMask layerMask;
    [Range(0, 1)]
    public float gravity = .5f;
    [Range(0, 10)]
    public float dieRate = 0;
    [Range(0, 20)]
    public float thicknessGravityMod;
    [Range(0,1)]
    public float useExitNormalAsVelocity;

    [Range(.1f, 2)]
    public float incrementSize = .1f;

    bool shoot;

    public GameObject shootParticle;
    public GameObject laser;

	private void OnEnable()
	{
        List <RayCrafting.RayCrafterHit> hits = RayCrafter.RayCast(transform.position,
                                                            transform.forward,
                                                            gravity,
                                                            300,
                                                            layerMask,
                                                            null,
                                                            null,
                                                            0,
                                                            dieRate,
                                                            thicknessGravityMod,
                                                                   useExitNormalAsVelocity,
                                                                   incrementSize);


        if(shoot){
            shoot = false;
            Vector3 lastPos = transform.position;

            foreach(RayCrafterHit hit in hits){
                GameObject newLaser = (GameObject)Instantiate(laser, lastPos, Quaternion.identity);
                LineRenderer lR = newLaser.GetComponent<LineRenderer>();
                lR.SetPosition(0, lastPos);
                lR.SetPosition(1, hit.entryPoint);
                lastPos = hit.exitPoint;
                Instantiate(shootParticle, hit.exitPoint + hit.exitNormal * .1f, Quaternion.LookRotation(hit.exitDir));
            }
            GameObject newLaser2 = (GameObject)Instantiate(laser, lastPos, Quaternion.identity);
            LineRenderer lR2 = newLaser2.GetComponent<LineRenderer>();
            lR2.SetPosition(0, hits[hits.Count - 1].exitPoint);
            lR2.SetPosition(1, hits[hits.Count - 1].exitPoint + hits[hits.Count - 1].exitDir * 100);

        }
	}

    float t;

	private void Update()
	{
        
      

        t -= Time.deltaTime;
        if (t <= 0)
        {
            shoot = true;
            OnEnable();
            t = .3f;
        }

	}
}
