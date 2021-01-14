using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Meshes
{
	public static class Cube
    {
		private static List<Vector3> cubeVertices = new List<Vector3>
		{
			new Vector3 (0, 0, 0),
			new Vector3 (1, 0, 0),
			new Vector3 (1, 1, 0),
			new Vector3 (0, 1, 0),
			new Vector3 (0, 1, 1),
			new Vector3 (1, 1, 1),
			new Vector3 (1, 0, 1),
			new Vector3 (0, 0, 1),
		};

		//public static int[] triangles = 
		//{
		//	0, 2, 1, //face front
		//	0, 3, 2,
		//	2, 3, 4, //face top
		//	2, 4, 5,
		//	1, 2, 5, //face right
		//	1, 5, 6,
		//	0, 7, 4, //face left
		//	0, 4, 3,
		//	5, 4, 7, //face back
		//	5, 7, 6,
		//	0, 6, 7, //face bottom
		//	0, 1, 6
		//};

		public static Dictionary<string, List<Vector3>> vertices = new Dictionary<string, List<Vector3>>
		{
			{"front", new List<Vector3> { cubeVertices[0], cubeVertices[1], cubeVertices[2], cubeVertices[3] }},
			{"top", new List<Vector3> { cubeVertices[2], cubeVertices[5], cubeVertices[4], cubeVertices[3] }},
			{"right", new List<Vector3> { cubeVertices[1], cubeVertices[6], cubeVertices[5], cubeVertices[2] }},
			{"left", new List<Vector3> { cubeVertices[7], cubeVertices[0], cubeVertices[3], cubeVertices[4] }},
			{"back", new List<Vector3> { cubeVertices[6], cubeVertices[7], cubeVertices[4], cubeVertices[5] }},
			{"bottom", new List<Vector3> { cubeVertices[1], cubeVertices[0], cubeVertices[7], cubeVertices[6] }},
		};

		public static Dictionary<string, List<int>> triangles = new Dictionary<string, List<int>>
		{
			{"front", new List<int> { 0, 2, 1, 0, 3, 2 }},
			{"top", new List<int> { 2, 3, 4, 2, 4, 5 }},
			{"right", new List<int> { 1, 2, 5, 1, 5, 6 }},
			{"left", new List<int> { 0, 7, 4, 0, 4, 3 }},
			{"back", new List<int> { 5, 4, 7, 5, 7, 6 }},
			{"bottom", new List<int> { 0, 6, 7, 0, 1, 6 }},
		};

        public static List<Vector2> UVs = new List<Vector2>
        {
            new Vector2 (0, 0),
            new Vector2 (1, 0),
            new Vector2 (1, 1),
            new Vector2 (0, 1),
        };
	}
}
