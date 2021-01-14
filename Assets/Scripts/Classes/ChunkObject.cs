using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObject : MonoBehaviour
{
    private Mesh mesh;

    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> UVs = new List<Vector2>();

    public const int chunkSize = 8;
    public int[,,] chunkGrid = new int[chunkSize, chunkSize, chunkSize];
    private void Start()
    {
        CreateChunkObject();    
    }

    public void CreateChunkObject()
    {
        mesh = new Mesh();
        MeshFilter mf = GetComponent<MeshFilter>();

        FillWithBlocks();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
    }

    private void FillWithBlocks()
    {
        // create child blocks of this chunk
        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    chunkGrid[x, y, z] = 1;
                }
            }
        }

        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    BlockObject block = new BlockObject(this, new Vector3(x, y, z));
                    block.CreateFilteredBlockMesh();
                }
            }
        }
    }

    public bool NotBlockAt(Vector3 position)
    {
        int x = (int) position.x;
        int y = (int) position.y;
        int z = (int) position.z;

        if (x < 0 || x >= chunkSize ||
            y < 0 || y >= chunkSize ||
            z < 0 || z >= chunkSize)
        {
            return true;
        }
        return (chunkGrid[x, y, z] == 0);
    }
}
