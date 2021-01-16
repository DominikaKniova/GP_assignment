using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkedWorldManager : MonoBehaviour
{
    public const int chunkSize = 16;
    public const int numChunk = 6;
    public const int worldSize = chunkSize * numChunk;

    public GameObject chunkPrefab;

    public ChunkObject[,,] chunks = new ChunkObject[numChunk, numChunk, numChunk];
    public static int[,] heightMap = new int[worldSize, worldSize];
    void Awake()
    {
        // initialize world
        InitChunks();

        // generate random terrain
        heightMap = TerrainManager.GenerateHeightMap(worldSize, worldSize, 10);

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

    public Vector3Int World2ChunkCoords(Vector3 position)
    {
        Vector3Int chunkCoords = Vector3Int.FloorToInt(position);
        chunkCoords.x /= chunkSize;
        chunkCoords.y /= chunkSize;
        chunkCoords.z /= chunkSize;
        return chunkCoords;
    }

    public Vector3Int World2BlockCoords(Vector3 position)
    {
        Vector3Int blockCoords = Vector3Int.FloorToInt(position);
        blockCoords.x %= chunkSize;
        blockCoords.y %= chunkSize;
        blockCoords.z %= chunkSize;
        return blockCoords;
    }

    public int GetBlockType(RaycastHit hit)
    {
        // chunk position and block position in local/chunk/block coords
        Vector3Int chunkPosition = World2ChunkCoords(hit.transform.position);
        Vector3Int blockPosition = World2BlockCoords(hit.point);
        return chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].GetBlockType(blockPosition);
    }

    public Vector3 GetWireframePosition(RaycastHit hit)
    {
        // chunk position and new block position in local/chunk/block coords
        Vector3Int chunkPosition = World2ChunkCoords(hit.transform.position);
        Vector3Int blockPosition = World2BlockCoords(hit.point);

        // spawn position for a new block
        Vector3 spawnBlockPosition = blockPosition + hit.normal / 2.0f;

        // check whether spawn position is in current chunk or in neghbourhood one and adjust chunk and spawn position
        if (isOutsideChunk(ref spawnBlockPosition, ref chunkPosition))
        {
            Debug.Log("Outside chunk");
            // check whether a new chunk needs to be added TODO !!!
        }

        return new Vector3(chunkPosition.x * chunkSize + (int)spawnBlockPosition.x, chunkPosition.y * chunkSize + (int)spawnBlockPosition.y, chunkPosition.z * chunkSize + (int)spawnBlockPosition.z);
    }

    public void DestroyBlock(RaycastHit hit)
    {
        // chunk position and block position in local/chunk/block coords
        Vector3Int chunkPosition = World2ChunkCoords(hit.transform.position);
        Vector3Int blockPosition = World2BlockCoords(hit.point - hit.normal / 2.0f);

        chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].DestroyBlock(blockPosition);
    }

    public void AddBlock(RaycastHit hit, int blockType)
    {
        // chunk position and new block position in local/chunk/block coords
        Vector3Int chunkPosition = World2ChunkCoords(hit.transform.position);
        Vector3Int blockPosition = World2BlockCoords(hit.point);

        // spawn position for a new block
        Vector3 spawnBlockPosition = blockPosition + hit.normal / 2.0f;

        // check whether spawn position is in current chunk or in neghbourhood one and adjust chunk and spawn position
        if (isOutsideChunk(ref spawnBlockPosition, ref chunkPosition))
        {
            Debug.Log("Outside chunk");
            // check whether a new chunk needs to be added TODO !!!
        }

        // add block to chunk
        chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].AddBlock(spawnBlockPosition, blockType);
    }

    private bool isOutsideChunk(ref Vector3 blockPosition, ref Vector3Int chunkPosition)
    {
        if (blockPosition.x < 0.0f)
        {
            blockPosition.x = chunkSize - 1;
            chunkPosition += Vector3Int.left;
            return true;
        }
        if (blockPosition.y < 0.0f)
        {
            blockPosition.y = chunkSize - 1;
            chunkPosition += Vector3Int.down;
            return true;
        }
        if (blockPosition.z < 0.0f)
        {
            blockPosition.z = chunkSize - 1;
            chunkPosition += new Vector3Int(0, 0, -1);
            return true;
        }

        if (blockPosition.x > 15.0f)
        {
            blockPosition.x = 0;
            chunkPosition += Vector3Int.right;
            return true;
        }
        if (blockPosition.y > 15.0f)
        {
            blockPosition.y = 0;
            chunkPosition += Vector3Int.up;
            return true;
        }
        if (blockPosition.z > 15.0f)
        {
            blockPosition.z = 0;
            chunkPosition += new Vector3Int(0, 0, 1);
            return true;
        }

        return false;
    }

    public int GetHeightForPosition(Vector3 position)
    {
        Vector3Int idx = Vector3Int.FloorToInt(position);
        return heightMap[idx.x, idx.z];
    }

    private void ClearChunks()
    {
        for (int y = 0; y < numChunk; y++)
            for (int x = 0; x < numChunk; x++)
                for (int z = 0; z < numChunk; z++)
                {
                    chunks[x, y, z].ClearChunk();
                }
    }
    public void ReGenerateWorld()
    {
        // empty scene
        ClearChunks();
        Debug.Log("done clear");

        // regenerate new height map for terrain
        heightMap = TerrainManager.GenerateHeightMap(worldSize, worldSize, 10);

        FillWorldWithChunks();
    }

}
