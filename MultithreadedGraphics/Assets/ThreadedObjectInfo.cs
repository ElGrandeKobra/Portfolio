using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThreadedObjectInfo : MonoBehaviour {
    public Mesh[] meshes;
    public Material[] materials;
    public float[] renderDistances;
    public float[] qualitySettingMultipliers = { .1f, .2f, .4f, .6f, .8f, 1 };
}
