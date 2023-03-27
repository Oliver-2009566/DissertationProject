using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    // The mesh is the terrain we will generate
    Mesh mesh;

    // Meshes are made of triangles, the vertices are the corners of the triangles. The colors are what colour a vertex should be rendered as
    Vector3[] vertices;
    int[] triangles;
    Color[] colors;

    // The size of the mesh, this can be confiqured to give bigger and smaller meshes
    public int xSize = 20;
    public int zSize = 20;

    // The colour gradient I am using to colour the terrain
    public Gradient gradient;

    // Values used to normalise the heights of the vertexes in order to find whats the lowest and highest, so we can colour the vertexes accordingly
    float minTerrainHeight;
    float maxTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

    }

    private void Update()
    {
        // Called every frame for testing reasons, allows for tweaking values in the editor and seeing the changes dynamically
        CreateShape();
        UpdateMesh();
    }


    void CreateShape ()
    {
        // Creates a vector3 array called vertices with a size fitting the grid
        // The amount of vertices we need for a size in the mesh is the size of the grid, plus 1 vertex
        // Total vertexes is that multiplied by the size of the other side plus 1
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        // Loops across the z-axis of the grid
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            // Loops along the x-axis of the grid
            for (int x = 0; x <= xSize; x++)
            {
                // Creates each vertex according to the x/z values in the loops, iterating over the whole grid
                // The y values are given based on a perlin noise map, imported from the math function
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 3f;
                vertices[i] = new Vector3(x, y, z);

                // Sets the max and min heights of the y values if they exceed or go below the max and min respectfully
                if (y > maxTerrainHeight)
                    maxTerrainHeight = y;
                if (y < minTerrainHeight)
                    minTerrainHeight = y; 

                i++;
            }   
        }

        // Creates an int array for points which represent triangles
        // Triangles are three integer values representing the index of the vertexes of the triangle
        // Triangles are represented as vertices which go in a clockwise direction
        // Triangles which go anticlockwise aren't rendered, as such the underside of each triangle is transparent
        triangles = new int[xSize * zSize * 6];

        // These represent the current vertex and triangle we are creating
        int vert = 0;
        int tris = 0;

        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                // This creates a quad (2 triangles) 
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2; 

                vert++;
                tris += 6;
            }
            vert++;
        }

        // This creates an array of colours the size of how many vertices there are
        colors = new Color[vertices.Length];
        
        // Iterates through every single vertex in the mesh
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                // Calculates the height of the y value of the vertex in comparrison to the max and min heights
                // This is used to evaluate what colour in the gradient the vertex should be
                float height = Mathf.InverseLerp(minTerrainHeight, maxTerrainHeight, vertices[i].y);
                colors[i] = gradient.Evaluate(height);
                i++;
            }   
        }

    }

    // This refreshes the mesh with the current vertices, triangles and colours
    // Used to constantly update the mesh when tinkering with the editor variables in order to see live changes
    void UpdateMesh() 
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.colors = colors;

        mesh.RecalculateNormals();
    }

}
