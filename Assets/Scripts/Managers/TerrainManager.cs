using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainManager
{
    public static int waterLevel = 4;
    public static int greeneryLevel = 20;

    private static int octavesCount = 3;
    private static float[] frequencies = {2, 5, 7};
    private static float[] elevations = {1, 0.5f, 0.75f};

    public static int[,] GenerateHeightMap(float width, float depth, float maxHeight)
    {
        int[,] heightMap = new int[(int)depth, (int)width];
        float rnd = Random.Range(0.0f, 3.5f);

        for (float y = 0; y < depth; y++)
        {
            for (float x = 0; x < width; x++)
            {
                float value = 0.0f;
                
                for (int i = 0; i < octavesCount; i++)
                {
                    value += elevations[i] * Mathf.PerlinNoise((frequencies[i] * y + rnd) / depth, (frequencies[i] * x + rnd) / width + rnd);
                }
                heightMap[(int)y, (int)x] = Mathf.RoundToInt(Mathf.Pow(value, 2.8f) * maxHeight);
            }
        }
        return heightMap;
    } 
}
