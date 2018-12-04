using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingSnapper : MonoBehaviour {
    public NewBuildingScript.BuildingTypes buildingType;
    public bool faceCamera;

	private void Awake()
	{
        if (NewBuildingScript.instance)
            NewBuildingScript.instance.AddSnapper(this);
	}

    void OnEnable()
	{
        if (NewBuildingScript.instance)
        {
            if (NewBuildingScript.instance.structures[buildingType].ContainsKey(transform.position))
            {
                gameObject.SetActive(false);
            }
        }
	}
    void OnDestroy(){
        if(NewBuildingScript.instance)
            NewBuildingScript.instance.StartCoroutine(NewBuildingScript.instance.RemoveSnapper(this));
    }
}
