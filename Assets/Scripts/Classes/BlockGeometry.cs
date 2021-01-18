using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGeometry
{
    private ChunkObject parent;
    private Vector3 position;
    private byte blockType;
    public BlockGeometry(ChunkObject parent, Vector3 position, byte blockType)
    {
        this.parent = parent;
        this.position = position;
        this.blockType = blockType;
    }

    /* Add geometry of one face of a block to parent's (chunk's) geometry buffers */
    private void BlockSide(string side)
    {
        // triangle vertices indices
        int buffSize = parent.vertices.Count;
        parent.triangles.AddRange(new List<int> { buffSize, buffSize + 2, buffSize + 1, buffSize, buffSize + 3, buffSize + 2 });

        // face vertices
        for (int i = 0; i < 4; i++)
        {
            parent.vertices.Add(BlockData.vertices[side][i] + position);
        }

        // texture coords based on blockType
        parent.UVs.AddRange(BlockData.atlasUVs[BlockData.numType2string[blockType]]);
    }

    /* Add all faces of a block to chunk's geometry */
    public void CreateBlockMesh()
    {
        // add all faces of the block
        foreach (string face in BlockData.faceOrder)
        {
            BlockSide(face);
        }
    }

    /* Add only those faces of a block to chunk's geometry that are visible */
    public void CreateFilteredBlockMesh()
    {
        // add only those that are not adjacent to other block faces
        if (!parent.IsBlockAt(position + Vector3.forward)) BlockSide("back");
        if (!parent.IsBlockAt(position - Vector3.forward)) BlockSide("front");

        if (!parent.IsBlockAt(position + Vector3.right)) BlockSide("right");
        if (!parent.IsBlockAt(position - Vector3.right)) BlockSide("left");

        if (!parent.IsBlockAt(position + Vector3.up)) BlockSide("top");
        if (!parent.IsBlockAt(position - Vector3.up)) BlockSide("bottom");
    }
}
