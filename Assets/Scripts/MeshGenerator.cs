using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Generates the mesh that makes up the terrain of the island
// With help from: https://youtu.be/4RpVBYW1r5M
public static class MeshGenerator
{   
    // Function that generates the mesh using a noise map, the height multiplier and the height curve
    public static MeshData GenerateTerrainMesh(float[,] heightMap, float heightMultiplier, AnimationCurve heightCurve)
    {
        // Width is the x-axis size of the mesh
        // Height is the z-axis size
        // topLeftX and topLeftZ are the co-ords for the top left vertex
        int width = heightMap.GetLength(0);
        int height = heightMap.GetLength(1);
        float topLeftX = (width - 1) / -2f;
        float topLeftZ = (height - 1) / 2f;

        MeshData meshData = new MeshData(width, height);
        int vertexIndex = 0;

        // Repeats through every vertex in the mesh
        // Sets the position of the vertex to where it should be in the grid
        // As well as sets its y-axis to a value calculated using the noise map, the height multiplier and the height curve
        for (int y = 0; y < height; y++)
        {
            for  (int x = 0; x < width; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[x,y]) * heightMultiplier, topLeftZ - y);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)width, y / (float)height);

                // Creates the triangles which make up the grid
                // Triangles are made from vertices in a clockwise orientation
                // Unity only renders triangles whose vertices are in a clockwise orientation, those with anti-clockwise vertices aren't rendered
                if (x < width - 1 && y < height - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + width + 1, vertexIndex + width);
                    meshData.AddTriangle(vertexIndex + width + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }
        return meshData;
    }
}

// The MeshData class is a custom class housing all the information needed for the mesh
// This includes vertices and their locations
// The information which defines the triangles in the mesh
// The UVs which make up the colours of the mesh
public class MeshData
{
    public Vector3[] vertices;
    public int[] triangles;
    public Vector2[] uvs;

    int triangleIndex;

    public MeshData(int meshWidth, int meshHeight)
    {
        vertices = new Vector3[meshWidth * meshHeight];
        uvs = new Vector2[meshWidth * meshHeight];
        triangles = new int[(meshHeight - 1) * (meshHeight - 1) * 6];
    }

    public void AddTriangle(int a, int b, int c)
    {
        triangles[triangleIndex] = a;
        triangles[triangleIndex + 1] = b;
        triangles[triangleIndex + 2] = c;
        triangleIndex += 3;
    }

    public Mesh CreateMesh()
    {
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        return mesh;
    }
}