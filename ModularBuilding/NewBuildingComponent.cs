using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuildingComponent : MonoBehaviour {
    public NewBuildingScript.BuildingTypes buildingType;
    public GameObject gib;
    private void Start()
    {
        if (NewBuildingScript.instance)
            NewBuildingScript.instance.AddBuildingComponent(this);
    }

    private void OnDestroy()
    {
        if (NewBuildingScript.instance)
        {
            Instantiate(gib, transform.position, Quaternion.identity);
            NewBuildingScript.instance.RemoveBuildingComponent(this);
            NewBuildingScript.instance.EnableLastSnapperAtPos(buildingType, transform.position);
        }
    }
}
