using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RayCrafting
{
    public class RayCrafter : MonoBehaviour
    {


     

        public static List<RayCrafterHit> RayCast(Vector3 pos,
                                       Vector3 dir,
                                       float gravity,
                                       float maxDistance,
                                       LayerMask layerMask,
                                       List<Collider> excludeColliders,
                                       List<Collider> forceIncludeColliders,
                                       int inclusionaryLayer,
                                       float dieRate,
                                        float thicknessGravityMod,
                                                 float useExitNormalAsVelocity,
                                                 float incrementSize)
        {

            float rayLife = 1;
            Vector3 currentRayDir = dir.normalized;


            List<Vector3> originalScales = new List<Vector3>();

            if (excludeColliders != null)
            {
                foreach (Collider c in excludeColliders)
                {
                    c.enabled = false;
                }
            }

            List<RayCrafterHit> hits = new List<RayCrafterHit>();

            List<int> forceIncludeLayers = new List<int>();
            if (forceIncludeColliders != null)
            {
                for (int i = 0; i < forceIncludeColliders.Count; i++)
                {
                    forceIncludeLayers.Add(forceIncludeColliders[i].gameObject.layer);
                    forceIncludeColliders[i].gameObject.layer = inclusionaryLayer;
                }
            }

            int maxIterations = 100000;
            bool gotExit = false;
            Vector3 origin = pos;
            float dist = 0;

            while (rayLife > 0 && maxIterations > 0 && dist < maxDistance)
            {
                maxIterations--;
                RaycastHit hitInfo = new RaycastHit();
                currentRayDir += new Vector3(0, -.1f * gravity * incrementSize, 0);

                if (hits.Count != 0)
                {
                    if (gotExit)
                    {
                        origin = hits[hits.Count - 1].exitPoint + currentRayDir * .01f;
                        gotExit = false;
                    }
                }


                Debug.DrawRay(origin, currentRayDir.normalized * incrementSize, Color.Lerp(Color.red,Color.green,rayLife), .1f);

                dist += (currentRayDir.normalized * incrementSize).magnitude;

                if (Physics.Raycast(origin, currentRayDir.normalized, out hitInfo, incrementSize, layerMask, QueryTriggerInteraction.Ignore))
                {


                    RayCrafterHit newHit = new RayCrafterHit();
                    newHit.transform = hitInfo.transform;
                    newHit.entryPoint = hitInfo.point;
                    newHit.entryNormal = hitInfo.normal;

                    hitInfo.transform.position += new Vector3(0, 10000, 0);

                    RaycastHit exitHitInfo = new RaycastHit();
                    if (Physics.Raycast(origin + currentRayDir.normalized * 10000 + new Vector3(0, 10000, 0), -currentRayDir.normalized, out exitHitInfo, 10000, layerMask, QueryTriggerInteraction.Ignore))
                    {
                        newHit.exitPoint = exitHitInfo.point - new Vector3(0, 10000, 0);
                        newHit.exitNormal = exitHitInfo.normal;
                        gotExit = true;
                    }
                    else
                    {
                        Debug.Log("Error! Object has no backface");
                        rayLife = -1;
                    }
                    hitInfo.transform.position -= new Vector3(0, 10000, 0);

                    newHit.thickness = Vector3.Distance(newHit.exitPoint, newHit.entryPoint);

                    currentRayDir += new Vector3(0, -.2f * thicknessGravityMod * newHit.thickness, 0);
                    currentRayDir = Vector3.Lerp(currentRayDir, newHit.exitNormal, useExitNormalAsVelocity);

                    float possibleThickness = rayLife * (1 - dieRate);
                    newHit.passThroughValue = possibleThickness / newHit.thickness;
                    newHit.exitDir = currentRayDir.normalized;
                    hits.Add(newHit);

                    rayLife -= newHit.thickness * dieRate;
                }
                origin += currentRayDir.normalized * incrementSize;

            }

            if (excludeColliders != null)
            {
                foreach (Collider c in excludeColliders)
                {
                    c.enabled = true;
                }
            }

            if (forceIncludeColliders != null)
            {
                for (int i = 0; i < forceIncludeColliders.Count; i++)
                {
                    forceIncludeColliders[i].gameObject.layer = forceIncludeLayers[i];
                }
            }

            return hits;
        }
    }

   

    public class RayCrafterHit{
        public Transform transform;
        public Vector3 entryPoint;
        public Vector3 exitPoint;
        public Vector3 entryNormal;
        public Vector3 exitNormal;
        public Vector3 exitDir;
        public float thickness;
        public float passThroughValue;
    }
}
