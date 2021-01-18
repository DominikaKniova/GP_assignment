using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Vector3 ToVector3()
    {
        return new Vector3(x, y, z);
    }
}

[System.Serializable]
public class ChunkData
{
    public List<Vector3S> blockPositions = new List<Vector3S>();
    public List<int> blockTypes = new List<int>();
}

[System.Serializable]
public class SaveData
{
    public Dictionary<Vector3S, ChunkData> chunks = new Dictionary<Vector3S, ChunkData>();
    public int[,] heightMap = new int[WorldManager.worldSize, WorldManager.worldSize];
    public Vector3S playerPosition;
}
