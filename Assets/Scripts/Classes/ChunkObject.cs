using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObject : MonoBehaviour
{
    private Mesh mesh;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> UVs = new List<Vector2>();

    public const int chunkSize = 10;
    public int[,,] chunkGrid = new int[chunkSize, chunkSize, chunkSize];
    private void Start()
    {
        CreateChunkObject();    
    }

    public void CreateChunkObject()
    {
        // generate chunk's geometry
        mesh = new Mesh();
        MeshFilter mf = GetComponent<MeshFilter>();
        MeshCollider mc = GetComponent<MeshCollider>();

        FillWithBlocks();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }

    private void FillWithBlocks()
    {
        // randomly initialize blocks in chunk
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (Random.Range(0, 2) == 1)
                    {
                        chunkGrid[x, y, z] = 1;
                    }
                }
            }
        }

        // create child blocks of this chunk
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    if (chunkGrid[x, y, z] == 1)
                    {
                        BlockGeometry block = new BlockGeometry(this, new Vector3(x, y, z));
                        block.CreateBlockMesh();
                    }
                }
            }
        }
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
}
