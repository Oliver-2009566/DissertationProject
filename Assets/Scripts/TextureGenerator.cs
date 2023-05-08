using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This class creates both the noise map and colour map textures
// With help from: https://youtu.be/RDQK1_SWFuc
public static class TextureGenerator
{
    // Creates a 2d texture using a provided colour map and the size of the mesh
    public static Texture2D TextureFromColorMap(Color[] colorMap, int width, int height)
    {
        Texture2D texture = new Texture2D(width, height);
        texture.filterMode = FilterMode.Point;
        texture.wrapMode = TextureWrapMode.Clamp;
        texture.SetPixels(colorMap);
        texture.Apply();
        return texture;
    }

    // Creates a perlin noise map using only the noise map values
    public static Texture2D TextureFromHeightMap(float[,] heightMap)
    {
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);

        Color[] colorMap = new Color[width * height];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, heightMap[x,y]);
            }
        }
        return TextureFromColorMap(colorMap, width, height);
    }
}
