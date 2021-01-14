using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockObject
{
    public ChunkObject parent;
    public Vector3 position;
    public BlockObject(ChunkObject parent, Vector3 position)
    {
        this.parent = parent;
        this.position = position;
    }
    private void BlockSide(string side)
    {
        int vSize = parent.vertices.Count;
        parent.triangles.AddRange(new List<int> { vSize, vSize + 2, vSize + 1, vSize, vSize + 3, vSize + 2 });
        for (int i = 0; i < 4; i++)
        {
            parent.vertices.Add(Meshes.Cube.vertices[side][i] + position);
        }
        parent.UVs.AddRange(Meshes.Cube.UVs);
    }
    public void CreateBlockMesh()
    {
        foreach (string face in Meshes.Cube.faceOrder)
        {
            BlockSide(face);
        }
    }

    public void CreateFilteredBlockMesh()
    {
        if (parent.NotBlockAt(position + Vector3.forward)) BlockSide("back");
        if (parent.NotBlockAt(position - Vector3.forward)) BlockSide("front");

        if (parent.NotBlockAt(position + Vector3.right)) BlockSide("right");
        if (parent.NotBlockAt(position - Vector3.right)) BlockSide("left");

        if (parent.NotBlockAt(position + Vector3.up)) BlockSide("top");
        if (parent.NotBlockAt(position - Vector3.up)) BlockSide("bottom");
    }
}
