using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkObject
{
    private Mesh mesh;
    private GameObject chunkGameObj;

    private const int chunkSize = WorldManager.chunkSize;
    public Vector3Int position;

    // geometry buffers
    public List<Vector3> vertices = new List<Vector3>();
    public List<int> triangles = new List<int>();
    public List<Vector2> UVs = new List<Vector2>();

    // 3D grid storing type of each block in this chunk
    public byte[,,] chunkGrid = new byte[chunkSize, chunkSize, chunkSize];

    public ChunkObject(Vector3Int _position, GameObject chunkPrefab)
    {
        position = _position;
        // create chunk game object
        chunkGameObj = MonoBehaviour.Instantiate(chunkPrefab, _position, Quaternion.identity);
    }

    /* Create chunk's geometry with child blocks */
    public void CreateChunkObject(bool withInit = true)
    {
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

        // initialize game object's components
        mf.mesh = mesh;
        mc.sharedMesh = mesh;
    }

    /* Based on randomly generated terraing height map initialize 3D chunkGrid with information about block types */
    private void InitBlocksHeightMapBased()
    {
        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                int height = WorldManager.heightMap[x + position.x, z + position.z];
                if (height == 0)
                {
                    // height cannot be 0
                    WorldManager.heightMap[x + position.x, z + position.z]++;
                    height++;
                }
                if (position.y <= height)
                {
                    height = position.y + chunkSize <= height ? chunkSize : height % chunkSize;
                    for (int y = 0; y < height; y++)
                        chunkGrid[x, y, z] = GenerateChunkType(position.y + y);
                }
            }
    }

    /* Based on block's height decide its type */
    private byte GenerateChunkType(int h)
    {
        // make transitions between water and sand straight
        if (h < 6) return (byte)BlockData.Type.WATER;
        if (h >= 6 && h < 9) return (byte)BlockData.Type.SAND;

        // add noise to height to make transitions less obvious
        h += Random.Range(0, 5);
        if (h >= 9 && h < 14) return (byte)BlockData.Type.GRASS;
        if (h >= 14 && h < 20) return (byte)BlockData.Type.DIRT;
        if (h >= 20 && h < 28) return (byte)BlockData.Type.ROCK;
        return (byte)BlockData.Type.SNOW;
    }

    /* Fill chunk's geometry buffers with blocks */
    private void FillChunkWithBlocks()
    {
        // create child blocks of this chunk
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
                for (int z = 0; z < chunkSize; z++) 
                { 
                    if (chunkGrid[x, y, z] != 0)   // 0 means empty block
                    {
                        BlockGeometry block = new BlockGeometry(this, new Vector3Int(x, y, z), chunkGrid[x, y, z]);
                        // add only those faces of a block that will not be occluded by other blocks in this chunk
                        block.CreateFilteredBlockMesh();
                    }
                }
    }

    /* Check whether there exists a block at position pos */
    public bool IsBlockAt(Vector3Int pos)
    {
        if (pos.x < 0 || pos.x >= chunkSize ||
            pos.y < 0 || pos.y >= chunkSize ||
            pos.z < 0 || pos.z >= chunkSize)
        {
            return false;
        }
        return (chunkGrid[pos.x, pos.y, pos.z] != 0);
    }

    public void DestroyBlock(Vector3Int pos)
    {
        // set occupied block as empty
        chunkGrid[pos.x, pos.y, pos.z] = 0;

        // redraw whole chunk
        ReCreateChunkObject();
    }

    public void AddBlock(Vector3Int pos, byte blockType, bool recreateChunk = true)
    {
        // set empty block as occupied (with its new type)
        chunkGrid[pos.x, pos.y, pos.z] = blockType;

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

    /* Clear chunk's whole geometry and set all its block as empty */
    public void ClearChunk()
    {
        ClearBuffers();
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
                for (int z = 0; z < chunkSize; z++)
                    chunkGrid[x, y, z] = 0;
    }

    public byte GetBlockType(Vector3Int pos)
    {
        return chunkGrid[pos.x, pos.y, pos.z];
    }

    /* Create chunk's geometry from saved file */
    public void ReCreateChunkFromSave(ref ChunkData chunkData, Vector3S chunkPosition)
    {
        for (int i = 0; i < chunkData.blockPositions.Count; i++)
        {
            chunkGrid[chunkData.blockPositions[i].x, chunkData.blockPositions[i].y, chunkData.blockPositions[i].z] = chunkData.blockTypes[i];
        }
        CreateChunkObject(false);
    }
}
