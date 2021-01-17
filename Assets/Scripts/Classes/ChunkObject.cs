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
    public int[,,] chunkGrid = new int[chunkSize, chunkSize, chunkSize];

    public Vector3Int position;
    private GameObject chunkGameObj;

    public static string[] blockTypes = new string[7] { "empty", "grass", "rock", "dirt", "sand", "snow", "water"};

    public ChunkObject(Vector3 _position, GameObject chunkPrefab)
    {
        position = new Vector3Int((int)_position.x , (int)_position.y, (int)_position.z);
        // create chunk game object
        chunkGameObj = MonoBehaviour.Instantiate(chunkPrefab, _position, Quaternion.identity);
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
        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                int height = ChunkedWorldManager.heightMap[x + position.x, z + position.z];

                if (position.y <= height)
                {
                    height = position.y + chunkSize <= height ? chunkSize : height % chunkSize;
                    for (int y = 0; y < height; y++)
                    {
                        if (position.y + y < 10) chunkGrid[x, y, z] = 6;
                        else if (position.y + y > 20) chunkGrid[x, y, z] = 5;
                        else chunkGrid[x, y, z] = 1;
                    }
                }
            }
    }

    public bool IsBlockAt(Vector3 pos)
    {
        int x = (int)pos.x;
        int y = (int)pos.y;
        int z = (int)pos.z;

        // check whether there exists a block at the position
        if (x < 0 || x >= chunkSize ||
            y < 0 || y >= chunkSize ||
            z < 0 || z >= chunkSize)
        {
            return false;
        }
        return (chunkGrid[x, y, z] != 0);
    }

    public void DestroyBlock(Vector3Int pos)
    {
        // set occupied block as empty
        chunkGrid[pos.x, pos.y, pos.z] = 0;

        // update height map
        if ( position.y + pos.y + 1 == ChunkedWorldManager.heightMap[position.x + pos.x, position.z + pos.z])
        {
            for (int y = pos.y - 1; y >= 0; y--)
            {
                if (chunkGrid[pos.x, pos.y, pos.z] != 0)
                {
                    ChunkedWorldManager.heightMap[position.x + pos.x, position.z + pos.z] = position.y + y;
                    break;
                }
            }
        }

        // redraw whole chunk
        ReCreateChunkObject();
    }

    public void AddBlock(Vector3Int pos, int blockType, bool recreateChunk = true)
    {
        // set empty block as occupied (with its type)
        chunkGrid[pos.x, pos.y, pos.z] = blockType;

        // update height map
        if (position.y + pos.y + 1 > ChunkedWorldManager.heightMap[position.x + pos.x, position.z + pos.z])
        {
            ChunkedWorldManager.heightMap[position.x + pos.x, position.z + pos.z] = position.y + pos.y + 1;
        }

        // redraw whole chunk
        if (recreateChunk) ReCreateChunkObject();
    }

    private void ClearBuffers()
    {
        vertices.Clear();
        triangles.Clear();
        UVs.Clear();
    }
    private void ReCreateChunkObject()
    {
        ClearBuffers();
        CreateChunkObject(false);
    }

    public void ClearChunk()
    {
        ClearBuffers();
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
                for (int z = 0; z < chunkSize; z++)
                {
                    chunkGrid[x, y, z] = 0;
                }
    }

    public int GetBlockType(Vector3 pos)
    {
        return chunkGrid[(int)pos.x, (int)pos.y, (int)pos.z];
    }

    public void ReCreateChunkFromSave(ref ChunkData chunkData)
    {
        for (int i = 0; i < chunkData.blockPositions.Count; i++)
        {
            chunkGrid[chunkData.blockPositions[i].x, chunkData.blockPositions[i].y, chunkData.blockPositions[i].z] = chunkData.blockTypes[i];
        }
        CreateChunkObject(false);
    }
}
