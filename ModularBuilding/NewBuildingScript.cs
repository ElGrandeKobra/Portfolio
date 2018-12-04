using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBuildingScript : MonoBehaviour {

    public enum BuildingTypes
    {
        foundation,
        wall,
        ceiling,
        centerPiece
    }

    public LayerMask layerMask;

    public GameObject[] buildingComponents;
    public Transform child;

    public Dictionary<BuildingTypes, Dictionary<Vector3, List<BuildingSnapper>>> buildingSnappers = new Dictionary<BuildingTypes, Dictionary<Vector3, List<BuildingSnapper>>>();
    public Dictionary<BuildingTypes, Dictionary<Vector3, NewBuildingComponent>> structures = new Dictionary<BuildingTypes, Dictionary<Vector3, NewBuildingComponent>>();
    bool alreadyInitiatedDictionaries;
    static public NewBuildingScript instance;

    public float delayDominoTime = .1f;

    //Add snapper
    public void AddSnapper(BuildingSnapper snapper){
        if (buildingSnappers[snapper.buildingType].ContainsKey(snapper.transform.position) == false)
        {
            buildingSnappers[snapper.buildingType].Add(snapper.transform.position, new List<BuildingSnapper>());
        }
        else
        { //Deactivate last one at this position
            List<BuildingSnapper> snapperList = buildingSnappers[snapper.buildingType][snapper.transform.position];
            if (snapperList != null)
            {
                if (snapperList.Count > 1)
                {
                    snapperList[snapperList.Count - 1].gameObject.SetActive(false);
                }
            }
        }

        buildingSnappers[snapper.buildingType][snapper.transform.position].Add(snapper);

    }

    //Clear snapped from cache and clean up any dictionaries
    public IEnumerator RemoveSnapper(BuildingSnapper snapper){

        buildingSnappers[snapper.buildingType][snapper.transform.position].Remove(snapper);
        Vector3 pos = snapper.transform.position;
        BuildingTypes type = snapper.buildingType;

        //No more supports exist at this snapper's position - if a building component exists there, destroy it
        if (buildingSnappers[type][pos].Count == 0)
        {
            if (type!= BuildingTypes.foundation)
            {
                if (structures[type].ContainsKey(pos))
                {
                    NewBuildingComponent component = structures[type][pos];
                    yield return new WaitForSeconds(delayDominoTime);
                    if (component)
                    {
                        Destroy(component.gameObject);
                    }
                }
            }
            buildingSnappers[type].Remove(pos);
        }else{//Activate the most recently instantiated snapper at this position
            EnableLastSnapperAtPos(type, pos);
        }
    }

    public void EnableLastSnapperAtPos(BuildingTypes type, Vector3 pos){
        if (buildingSnappers[type].ContainsKey(pos))
        {
            List<BuildingSnapper> snapperList = buildingSnappers[type][pos];
            if (snapperList.Count > 0)
            {
                snapperList[snapperList.Count - 1].gameObject.SetActive(true);
            }
        }
    }


    //Add building component
    public void AddBuildingComponent(NewBuildingComponent component)
    {
        structures[component.buildingType].Add(component.transform.position, component);
    }

    //Clear snapped from cache and clean up any dictionaries
    public void RemoveBuildingComponent(NewBuildingComponent component)
    {
        structures[component.buildingType].Remove(component.transform.position);
    }

	private void Awake()
	{
        instance = this;
        if (!alreadyInitiatedDictionaries)
        {
            buildingSnappers.Add(BuildingTypes.foundation, new Dictionary<Vector3, List<BuildingSnapper>>());
            buildingSnappers.Add(BuildingTypes.wall, new Dictionary<Vector3, List<BuildingSnapper>>());
            buildingSnappers.Add(BuildingTypes.ceiling, new Dictionary<Vector3, List<BuildingSnapper>>());
            buildingSnappers.Add(BuildingTypes.centerPiece, new Dictionary<Vector3, List<BuildingSnapper>>());

            structures.Add(BuildingTypes.foundation, new Dictionary<Vector3, NewBuildingComponent>());
            structures.Add(BuildingTypes.wall, new Dictionary<Vector3, NewBuildingComponent>());
            structures.Add(BuildingTypes.ceiling, new Dictionary<Vector3, NewBuildingComponent>());
            structures.Add(BuildingTypes.centerPiece, new Dictionary<Vector3, NewBuildingComponent>());

            alreadyInitiatedDictionaries = true;
        }
	}

	// Update is called once per frame
	void Update () {
        if(Input.GetKeyDown(KeyCode.Mouse0)){
            RaycastHit hit = new RaycastHit();
            if(Physics.Raycast(transform.position,child.forward, out hit, 100, layerMask, QueryTriggerInteraction.Ignore)){
                BuildingSnapper snapper = hit.transform.GetComponentInParent<BuildingSnapper>();
                if(snapper){
                    if(snapper.faceCamera){
                        Vector3 one = new Vector3(transform.position.x, 0, transform.position.z);
                        Vector3 two = new Vector3(snapper.transform.position.x, 0, snapper.transform.position.z);
                        snapper.transform.rotation = Quaternion.LookRotation(one - two, Vector3.up);
                        snapper.transform.localEulerAngles = new Vector3(0, Mathf.Round(snapper.transform.localEulerAngles.y / 90)*90, 0);
                    }
                    Instantiate(buildingComponents[(int)snapper.buildingType], snapper.transform.position, snapper.transform.rotation);
                    snapper.gameObject.SetActive(false);
                }
            }
        } else if(Input.GetKeyDown(KeyCode.Mouse1)){
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(transform.position, child.forward, out hit, 100, layerMask, QueryTriggerInteraction.Ignore))
            {
                if(hit.transform.GetComponent<NewBuildingComponent>()){
                    Destroy(hit.transform.GetComponentInParent<NewBuildingComponent>().gameObject);
                }
            }
        }
	}
}
