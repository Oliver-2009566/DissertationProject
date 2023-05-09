using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script generates the entire map, calling all the necessary functions that are needed to generate an island
// With help from: https://www.youtube.com/watch?v=wbpMiKiSKm8&list=PLFt_AvWsXl0eBW2EiBtl_sxmDtSgZBxB3
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

    private float waitTime = 2f;

    // When the game is started, create a falloff map and generate a brand new island
    void Awake()
    {
        falloffMap = FalloffGenerator.GenerateFalloffMap(mapSize);
        GenerateNewMap();
    }
    
    // Randomises some values in order make a brand new island
    public void GenerateNewMap()
    {
        lacunarity = Random.Range(1.9f, 2.1f);
        meshHeightMultiplier = Random.Range(10f, 30f);
        seed = Random.Range(0, 1000000000);
        GenerateMap();
    }
    // Creates a noise map, places L-System plant object around where they should be on the map
    // Subtracts the falloff map if the variable useFalloff is true
    public void GenerateMap()
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, octaves, persistence, lacunarity, offset);

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

        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        foreach(GameObject plant in plants)
        GameObject.Destroy(plant);

        bool planted = false;
        float topLeftX = (noiseMap.GetLength(0) - 1) / -2f;
        float topLeftZ = (noiseMap.GetLength(1) - 1) / 2f;

        for(int i = 0; i < 20; i++)
        {
            while(planted == false)
            {
                int xPos = Random.Range(0, noiseMap.GetLength(0));
                int zPos = Random.Range(0, noiseMap.GetLength(1));
                if (noiseMap[xPos,zPos] > 0.5)
                {
                    Vector3 plantPosition = new Vector3((topLeftX + xPos) *  10, (meshHeightCurve.Evaluate(noiseMap[xPos,zPos]) * meshHeightMultiplier) * 10, (topLeftZ - zPos) * 10);
                    GameObject plant = Instantiate(plantPrefab);
                    plant.transform.localPosition = plantPosition;
                    planted = true;
                }     
            }
            planted = false;
        }

        // This calls one of a variety of different draw functions based on what setting the map generator is set to
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


    IEnumerator GenerateMapSlow()
    {
        lacunarity = Random.Range(1.9f, 2.1f);
        meshHeightMultiplier = Random.Range(10f, 30f);
        seed = Random.Range(0, 1000000000);

        GameObject[] plants = GameObject.FindGameObjectsWithTag("Plant");
        foreach(GameObject plant in plants)
        GameObject.Destroy(plant);

        GameObject water = GameObject.Find("Water");
        GameObject mesh = GameObject.Find("Mesh");

        water.GetComponent<Renderer>().enabled = false;
        mesh.GetComponent<Renderer>().enabled = false;

        MapDisplay display = FindObjectOfType<MapDisplay> ();

        float[,] noiseMap = Noise.GenerateNoiseMap(mapSize, mapSize, seed, noiseScale, octaves, persistence, lacunarity, offset);
        display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));  


        yield return new WaitForSeconds(waitTime);

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

        display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap)); 

        yield return new WaitForSeconds(waitTime); 

        display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize)); 

        yield return new WaitForSeconds(waitTime);
        
        display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, meshHeightMultiplier, meshHeightCurve), TextureGenerator.TextureFromColorMap(colorMap, mapSize, mapSize));
        mesh.GetComponent<Renderer>().enabled = true;

        yield return new WaitForSeconds(waitTime);

        bool planted = false;
        float topLeftX = (noiseMap.GetLength(0) - 1) / -2f;
        float topLeftZ = (noiseMap.GetLength(1) - 1) / 2f;

        for(int i = 0; i < 20; i++)
        {
            while(planted == false)
            {
                int xPos = Random.Range(0, noiseMap.GetLength(0));
                int zPos = Random.Range(0, noiseMap.GetLength(1));
                if (noiseMap[xPos,zPos] > 0.5)
                {
                    Vector3 plantPosition = new Vector3((topLeftX + xPos) *  10, (meshHeightCurve.Evaluate(noiseMap[xPos,zPos]) * meshHeightMultiplier) * 10, (topLeftZ - zPos) * 10);
                    GameObject plant = Instantiate(plantPrefab);
                    plant.transform.localPosition = plantPosition;
                    planted = true;
                }     
            }
            planted = false;
        }


        yield return new WaitForSeconds(waitTime);

        water.GetComponent<Renderer>().enabled = true;

        yield return null;
    }

    // Whenever anything is edited in the inspector, it makes sure these variables are clamped
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

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            StartCoroutine(GenerateMapSlow());
        }
    }

}

// The struct which makes up the different terrain types used by the island
[System.Serializable]
public struct TerrainType
{
    public string name;
    public float height;
    public Color color;
}