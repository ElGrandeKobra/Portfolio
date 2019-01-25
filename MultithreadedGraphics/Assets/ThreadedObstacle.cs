using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadedObstacle : MonoBehaviour {
    public float radius;
    /// <summary>
    /// Set to 0 for inner obstacles and a higher number like .6 for outer ones
    /// </summary>
    [Range(0,3)]
    public float offset = .6f;
    [HideInInspector] public Vector3 pos;
    Transform trans;
    int index = -1;
    [HideInInspector] public Vector3 dir;
    [HideInInspector] public Vector3 dirNorm;
    [HideInInspector] public float sumRad;
    [HideInInspector] public float dist;
    [HideInInspector] public float cosAngle;

	private void updatePosition()
	{
        pos = trans.position;
	}

	private void OnEnable()
	{
        trans = transform;
        if(ThreadedLODS.instance){
            bool found = false;
            for (int i = 0; i < ThreadedLODS.instance.threadedObstacles.Count;i++){
                if(ThreadedLODS.instance.threadedObstacles[i] == null){
                    index = i;
                    ThreadedLODS.instance.threadedObstacles[i] = this;
                    found = true;
                    continue;
                }
            }
            if(!found){
                  ThreadedLODS.instance.threadedObstacles.Add(this);
            }
        }else{
            Invoke("OnEnable", 1);
        }
        updatePosition();
	}

	private void OnDestroy()
	{
        if (ThreadedLODS.instance)
        {
            ThreadedLODS.instance.threadedObstacles[ThreadedLODS.instance.threadedObstacles.IndexOf(this)] = null;
        }
	}
	private void OnDrawGizmos()
	{
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, radius + offset + .01f);
	}
}
