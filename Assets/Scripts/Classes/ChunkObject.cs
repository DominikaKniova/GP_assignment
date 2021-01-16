using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObject
{
    private Mesh mesh;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> UVs = new List<Vector2>();

    private const int chunkSize = ChunkedWorldManager.chunkSize;
    private int[,,] chunkGrid = new int[chunkSize, chunkSize, chunkSize];

    public Vector3 position;
    private GameObject chunkGameObj;

    public static string[] blockTypes = new string[7] { "empty", "grass", "rock", "dirt", "sand", "snow", "water"};

    public ChunkObject(Vector3 position, GameObject chunkPrefab)
    {
        this.position = position;
        // create chunk game object
        chunkGameObj = MonoBehaviour.Instantiate(chunkPrefab, position, Quaternion.identity);
    }

    public void CreateChunkObject(bool withInit = true)
    {
        // generate chunk's geometry
        mesh = new Mesh();
        MeshFilter mf = chunkGameObj.GetComponent<MeshFilter>();
        MeshCollider mc = chunkGameObj.GetComponent<MeshCollider>();

        if (withInit) InitBlocksHeightMapBased(); 
        FillChunkWithBlocks();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }

    private void FillChunkWithBlocks()
    {
        // create child blocks of this chunk
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
                for (int z = 0; z < chunkSize; z++) 
                { 
                    if (chunkGrid[x, y, z] != 0)
                    {
                        BlockGeometry block = new BlockGeometry(this, new Vector3(x, y, z), blockTypes[chunkGrid[x, y, z]]);
                        block.CreateFilteredBlockMesh();
                    }
                }
    }

    private void InitBlocksHeightMapBased()
    {
        int hmax = -1;
        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                int height = ChunkedWorldManager.heightMap[x + (int)position.x, z + (int)position.z];
                if (hmax < height) hmax = height;
                if (position.y <= height)
                {
                    int ymax;
                    if (position.y + chunkSize <= height)
                    {
                        ymax = chunkSize;
                    }
                    else
                    {
                        ymax = height % chunkSize;
                    }
                    for (int y = 0; y < ymax; y++)
                    {
                        if (position.y + y < 10) chunkGrid[x, y, z] = 6;
                        else if (position.y + y > 20) chunkGrid[x, y, z] = 5;
                        else chunkGrid[x, y, z] = 1;
                    }
                }
            }
        Debug.Log(hmax);
    }

    public bool IsBlockAt(Vector3 position)
    {
        int x = (int) position.x;
        int y = (int) position.y;
        int z = (int) position.z;

        // check whether there exists a block at the position
        if (x < 0 || x >= chunkSize ||
            y < 0 || y >= chunkSize ||
            z < 0 || z >= chunkSize)
        {
            return false;
        }
        return (chunkGrid[x, y, z] != 0);
    }

    public void DestroyBlock(Vector3 position)
    {
        Debug.Log("destroying object " + position);
        if (chunkGrid[(int)position.x, (int)position.y, (int)position.z] == 0) Debug.Log("already not there");
        chunkGrid[(int)position.x, (int)position.y, (int)position.z] = 0;
        ReCreateChunkObject();
    }

    public void AddBlock(Vector3 position, int blockType)
    {
        chunkGrid[(int)position.x, (int)position.y, (int)position.z] = blockType;
        ReCreateChunkObject();
    }

    private void ReCreateChunkObject()
    {
        vertices.Clear();
        triangles.Clear();
        UVs.Clear();
        CreateChunkObject(false);
    }

    public int GetBlockType(Vector3 position)
    {
        return chunkGrid[(int)position.x, (int)position.y, (int)position.z];
    }

}
