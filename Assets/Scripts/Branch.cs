using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public void DrawBranch(Vector3 start, Vector3 end)
    {
        LineRenderer lineRenderer = GetComponent<LineRenderer>();
        var points = new Vector3[2];
        points[0] = start;
        points[1] = end;
        lineRenderer.SetPositions(points);
    }
}
