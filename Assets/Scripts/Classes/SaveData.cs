using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Vector3S
{
    public float x;
    public float y;
    public float z;

    public Vector3S(Vector3 v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}
[System.Serializable]
public class SaveData
{
    public List<Vector3S> blockPositions = new List<Vector3S>();
    public List<int> blockTypes = new List<int>();
    public Vector3S playerPosition;
}
