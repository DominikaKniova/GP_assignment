using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkedWorldManager : MonoBehaviour
{
    public const int chunkSize = 16;
    public const int numChunk = 6;
    public const int worldSize = chunkSize * numChunk;

    public GameObject chunkPrefab;
    public GameObject wireframeBlockPrefab;

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
        Vector3Int blockPosition = World2BlockCoords(hit.point - hit.normal / 2.0f);

        return chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].GetBlockType(blockPosition);
    }

    public GameObject SnapWireframeBlock(RaycastHit hit, Vector3 playerPosition)
    {
        // check if hit point is out of world bounds
        if (isOutOfWorld(hit.point)) return null;

        // position of a new block in world coords
        Vector3 blockPositionWorld = hit.point + hit.normal / 2.0f;

        // get coords of a new block int block coords
        Vector3Int blockPosition = World2BlockCoords(blockPositionWorld);
        // get coords of the chunk where a new block will be spawned in chunk coords
        Vector3Int chunkPosition = World2ChunkCoords(blockPositionWorld);

        // show wireframe block (if player is not colliding with it)
        if (!isCollidingWithPlayer(playerPosition, blockPosition, chunkPosition))
        {
            Vector3 spawnPosition = 0.5f * Vector3.one + new Vector3(chunkPosition.x * chunkSize + blockPosition.x, 
                chunkPosition.y * chunkSize + blockPosition.y, chunkPosition.z * chunkSize + blockPosition.z);
            
            return Instantiate(wireframeBlockPrefab, spawnPosition, Quaternion.identity);
        }
        return null;
    }

    public void DestroyBlock(RaycastHit hit)
    {
        // chunk position and block position in local/chunk/block coords
        Vector3Int chunkPosition = World2ChunkCoords(hit.transform.position);
        Vector3Int blockPosition = World2BlockCoords(hit.point - hit.normal / 2.0f);

        chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].DestroyBlock(blockPosition);
    }

    private bool isCollidingWithPlayer(Vector3 playerPosition, Vector3Int blockPosition, Vector3Int chunkPosition)
    {
        // get position of player in chunk and block coordinates
        Vector3Int playerPosChunk = World2ChunkCoords(playerPosition);
        Vector3Int playerPosBlock = World2BlockCoords(playerPosition);

        if (playerPosChunk.x == chunkPosition.x && playerPosChunk.z == chunkPosition.z)
            if (playerPosBlock.x == blockPosition.x && playerPosBlock.z == blockPosition.z)
                return true;
            else return false;
        else return false;
    }

    private bool isOutOfWorld(Vector3 pos)
    {
        if (pos.x <= 0 || pos.x >= worldSize || pos.y <= 0 || pos.y >= worldSize || pos.z <= 0 || pos.z >= worldSize)
            return true;
        return false;
    }

    public void AddBlock(RaycastHit hit, int blockType, Vector3 playerPosition)
    {
        // check if hit point is out of world bounds
        if (isOutOfWorld(hit.point)) return;

        // position of a new block in world coords
        Vector3 spawnPosition = hit.point + hit.normal / 2.0f;

        // get coords of a new block int block coords
        Vector3Int blockPosition = World2BlockCoords(spawnPosition);
        // get coords of the chunk where a new block will be spawned in chunk coords
        Vector3Int chunkPosition = World2ChunkCoords(spawnPosition);

        // add block to chunk (if player is not colliding with the new block)
        if ( !isCollidingWithPlayer(playerPosition, blockPosition, chunkPosition))
            chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].AddBlock(blockPosition, blockType);
    }

    public int GetHeightForPosition(int x, int z)
    {
        return heightMap[x, z];
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

    private void ClearHeightMap()
    {
        for (int x = 0; x < worldSize; x++)
            for (int z = 0; z < worldSize; z++)
            {
                heightMap[x, z] = 0;
            }
    }
    public void ReGenerateWorld()
    {
        // destroy current world
        ClearChunks();
        Debug.Log("done clear");

        // regenerate new height map for new terrain
        heightMap = TerrainManager.GenerateHeightMap(worldSize, worldSize, 10);

        // build new world
        FillWorldWithChunks();
    }

    public void EmptyWorld()
    {
        // empty scene
        ClearChunks();
    }

}
