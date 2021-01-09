using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public GameObject blockPrefab;
    public GameObject wireframeBlockPrefab;
    public GameObject ground;

    private GridCell[,,] grid;

    private const int xDim = 100;
    private const int yDim = 100;
    private const int zDim = 100;

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
        InitGrid();

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
        TestSpawning();
    }

    private void InitGrid()
    {
        grid = new GridCell[xDim, yDim, zDim];

        for (int i = 0; i < xDim; i++)
        {
            for (int j = 0; j < yDim; j++)
            {
                for (int k = 0; k < zDim; k++)
                {
                    grid[i, j, k] = new GridCell();
                }
            }
        }
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

    private void TestSpawning()
    {
        for (int i = 0; i < 1; i+=2)
        {
            for (int j = 0; j < zDim; j+=2)
            {
                for (int k = 0; k < xDim; k+=2)
                {
                    Vector3 spawnPosition = new Vector3(k * step + minX + center, i * step + center, j * step + minZ + center);
                    GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
                    block.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
                }
            }
        }
    }

    public void AddBlock(Vector3 position, int type)
    {
        Vector3Int idx = World2Idx(position);

        grid[idx.x, idx.y, idx.z].occupied = true;
        grid[idx.x, idx.y, idx.z].blockType = type;

        Vector3 spawnPosition = Idx2SpawnPosition(idx);

        GameObject block = Instantiate(blockPrefab, spawnPosition, Quaternion.identity);
        block.GetComponent<Renderer>().material.color = new Color(Random.value, Random.value, Random.value);
        block.transform.localScale = new Vector3(cubeSize, cubeSize, cubeSize);
    }

    public void DestroyBlock(int x, int y, int z)
    {
        grid[x, y, z].occupied = false;
        grid[x, y, z].blockType = -1;
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

    public bool CanSnap(Vector3 position)
    {
        // check if it has at least one neighbour

        // convert position to grid indices
        Vector3Int idx = World2Idx(position);



        return true;
    }
}
