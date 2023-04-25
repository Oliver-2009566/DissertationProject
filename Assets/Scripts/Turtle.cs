using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turtle : MonoBehaviour
{
    private class TurtleTransform
    {
        public Vector3 Position{get;}
        public Quaternion Orientation{get;}

        public TurtleTransform(Vector3 position, Quaternion orientation)
        {
            Position = position;
            Orientation = orientation;
        }
    }


    public Vector3 Position{get; private set;}
    public Quaternion Orientation{get; private set;}

    private Stack<TurtleTransform> stack = new Stack<TurtleTransform>();

    private GameObject branchPrefab;
    public Turtle(Vector3 position, GameObject branchPrefab)
    {
        Position = position;
        this.branchPrefab = branchPrefab;
    }


    public void Translate(Vector3 delta)
    {
        delta = Orientation * delta;
        GameObject branch = Instantiate(branchPrefab);
        var points = new Vector3[2];
        branch.transform.localPosition = Position;
        points[0] = Position;
        points[1] = Position + delta;
        branch.GetComponent<LineRenderer>().SetPositions(points);

        Position += delta;
    }

    public void Rotate(Vector3 delta) => Orientation = Quaternion.Euler(Orientation.eulerAngles + delta);

    public void Push() => stack.Push(new TurtleTransform(Position, Orientation));

    public void Pop()
    {
        var poppedTransform = stack.Pop();
        Position = poppedTransform.Position;
        Orientation = poppedTransform.Orientation;
    }
}
