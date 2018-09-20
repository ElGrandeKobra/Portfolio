using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RayCrafting;

public class RayCrafter_Example : MonoBehaviour {

    [SerializeField]
    LayerMask layerMask;

    [SerializeField][Range(0, 1)]
    float gravity = .5f;

    [SerializeField][Range(0, 10)]
    float dieRate = 0;

    [SerializeField][Range(0, 20)]
    float thicknessGravityMod;

    [SerializeField][Range(0,1)]
    float useExitNormalAsVelocity
    ;
    [SerializeField][Range(.1f, 2)]
    float incrementSize = .1f;

    [SerializeField][Range(0, 100)]
    int maxCollisions = 100;

    [SerializeField][Range(0, 300)]
    float maxDistance = 300;

    [Space]

    [SerializeField]
    GameObject[] shootParticles;

    [SerializeField]
    GameObject laser;

	private void OnEnable()
	{
        
        RayCrafter_Constructor constructor = new RayCrafter_Constructor();  //Set up the ray's data - By default you really only need to set pos and dir and the ray will perform like a regular raycast
        constructor.pos = transform.position;                               //Origin of the ray
        constructor.dir = transform.forward;                                //Direction of the ray
        constructor.gravity = gravity;                                      //Rate at which ray falls down
        constructor.maxDistance = maxDistance;                                      //Max distance covered by ray
        constructor.layerMask = layerMask;                                  //Default layers that the ray should hit
        constructor.forceIncludeColliders = null;                           //Colliders that the ray should hit even if they arent in the default layer mask
        constructor.excludeColliders = null;                                //Colliders that should be ignored even if they would be hit by default
        constructor.inclusionaryLayer = 0;                                  //The layer to set inclusionary objects to
        constructor.dieRate = dieRate;                                      //The rate at which thick objects kill the ray
        constructor.thicknessGravityMod = thicknessGravityMod;              //The rate at which thick objects make the ray fall down
        constructor.useExitNormalAsVelocity = useExitNormalAsVelocity;      //Lerp value from 0 - 1 that defines the direction of the ray leaving an object 
        constructor.incrementSize = incrementSize;                          //The step value for the solver - The lower, the more accurate the gravity calculations
        constructor.maxCollisions = maxCollisions;                                    //The max collisions
        constructor.showDebugLines = false;                                 //Don't draw debug lines in editor

        List <RayCrafter_Hit> hits = RayCrafter.RayCast(constructor);             

        Vector3 lastPos = transform.position;
        foreach(RayCrafter_Hit hit in hits){
            if (hit.hasExitData)
            {
                int material = 0;
                if(hit.specialObject){
                    material = hit.specialObject.material;
                }
                Instantiate(shootParticles[material], hit.exitPoint + hit.exitNormal * .1f, Quaternion.LookRotation(hit.exitDir));
                GameObject newLaser = (GameObject)Instantiate(laser, lastPos, Quaternion.identity);
                LineRenderer lR = newLaser.GetComponent<LineRenderer>();
                lR.SetPosition(0, lastPos);
                lR.SetPosition(1, hit.entryPoint);
                lastPos = hit.exitPoint;
            }
        }

        if (hits.Count > 0)
        {
            if (hits[hits.Count - 1].hasExitData)
            {
                GameObject newLaser2 = (GameObject)Instantiate(laser, lastPos, Quaternion.identity);
                LineRenderer lR2 = newLaser2.GetComponent<LineRenderer>();
                lR2.SetPosition(0, hits[hits.Count - 1].exitPoint);
                lR2.SetPosition(1, hits[hits.Count - 1].exitPoint + hits[hits.Count - 1].exitDir * 100);
            }
        }

	}

    float t;
	private void Update()
	{
        t -= Time.deltaTime;
        if (t <= 0)
        {
            OnEnable();
            t = .2f;
        }
	}
}
