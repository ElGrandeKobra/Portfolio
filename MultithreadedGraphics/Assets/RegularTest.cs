using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RegularTest : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        for (int i = 0; i < 6000; i++)
        {
            
            ThreadedLODS.ObjectData data = new ThreadedLODS.ObjectData();
            data.position = UnityEngine.Random.insideUnitSphere * 10;
            data.rotation = UnityEngine.Random.rotation;
            data.scale = new Vector3(1, 1, 1);

            GameObject g = (GameObject)Instantiate(prefab, data.position, data.rotation);
            g.transform.localScale = data.scale;
            g.transform.parent = transform;

        }
	}

}
