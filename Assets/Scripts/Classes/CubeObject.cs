using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeObject : MonoBehaviour
{

    private Mesh mesh;

    private List<Vector3> vertices = new List<Vector3>();
    private List<int> triangles = new List<int>();
    private List<Vector2> UVs = new List<Vector2>();
    
    void Start()
    {
        CreateCubeObject();
    }

    private void CreateCubeMesh()
    {
        mesh = new Mesh();

        CubeSide("front");
        CubeSide("top");
        CubeSide("right");
        CubeSide("left");
        CubeSide("back");
        CubeSide("bottom");
    }

    private void CubeSide(string side)
    {
        int vSize = vertices.Count;
        triangles.AddRange(new List<int> { vSize, vSize + 2, vSize + 1, vSize, vSize + 3, vSize + 2 });
        vertices.AddRange(Meshes.Cube.vertices[side]);
        UVs.AddRange(Meshes.Cube.UVs);
    }

    public void CreateCubeObject()
    {
        MeshFilter mf = GetComponent<MeshFilter>();

        CreateCubeMesh();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = UVs.ToArray();

        mesh.RecalculateBounds();
        mesh.RecalculateNormals();

        mf.mesh = mesh;
    }
}
