using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkedWorldManager : MonoBehaviour
{
    public const int chunkSize = 8;
    public const int numChunk = 3;
    public const int worldSize = chunkSize * numChunk;

    public GameObject chunkPrefab;

    private ChunkObject[,,] chunks = new ChunkObject[numChunk, numChunk, numChunk];
    private int[,] heightMap = new int[worldSize, worldSize];
    

    void Awake()
    {
        InitChunks();
        FillWorldWithChunks();
    }

    private void InitChunks()
    {
        for (int y = 0; y < numChunk; y++)
            for (int x = 0; x < numChunk; x++)
                for (int z = 0; z < numChunk; z++)
                {
                    chunks[x, y, z] = new ChunkObject(new Vector3(x * chunkSize, y * chunkSize, z * chunkSize), chunkPrefab);
                }
    }

    private void FillWorldWithChunks()
    {
        for (int y = 0; y < numChunk; y++)
            for (int x = 0; x < numChunk; x++)
                for (int z = 0; z < numChunk; z++)
                {
                    chunks[x, y, z].CreateChunkObject();
                }
    }

}
