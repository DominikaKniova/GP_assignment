using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    public GameObject wireframeBlockPrefab;
    public GameObject ground;

    private const int xDim = 50;
    private const int yDim = 50;
    private const int zDim = 50;

    int[,] heightMap = new int[xDim, zDim];

    private float minX;
    private float maxX;
    private float minY;
    private float maxY;
    private float minZ;
    private float maxZ;
    private float cubeSize = 1.0f;
    private float center;
    private float step;
    private float range;

    void Awake()
    {
        List<Vector3> corners = GroundCorners();
        minX = -corners[0].x;
        maxX = corners[0].x;
        minY = -corners[0].y;
        maxY = corners[0].y;
        minZ = -corners[0].z;
        maxZ = corners[0].z;

        range = maxX - minX;
        cubeSize = range / xDim;
        step = cubeSize;
        center = cubeSize / 2;

        //RandomTerrain();
    }
    private List<Vector3> GroundCorners()
    {
        List<Vector3> corners = new List<Vector3>(); ;
        List<Vector3> groundVertices = new List<Vector3>(ground.GetComponent<MeshFilter>().sharedMesh.vertices);
        corners.Add(ground.transform.TransformPoint(groundVertices[0]));
        corners.Add(ground.transform.TransformPoint(groundVertices[10]));
        corners.Add(ground.transform.TransformPoint(groundVertices[110]));
        corners.Add(ground.transform.TransformPoint(groundVertices[120]));
        groundVertices.Clear();
        return corners;
    }

    private void RandomTerrain()
    {
        heightMap = TerrainManager.GenerateHeightMap(xDim, zDim, 10);
        for (int z = 0; z < zDim; z++)
        {
            for (int x = 0; x < xDim; x++)
            {
                for (int y = 0; y < heightMap[z, x]; y++)
                {
                    Vector3 spawnPosition = new Vector3(x * step + minX + center, y * step + minY + center, z * step + minZ + center);
                    GameObject block = Instantiate(blockPrefabs[0], spawnPosition, Quaternion.identity);
                    block.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);

                    if (y <= TerrainManager.waterLevel)
                    {
                        block.GetComponent<Renderer>().material.color = new Color(0, 0, 255);
                    }
                    else if (y > TerrainManager.greeneryLevel)
                    {
                        block.GetComponent<Renderer>().material.color = new Color(255, 255, 255);
                    }
                }
            }
        }
    }

    private void TestSpawning()
    {
        for (int i = 0; i < 1; i+=2)
        {
            for (int j = 0; j < zDim; j+=2)
            {
                for (int k = 0; k < xDim; k+=2)
                {
                    Vector3 spawnPosition = new Vector3(k * step + minX + center, i * step + center, j * step + minZ + center);
                    GameObject block = Instantiate(blockPrefabs[0], spawnPosition, Quaternion.identity);
                    block.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
                }
            }
        }
    }

    public void AddBlock(Vector3 position, int type)
    {
        Vector3Int idx = World2Idx(position);

        Vector3 spawnPosition = Idx2SpawnPosition(idx);

        GameObject block = Instantiate(blockPrefabs[type], spawnPosition, Quaternion.identity);
        block.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
    }

    public void DestroyBlock(int x, int y, int z)
    {
        // Destroy(...)
    }

    public Vector3Int World2Idx(Vector3 position)
    {
        return Vector3Int.FloorToInt(new Vector3(position.x + maxX, Mathf.Max(position.y + maxY, 0.0f), position.z + maxZ));
    }

    public Vector3 Idx2SpawnPosition(Vector3Int idx)
    {
        return new Vector3(idx.x + minX + center, idx.y + minY + center, idx.z + minZ + center);
    }

    public GameObject SnapWireframe(Vector3 position)
    {
        // convert position to grid indices
        Vector3Int idx = World2Idx(position);
        Vector3 spawnPosition = Idx2SpawnPosition(idx);
        GameObject block = Instantiate(wireframeBlockPrefab, spawnPosition, Quaternion.identity);
        block.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
        return block;
    }

    public void ResetWorld()
    {
        // empty scene
        foreach (GameObject block in GameObject.FindGameObjectsWithTag("Block"))
        {
            Destroy(block);
        }
        RandomTerrain();
    }

    public int GetHeightForPosition(Vector3 position)
    {
        Vector3Int idx = World2Idx(position);
        return heightMap[idx.z, idx.x];
    }
}
