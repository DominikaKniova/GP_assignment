using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockGeometry
{
    public ChunkObject parent;
    public Vector3 position;
    public BlockGeometry(ChunkObject parent, Vector3 position)
    {
        this.parent = parent;
        this.position = position;
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
            parent.vertices.Add(Meshes.Cube.vertices[side][i] + position);
        }
        // texture coords
        parent.UVs.AddRange(Meshes.Cube.UVs);
    }
    public void CreateBlockMesh()
    {
        // add all faces of the block
        foreach (string face in Meshes.Cube.faceOrder)
        {
            BlockSide(face);
        }
    }

    public void CreateFilteredBlockMesh()
    {
        // add only faces of blocks that are not adjacent to other block faces
        if (!parent.IsBlockAt(position + Vector3.forward)) BlockSide("back");
        if (!parent.IsBlockAt(position - Vector3.forward)) BlockSide("front");

        if (!parent.IsBlockAt(position + Vector3.right)) BlockSide("right");
        if (!parent.IsBlockAt(position - Vector3.right)) BlockSide("left");

        if (!parent.IsBlockAt(position + Vector3.up)) BlockSide("top");
        if (!parent.IsBlockAt(position - Vector3.up)) BlockSide("bottom");
    }
}
