using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RayCrafting
{
    public class RayCrafter_SpecialObject : MonoBehaviour
    {
        public ObjectBehavior objectBehavior = ObjectBehavior.doNothing;
        public float density = 1;
        public int material = 0;
        public Collider thisCollider;

        private void Awake()
        {
            if (thisCollider)
            {
                RayCrafter.specialObjectDictionary.Add(transform, this);
                RayCrafter.specialObjectList.Add(thisCollider);
            }else{
                Debug.Log("Special Object Doesn't Have Collider!");
            }
        }

		private void OnDestroy()
		{
            if (thisCollider)
            {
                RayCrafter.specialObjectList.Remove(thisCollider);
                RayCrafter.specialObjectDictionary.Remove(transform);
            }
		}
	}
}
