using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Class containing precomputed data for building/destroying blocks (cube mesh objects) */
public static class BlockData
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

	public static string[] faceOrder = new string[] { "front", "top", "right", "left", "back", "bottom" };

	/* Array of vertices for each cube face */
	public static Dictionary<string, List<Vector3>> vertices = new Dictionary<string, List<Vector3>>
	{
		{"front", new List<Vector3> { cubeVertices[0], cubeVertices[1], cubeVertices[2], cubeVertices[3] }},
		{"top", new List<Vector3> { cubeVertices[2], cubeVertices[5], cubeVertices[4], cubeVertices[3] }},
		{"right", new List<Vector3> { cubeVertices[1], cubeVertices[6], cubeVertices[5], cubeVertices[2] }},
		{"left", new List<Vector3> { cubeVertices[7], cubeVertices[0], cubeVertices[3], cubeVertices[4] }},
		{"back", new List<Vector3> { cubeVertices[6], cubeVertices[7], cubeVertices[4], cubeVertices[5] }},
		{"bottom", new List<Vector3> { cubeVertices[1], cubeVertices[0], cubeVertices[7], cubeVertices[6] }},
	};

	/* Indices of block type materials in texture atlas */
	public static Dictionary<string, Vector2> atlasCoords = new Dictionary<string, Vector2>
	{
		{"grass", new Vector2 (0, 1)},
		{"rock", new Vector2 (1, 1)},
		{"dirt", new Vector2 (0, 0)},
		{"sand", new Vector2 (1, 0)},
		{"snow", new Vector2 (2, 1)},
		{"water", new Vector2 (2, 0)},
	};

	public static float atlasOffU = 1.0f / 3.0f;
	public static float atlasOffV = 1.0f / 2.0f;

	/* Texture UV coordinates precomputed for every block type */
	public static Dictionary<string, List<Vector2>> atlasUVs = new Dictionary<string, List<Vector2>>
	{
		{"grass", new List<Vector2>
			{
			new Vector2 (atlasCoords["grass"].x * atlasOffU, atlasCoords["grass"].y * atlasOffV),
			new Vector2 ((atlasCoords["grass"].x + 1) * atlasOffU, atlasCoords["grass"].y * atlasOffV),
			new Vector2 ((atlasCoords["grass"].x + 1) * atlasOffU, (atlasCoords["grass"].y + 1) * atlasOffV),
			new Vector2 (atlasCoords["grass"].x * atlasOffU, (atlasCoords["grass"].y + 1) * atlasOffV),
			}
		},

		{"rock", new List<Vector2>
			{
			new Vector2 (atlasCoords["rock"].x * atlasOffU, atlasCoords["rock"].y * atlasOffV),
			new Vector2 ((atlasCoords["rock"].x + 1) * atlasOffU, atlasCoords["rock"].y * atlasOffV),
			new Vector2 ((atlasCoords["rock"].x + 1) * atlasOffU, (atlasCoords["rock"].y + 1) * atlasOffV),
			new Vector2 (atlasCoords["rock"].x * atlasOffU, (atlasCoords["rock"].y + 1) * atlasOffV),
			}
		},

		{"dirt", new List<Vector2>
			{
			new Vector2 (atlasCoords["dirt"].x * atlasOffU, atlasCoords["dirt"].y * atlasOffV),
			new Vector2 ((atlasCoords["dirt"].x + 1) * atlasOffU, atlasCoords["dirt"].y * atlasOffV),
			new Vector2 ((atlasCoords["dirt"].x + 1) * atlasOffU, (atlasCoords["dirt"].y + 1) * atlasOffV),
			new Vector2 (atlasCoords["dirt"].x * atlasOffU, (atlasCoords["dirt"].y + 1) * atlasOffV),
			}
		},

		{"sand", new List<Vector2>
			{
			new Vector2 (atlasCoords["sand"].x * atlasOffU, atlasCoords["sand"].y * atlasOffV),
			new Vector2 ((atlasCoords["sand"].x + 1) * atlasOffU, atlasCoords["sand"].y * atlasOffV),
			new Vector2 ((atlasCoords["sand"].x + 1) * atlasOffU, (atlasCoords["sand"].y + 1) * atlasOffV),
			new Vector2 (atlasCoords["sand"].x * atlasOffU, (atlasCoords["sand"].y + 1) * atlasOffV),
			}
		},

		{"water", new List<Vector2>
			{
			new Vector2 (atlasCoords["water"].x * atlasOffU, atlasCoords["water"].y * atlasOffV),
			new Vector2 ((atlasCoords["water"].x + 1) * atlasOffU, atlasCoords["water"].y * atlasOffV),
			new Vector2 ((atlasCoords["water"].x + 1) * atlasOffU, (atlasCoords["water"].y + 1) * atlasOffV),
			new Vector2 (atlasCoords["water"].x * atlasOffU, (atlasCoords["water"].y + 1) * atlasOffV),
			}
		},

		{"snow", new List<Vector2>
			{
			new Vector2 (atlasCoords["snow"].x * atlasOffU, atlasCoords["snow"].y * atlasOffV),
			new Vector2 ((atlasCoords["snow"].x + 1) * atlasOffU, atlasCoords["snow"].y * atlasOffV),
			new Vector2 ((atlasCoords["snow"].x + 1) * atlasOffU, (atlasCoords["snow"].y + 1) * atlasOffV),
			new Vector2 (atlasCoords["snow"].x * atlasOffU, (atlasCoords["snow"].y + 1) * atlasOffV),
			}
		},
	};

	public static string[] numType2string = new string[7] { "empty", "grass", "rock", "dirt", "sand", "snow", "water" };

	/* Each block type has different destroy time */
	public static Dictionary<string, float> destroyTimes = new Dictionary<string, float>
	{
		{"grass", 1},
		{"rock", 4},
		{"dirt", 3},
		{"sand", 2},
		{"snow", 4},
		{"water", 2},
	};
		
	public enum Type
	{
		EMPTY,
		GRASS,
		ROCK,
		DIRT,
		SAND,
		SNOW,
		WATER
	}
}
