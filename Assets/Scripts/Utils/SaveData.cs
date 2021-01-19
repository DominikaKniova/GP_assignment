using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Serializable version of Vector3Int */
[System.Serializable]
public struct Vector3S
{
    public int x;
    public int y;
    public int z;
    public Vector3S(Vector3Int v)
    {
        x = v.x;
        y = v.y;
        z = v.z;
    }
    public Vector3S(int _x, int _y, int _z)
    {
        x = _x;
        y = _y;
        z = _z;
    }
}

/* Data structure for creating saves */
[System.Serializable]
public class ChunkData
{
    public List<Vector3S> blockPositions = new List<Vector3S>();
    public List<byte> blockTypes = new List<byte>();
}

/* Data structure for creating saves */
[System.Serializable]
public class SaveData
{
    public Dictionary<Vector3S, ChunkData> chunks = new Dictionary<Vector3S, ChunkData>();
    public Vector3S playerPosition;
}
