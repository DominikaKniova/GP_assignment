using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGeometry
{
    public ChunkObject parent;
    public Vector3 position;
    public byte blockType;
    public BlockGeometry(ChunkObject parent, Vector3 position, byte blockType)
    {
        this.parent = parent;
        this.position = position;
        this.blockType = blockType;
    }
    private void BlockSide(string side)
    {
        // add geometry of this block's face to parent's geometry
        // triangle vertex indices
        int vSize = parent.vertices.Count;
        parent.triangles.AddRange(new List<int> { vSize, vSize + 2, vSize + 1, vSize, vSize + 3, vSize + 2 });

        // face vertices
        for (int i = 0; i < 4; i++)
        {
            parent.vertices.Add(BlockData.vertices[side][i] + position);
        }

        // texture coords based on blockType
        parent.UVs.AddRange(BlockData.atlasUVs[BlockData.intType2string[blockType]]);
    }
    public void CreateBlockMesh()
    {
        // add all faces of the block
        foreach (string face in BlockData.faceOrder)
        {
            BlockSide(face);
        }
    }

    public void CreateFilteredBlockMesh()
    {
        // add only those faces of block that are not adjacent to other block faces
        if (!parent.IsBlockAt(position + Vector3.forward)) BlockSide("back");
        if (!parent.IsBlockAt(position - Vector3.forward)) BlockSide("front");

        if (!parent.IsBlockAt(position + Vector3.right)) BlockSide("right");
        if (!parent.IsBlockAt(position - Vector3.right)) BlockSide("left");

        if (!parent.IsBlockAt(position + Vector3.up)) BlockSide("top");
        if (!parent.IsBlockAt(position - Vector3.up)) BlockSide("bottom");
    }
}
