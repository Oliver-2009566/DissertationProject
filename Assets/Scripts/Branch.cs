using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Generates the lines which make up the branches of the L-System plant
public class Branch : MonoBehaviour
{
    // Function which draws a line from a start point to an end point.
    public void DrawBranch(Vector3 start, Vector3 end)
    {
        // Gets the line renderer component
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        // Creates a small array for the points
        var points = new Vector3[2];
        points[0] = start;
        points[1] = end;
        lineRenderer.SetPositions(points);
    }
}
