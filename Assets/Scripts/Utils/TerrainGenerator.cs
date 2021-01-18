using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator
{
    // parameters to make the terrain more interesting
    private static int octavesCount = 3;
    private static float[] frequencies = {2, 5, 7};
    private static float[] elevations = {1, 0.5f, 0.75f};

    /* Generate random terrain height map using Perlin Noise */
    public static int[,] GenerateHeightMap(int width, int depth, int heightVar)
    {
        int[,] heightMap = new int[depth, width];
        float rnd = Random.Range(0.0f, 5.0f);

        for (float y = 0; y < depth; y++)
            for (float x = 0; x < width; x++)
            {
                float value = 0.0f;
                for (int i = 0; i < octavesCount; i++)
                    value += elevations[i] * Mathf.PerlinNoise((frequencies[i] * y + rnd) / depth, (frequencies[i] * x + rnd) / width + rnd);

                heightMap[(int)y, (int)x] = Mathf.RoundToInt(Mathf.Pow(value, 2.8f) * heightVar);
            }

        return heightMap;
    } 
}
