using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    // world parameters
    public const int chunkSize = 16;
    public const int numChunks = 6;
    public const int worldSize = chunkSize * numChunks;
    public static int[,] heightMap = new int[worldSize, worldSize];

    // references to prefabs that will be spawned
    public GameObject chunkPrefab;
    public GameObject wireframeBlockPrefab;

    // 3D grid storing all chunks in the world
    public ChunkObject[,,] chunks = new ChunkObject[numChunks, numChunks, numChunks];

    void Awake()
    {
        // initialize world
        InitChunks();

        // generate random terrain
        heightMap = TerrainGenerator.GenerateHeightMap(worldSize, worldSize, 10);

        FillWorldWithChunks();
    }

    /* Initialize 3D grid of chunks */
    private void InitChunks()
    {
        for (int y = 0; y < numChunks; y++)
            for (int x = 0; x < numChunks; x++)
                for (int z = 0; z < numChunks; z++)
                {
                    chunks[x, y, z] = new ChunkObject(new Vector3Int(x * chunkSize, y * chunkSize, z * chunkSize), chunkPrefab);
                }
    }

    private void FillWorldWithChunks()
    {
        for (int y = 0; y < numChunks; y++)
            for (int x = 0; x < numChunks; x++)
                for (int z = 0; z < numChunks; z++)
                {
                    chunks[x, y, z].CreateChunkObject();
                }
    }

    /* Convert postion in world coordinates to position in chunk coordinates [0 ... chunkSize - 1])^3 */
    private Vector3Int World2ChunkCoords(Vector3 position)
    {
        Vector3Int chunkCoords = Vector3Int.FloorToInt(position);
        chunkCoords.x /= chunkSize;
        chunkCoords.y /= chunkSize;
        chunkCoords.z /= chunkSize;
        return chunkCoords;
    }

    /* Convert postion in world coordinates to position in block coordinates [0 ... numChunks - 1])^3 */
    private Vector3Int World2BlockCoords(Vector3 position)
    {
        Vector3Int blockCoords = Vector3Int.FloorToInt(position);
        blockCoords.x %= chunkSize;
        blockCoords.y %= chunkSize;
        blockCoords.z %= chunkSize;
        return blockCoords;
    }
    public GameObject SnapWireframeBlock(RaycastHit hit, Vector3 playerPosition)
    {
        // check if hit point is out of world bounds
        if (isOutOfWorld(hit.point)) return null;

        // position of a new block in world coords
        Vector3 blockPositionWorld = hit.point + hit.normal / 2.0f;

        // get coords of a new block in block coords
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

        // if block is in the lowest layer of terrain -> do not destroy
        if (!isInLowestLayer(blockPosition))
            chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].DestroyBlock(blockPosition);
    }
            

    public void AddBlock(RaycastHit hit, byte blockType, Vector3 playerPosition)
    {
        // check if hit point is out of world bounds
        if (isOutOfWorld(hit.point)) return;

        // position of a new block in world coords
        Vector3 spawnPosition = hit.point + hit.normal / 2.0f;

        // get coords of a new block in block coords
        Vector3Int blockPosition = World2BlockCoords(spawnPosition);
        // get coords of the chunk where a new block will be spawned in chunk coords
        Vector3Int chunkPosition = World2ChunkCoords(spawnPosition);

        // add block to chunk (if player is not colliding with the new block)
        if ( !isCollidingWithPlayer(playerPosition, blockPosition, chunkPosition))
            chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].AddBlock(blockPosition, blockType);
    }

    /* Check if position is out of world bounds */
    private bool isOutOfWorld(Vector3 pos)
    {
        if (pos.x <= 0 || pos.x >= worldSize || pos.y <= 0 || pos.y >= worldSize || pos.z <= 0 || pos.z >= worldSize)
            return true;
        return false;
    }

    /* Check if player is positioned at the same place where it wants to add/show block */
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

    /* Check if block is in lowest layer of the world */
    private bool isInLowestLayer(Vector3Int blockPosition)
    {
        if (blockPosition.y == 0)
                return true;
        return false;
    }

    public float GetDestroyTime(RaycastHit hit)
    {
        // chunk position and block position in chunk/block coords
        Vector3Int chunkPosition = World2ChunkCoords(hit.transform.position);
        Vector3Int blockPosition = World2BlockCoords(hit.point - hit.normal / 2.0f);

        byte type = chunks[chunkPosition.x, chunkPosition.y, chunkPosition.z].GetBlockType(blockPosition);

        return BlockData.destroyTimes[BlockData.numType2string[type]];
    }

    public int GetHeightForPosition(int x, int z)
    {
        return heightMap[x, z];
    }

    private void ClearChunks()
    {
        for (int y = 0; y < numChunks; y++)
            for (int x = 0; x < numChunks; x++)
                for (int z = 0; z < numChunks; z++)
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


    /* Create new world with new random height map */
    public void ReGenerateWorld()
    {
        // destroy current world
        ClearChunks();

        // regenerate new height map for new terrain
        heightMap = TerrainGenerator.GenerateHeightMap(worldSize, worldSize, 10);

        // build new world
        FillWorldWithChunks();
    }

    public void EmptyWorld()
    {
        // destroy current world and its height map
        ClearChunks();
        ClearHeightMap();
    }


    /* Get random position in the world that is high (used for player's initial position ... to see the beauty of the world) */
    public Vector3 GetHighPosition()
    {
        for (int x = worldSize/2; x < worldSize; x++)
            for (int z = worldSize / 2; z < worldSize; z++)
            {
                if (heightMap[x, z] >= 18)
                {
                    return new Vector3(x, heightMap[x, z], z);
                }
            }
        return Vector3.zero;
    }

}
