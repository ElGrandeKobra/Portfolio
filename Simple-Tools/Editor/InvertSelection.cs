using UnityEngine;
using UnityEditor;

using System.Collections.Generic;
using System;
using System.Linq;

public class InvertSelection : ScriptableWizard
{


    [MenuItem("Selection/Invert")]
    static void static_InvertSelection()
    {

        List<GameObject> oldSelection = new List<GameObject>();
        List<GameObject> newSelection = new List<GameObject>();


        foreach (GameObject obj in Selection.GetFiltered(typeof(GameObject), SelectionMode.ExcludePrefab))
            oldSelection.Add(obj);

        foreach (GameObject obj in FindObjectsOfType(typeof(GameObject)))
        {
            if (!oldSelection.Contains(obj))
                newSelection.Add(obj);
        }

        Selection.objects = newSelection.ToArray();

    }

    [MenuItem("Selection/Revert to Prefab")]
    static void Revert()
    {
        GameObject[] selection = Selection.gameObjects;

        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                PrefabUtility.RevertPrefabInstance(selection[i]);
            }
        }
        else
        {
            Debug.Log("Cannot revert to prefab - nothing selected");
        }
    }


    [MenuItem("Selection/Randomize Selection")]
    static void RandomizeCurrentSelection()
    {
        GameObject[] selection = Selection.gameObjects;
        List<GameObject> newSelection = new List<GameObject>();
        if (selection.Length > 0)
        {
            for (int i = 0; i < selection.Length; i++)
            {
                if (UnityEngine.Random.Range(0, 100) < 50)
                {
                    newSelection.Add(selection[i]);
                }
            }
        }
        Selection.objects = newSelection.ToArray();
    }
}


public class MirrorHelper : ScriptableWizard
{

    [MenuItem("NetworkManager/Gather Objects Into Lobby Manager")]
    static void GatherObjects()
    {
        // Find all assets labelled with 'architecture' :
        string[] guids1 = AssetDatabase.FindAssets("t:GameObject", null);
        Mirror.NetworkManager manager = FindObjectOfType<Mirror.NetworkManager>();
        if (manager == null)
        {
            Debug.Log("No manager!");
            return;
        }
        List<GameObject> objectsToAdd = new List<GameObject>();
        foreach (string guid1 in guids1)
        {
            string url = AssetDatabase.GUIDToAssetPath(guid1);
            Type type = AssetDatabase.GetMainAssetTypeAtPath(url);
            if (type == typeof(GameObject))
            {
                GameObject g = AssetDatabase.LoadAssetAtPath(url, type) as GameObject;
                if (g.GetComponent<Mirror.NetworkIdentity>())
                {
                    if (!manager.spawnPrefabs.Contains(g))
                    {
                        Debug.Log("Assigning " + g.name);
                        manager.spawnPrefabs.Add(g);
                    }
                    else
                    {
                        Debug.Log(g.name + " already assigned");
                    }
                }
            }
        }
    }

    [MenuItem("NetworkManager/Order Spawn Prefabs Alphabetically")]
    static void OrderAlpha()
    {
        // Find all assets labelled with 'architecture' :
        string[] guids1 = AssetDatabase.FindAssets("t:GameObject", null);
        Mirror.NetworkManager manager = FindObjectOfType<Mirror.NetworkManager>();
        if (manager == null)
        {
            Debug.Log("No manager!");
            return;
        }
        List<GameObject> objectsToAdd = manager.spawnPrefabs;
        objectsToAdd = objectsToAdd.OrderBy(x => x.name.ToLower()).ToList<GameObject>();
        manager.spawnPrefabs = objectsToAdd;
    }

    [MenuItem("NetworkManager/Assign Network Identities")]
    static void AssignNetworkIdentities()
    {
        // Find all assets labelled with 'architecture' :
        string[] guids1 = AssetDatabase.FindAssets("t:GameObject", null);
        Mirror.NetworkManager manager = FindObjectOfType<Mirror.NetworkManager>();
        if (manager == null)
        {
            Debug.Log("No manager!");
            return;
        }

        foreach (string guid1 in guids1)
        {
            string url = AssetDatabase.GUIDToAssetPath(guid1);
            Type type = AssetDatabase.GetMainAssetTypeAtPath(url);
            if (type == typeof(GameObject))
            {
                GameObject g = AssetDatabase.LoadAssetAtPath(url, type) as GameObject;

                Component[] components = g.GetComponents<Component>();
                bool needsToAddNetID = false;
                List<Component> componentsToDestroy = new List<Component>();
                foreach (Component c in components)
                {
                    if (c == null)
                    {
                        componentsToDestroy.Add(c);
                        needsToAddNetID = true;
                        Debug.Log(g.name);
                    }
                }
                for (int i = 0; i < componentsToDestroy.Count; i++){
                    DestroyImmediate(componentsToDestroy[i], true);
                }
                if (g.GetComponent<Mirror.NetworkIdentity>() == null)
                {
                    if (needsToAddNetID)
                    {
                        g.AddComponent<Mirror.NetworkIdentity>();
                    }
                }
            }
        }
    }
}