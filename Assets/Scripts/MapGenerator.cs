using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public enum DrawMode{NoiseMap, ColorMap, Mesh, FalloffMap};
    public DrawMode drawMode;

    public int mapSize;
    public float noiseScale;
    
    public int octaves;
    [Range(0, 1)]
    public float persistence;
    public float lacunarity;

    public int seed;
    public Vector2 offset;

    public bool useFalloff;

    public float meshHeightMultiplier;
    public AnimationCurve meshHeightCurve;

    public bool autoUpdate;

    public TerrainType[] regions;

    float[,] falloffMap;
    public GameObject plantPrefab;

    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
        GenerateNewMap();
    }

    public void GenerateNewMap()
    {
        lacunarity = Random.Range(1.9f, 2.1f);
        meshHeightMultiplier = Random.Range(10f, 30f);
        seed = Random.Range(0, 1000000000);
        GenerateMap();
    }
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, octaves, persistence, lacunarity, offset);

        for (int i = 0; i < 20; i++)
        {
            int xPos = Random.Range(0, noiseMap.GetLength(0));
            int yPos = Random.Range(0, noiseMap.GetLength(1));
            Debug.Log(xPos);
            Debug.Log(yPos);
            Vector3 plantPosition = new Vector3(xPos - 125, meshHeightCurve.Evaluate(noiseMap[xPos, yPos]) * meshHeightMultiplier, yPos - 125);

            GameObject plant = Instantiate(plantPrefab);
            plant.transform.localPosition = plantPosition;
        }

        Color[] colorMap = new Color[mapSize * mapSize];
        for(int y = 0; y < mapSize; y++)
        {
            for(int x = 0; x < mapSize; x++)
            {
                if (useFalloff)
                {
                    noiseMap[x, y] = Mathf.Clamp01(noiseMap[x, y] - falloffMap[x,y]);
                }
                float currentHeight = noiseMap[x, y];
                for(int i = 0; i < regions.Length; i++)
                {
                    if(currentHeight <= regions[i].height)
                    {
                        colorMap[y * mapSize + x] = regions[i].color;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay> ();
        if(drawMode == DrawMode.NoiseMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));  
        }
        else if(drawMode == DrawMode.ColorMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));  
        }
        else if (drawMode == DrawMode.Mesh)
        {
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));
        }
        else if (drawMode == DrawMode.FalloffMap)
        {
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FalloffGenerator.GenerateFalloffMap(mapSize)));
        }
    }

    void OnValidate()
    {
        if (mapSize < 1)
        {
            mapSize = 1;
        }
        if (lacunarity < 1)
        {
            lacunarity = 1;
        }
        if (octaves < 0)
        {
            octaves = 0;
        }
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
    }
}

[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}