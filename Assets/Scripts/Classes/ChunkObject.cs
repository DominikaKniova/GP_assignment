﻿using System.Collections;
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

    public ChunkObject(Vector3 position, GameObject chunkPrefab)
    {
        this.position = position;
        // create chunk game object
        chunkGameObj = MonoBehaviour.Instantiate(chunkPrefab, position, Quaternion.identity);
    }

    public void CreateChunkObject()
    {
        // generate chunk's geometry
        mesh = new Mesh();
        MeshFilter mf = chunkGameObj.GetComponent<MeshFilter>();
        MeshCollider mc = chunkGameObj.GetComponent<MeshCollider>();

        InitBlocksHeightMapBased();
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
                    if (chunkGrid[x, y, z] == 1)
                    {
                        BlockGeometry block = new BlockGeometry(this, new Vector3(x, y, z));
                        block.CreateFilteredBlockMesh();
                    }
                }
    }

    private void InitBlocks()
    {
        // randomly initialize blocks in chunk
        for (int y = 0; y < chunkSize; y++)
            for (int x = 0; x < chunkSize; x++)
                for (int z = 0; z < chunkSize; z++)
                { 
                    chunkGrid[x, y, z] = 1;
                }
    }

    private void InitBlocksHeightMapBased()
    {
        for (int x = 0; x < chunkSize; x++)
            for (int z = 0; z < chunkSize; z++)
            {
                int height = ChunkedWorldManager.heightMap[x + (int)position.x, z + (int)position.z];
                if (position.y < height)
                {
                    for (int y = 0; y < Mathf.Min(chunkSize, height); y++)
                    {
                        chunkGrid[x, y, z] = 1;
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
