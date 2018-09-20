using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RayCrafting
{
    public class RayCrafter : MonoBehaviour
    {

        /// <summary>
        /// More more performant than asking each object if it has a special object script attached
        /// </summary>
        public static Dictionary<Transform, RayCrafter_SpecialObject> specialObjectDictionary = new Dictionary<Transform, RayCrafter_SpecialObject>();

        /// <summary>
        /// Keep track of all special objects in the scene
        /// </summary>
        public static List<Collider> specialObjectList = new List<Collider>();


        public static List<RayCrafter_Hit> RayCast(RayCrafter_Constructor constructor)
        {

            foreach(Collider specialObjectC in specialObjectList){
                RayCrafter_SpecialObject obj = null;
                if(specialObjectDictionary.ContainsKey(specialObjectC.transform)){ //You can remove this for performance improvement
                    obj = specialObjectDictionary[specialObjectC.transform];
                    if (obj.objectBehavior == ObjectBehavior.forceExclude)
                        constructor.excludeColliders.Add(specialObjectC);
                    if (obj.objectBehavior == ObjectBehavior.forceInclude)
                        constructor.forceIncludeColliders.Add(specialObjectC);
                }
            }

            float rayLife = 1;
            Vector3 currentRayDir = constructor.dir.normalized;

            List<Vector3> originalScales = new List<Vector3>();

            if (constructor.excludeColliders != null)
            {
                foreach (Collider c in constructor.excludeColliders)
                {
                    c.enabled = false;
                }
            }

            List<RayCrafter_Hit> hits = new List<RayCrafter_Hit>();

            List<int> forceIncludeLayers = new List<int>();
            if (constructor.forceIncludeColliders != null)
            {
                for (int i = 0; i < constructor.forceIncludeColliders.Count; i++)
                {
                    forceIncludeLayers.Add(constructor.forceIncludeColliders[i].gameObject.layer);
                    constructor.forceIncludeColliders[i].gameObject.layer = constructor.inclusionaryLayer;
                }
            }

            int maxIterations = 10000000; //Make sure we don't accidentally hit an infinite loop
            bool gotExit = false;
            Vector3 origin = constructor.pos;
            float dist = 0;
            int numCollisions = 0;

            while (rayLife > 0 && maxIterations > 0 && dist < constructor.maxDistance && numCollisions < constructor.maxCollisions)
            {
                maxIterations--;

                RaycastHit hitInfo = new RaycastHit();
                currentRayDir += new Vector3(0, -.1f * constructor.gravity * constructor.incrementSize, 0);

                if (hits.Count != 0)
                {
                    if (gotExit)
                    {
                        origin = hits[hits.Count - 1].exitPoint + currentRayDir * .01f;
                        gotExit = false;
                    }
                }

                if(constructor.showDebugLines)
                    Debug.DrawRay(origin, currentRayDir.normalized * constructor.incrementSize, Color.Lerp(Color.red,Color.green,rayLife), .1f);

                dist += (currentRayDir.normalized * constructor.incrementSize).magnitude;

                if (Physics.Raycast(origin, currentRayDir.normalized, out hitInfo, constructor.incrementSize, constructor.layerMask, QueryTriggerInteraction.Ignore))
                {
                    numCollisions++;
                    RayCrafter_Hit newHit = new RayCrafter_Hit();
                    newHit.transform = hitInfo.transform;
                    newHit.entryPoint = hitInfo.point;
                    newHit.entryNormal = hitInfo.normal;

                    float objectDensity = 1;

                    //Hit a special object
                    if(specialObjectDictionary.ContainsKey(hitInfo.transform)){
                        newHit.specialObject = specialObjectDictionary[hitInfo.transform];
                        objectDensity = newHit.specialObject.density;
                    }


                    if (constructor.maxCollisions > 0)
                    {
                        //Hopefully there's no other obstacles up here in space - perform individual calculations here
                        hitInfo.transform.position += new Vector3(0, 10000, 0);

                        RaycastHit exitHitInfo = new RaycastHit();
                        newHit.hasExitData = true;

                        if (Physics.Raycast(origin + currentRayDir.normalized * 10000 + new Vector3(0, 10000, 0), -currentRayDir.normalized, out exitHitInfo, 10000, constructor.layerMask, QueryTriggerInteraction.Ignore))
                        {
                            newHit.exitPoint = exitHitInfo.point - new Vector3(0, 10000, 0);
                            newHit.exitNormal = exitHitInfo.normal;
                            gotExit = true;
                        }
                        else
                        {
                            rayLife = -1;
                            newHit.hasExitData = false;
                        }
                        //Come back down to Earth
                        hitInfo.transform.position -= new Vector3(0, 10000, 0);

                        newHit.thickness = Vector3.Distance(newHit.exitPoint, newHit.entryPoint);
                        currentRayDir += new Vector3(0, -.2f * constructor.thicknessGravityMod * newHit.thickness * objectDensity, 0);
                        currentRayDir = Vector3.Lerp(currentRayDir, newHit.exitNormal, constructor.useExitNormalAsVelocity);

                        float possibleThickness = rayLife * (1 - constructor.dieRate);
                        newHit.passThroughValue = possibleThickness / newHit.thickness * objectDensity;
                        newHit.exitDir = currentRayDir.normalized;
                        rayLife -= newHit.thickness * constructor.dieRate * objectDensity;
                    }
                    else
                    {
                        rayLife = -1;
                    }

                    hits.Add(newHit);

                }
                origin += currentRayDir.normalized * constructor.incrementSize;
            }

            if (constructor.excludeColliders != null)
            {
                foreach (Collider c in constructor.excludeColliders)
                {
                    c.enabled = true;
                }
            }

            if (constructor.forceIncludeColliders != null)
            {
                for (int i = 0; i < constructor.forceIncludeColliders.Count; i++)
                {
                    constructor.forceIncludeColliders[i].gameObject.layer = forceIncludeLayers[i];
                }
            }

            return hits;
        }
    }

    /// <summary>
    /// By default, the raycrafter will act like a normal ray and be as performant one with no extra calculations.
    /// </summary>
    public class RayCrafter_Constructor {
        /// <summary>
        /// The start position of the ray - Default = (0,0,0)
        /// </summary>
        public Vector3 pos;
        /// <summary>
        /// The start direction of the ray - Default = (0,0,1)
        /// </summary>
        public Vector3 dir = Vector3.forward;
        /// <summary>
        /// The rate at which the rate falls down - Default = 0
        /// </summary>
        public float gravity = 0;
        /// <summary>
        /// Max distance the ray can travel - Default = 100
        /// </summary>
        public float maxDistance = 100;
        /// <summary>
        /// The default layer mask - Default = Everything
        /// </summary>
        public LayerMask layerMask = ~0;
        /// <summary>
        /// List of colliders to keep out of raycasting - Default = null
        /// </summary>
        public List<Collider> excludeColliders = null;
        /// <summary>
        /// List of colliders that are forced to be calculated with - Default = null
        /// </summary>
        public List<Collider> forceIncludeColliders = null;
        /// <summary>
        /// The layer to set the forcibly included colliders to - Default = 0
        /// </summary>
        public int inclusionaryLayer = 0;
        /// <summary>
        /// The rate at a which the ray dies from thicker objects - a value means that a thickness of 1 kills the ray - Default = 1
        /// </summary>
        public float dieRate = 1;
        /// <summary>
        /// How the thickness of an object forces the exit direction to be lower than usual - 0 means no effect - Default = 0
        /// </summary>
        public float thicknessGravityMod = 0;
        /// <summary>
        /// Lerp value - 0 means don't do anything, 1 means set exit velocity direction as the exit normal - Default = 0
        /// </summary>
        public float useExitNormalAsVelocity = 0;
        /// <summary>
        /// The rate at which gravity affects the ray - lower = lower CPU, higher = higher CPU - Default = .2f
        /// </summary>
        public float incrementSize = .2f;
        /// <summary>
        /// The max colliders that can be hit. Set to 0 for no exit ray data on the first object hit - Default = 0
        /// </summary>
        public int maxCollisions = 0;
        /// <summary>
        /// Should the ray be drawn in the editor? - Default = false
        /// </summary>
        public bool showDebugLines = false;
    }

    public class RayCrafter_Hit {
        /// <summary>
        /// Transform of the object hit
        /// </summary>
        public Transform transform;
        /// <summary>
        /// Where the object is initialy hit
        /// </summary>
        public Vector3 entryPoint;
        /// <summary>
        /// Where the ray leaves the from object frmo
        /// </summary>
        public Vector3 exitPoint;
        /// <summary>
        /// The normal of the face that the ray enters
        /// </summary>
        public Vector3 entryNormal;
        /// <summary>
        /// The normal of the face that the exit froms
        /// </summary>
        public Vector3 exitNormal;
        /// <summary>
        /// The direction of the ray after leaving this object
        /// </summary>
        public Vector3 exitDir;
        /// <summary>
        /// The thickness of the object based on the entry and exit points
        /// </summary>
        public float thickness;
        /// <summary>
        /// How far the ray gets through the object if it dies within the object
        /// </summary>
        public float passThroughValue;
        /// <summary>
        /// If the ray has data about the exit - will only be false if max collisions was set to 0 or if the collider has no backface
        /// </summary>
        public bool hasExitData = false;
        /// <summary>
        /// The special object hit - is null if doesn't exist
        /// </summary>
        public RayCrafter_SpecialObject specialObject;
    }

    public enum ObjectBehavior
    {
        doNothing,
        forceExclude,
        forceInclude
    }
}
