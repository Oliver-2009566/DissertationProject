using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// The turtle classes which draw the L-Systems plants
// Made with help from: https://www.youtube.com/watch?v=PECzSINrc60
public class Turtle : MonoBehaviour
{
    // A class which defines a position the turtle was in. Used for pushing and popping
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

    // The position of the turtle
    public Vector3 Position{get; private set;}
    // The rotation of the circle
    public Quaternion Orientation{get; private set;}
    // The previous positions of the turtle when it pushes onto the stack
    private Stack<TurtleTransform> stack = new Stack<TurtleTransform>();
    // A branch object used to build the structure of my L-System plants
    private GameObject branchPrefab;
    public Turtle(Vector3 position, GameObject branchPrefab)
    {
        Position = position;
        this.branchPrefab = branchPrefab;
    }

    // Builds a branch to the next point in the L-System then moves to the next point
    public void Translate(Vector3 delta)
    {
        // Delta is where the turtule will end up
        delta = Orientation * delta;
        // We create a branch object and set its position to to where the turtle currently is
        GameObject branch = Instantiate(branchPrefab);
        branch.transform.localPosition = Position;
        // Creates an array of Vector3 positions which define where the two points for a line should be
        // Then tells the branch to set those positions in its line renderer, creating a branch out of the newly rendered line
        var points = new Vector3[2];
        points[0] = Position;
        points[1] = Position + delta;
        branch.GetComponent<LineRenderer>().SetPositions(points);
        // Updates the position of the turtle
        Position += delta;
    }

    // Simple rotates the turtle
    public void Rotate(Vector3 delta) => Orientation = Quaternion.Euler(Orientation.eulerAngles + delta);

    // Pushes its current position and orientation onto the stack
    public void Push() => stack.Push(new TurtleTransform(Position, Orientation));

    // Pops its position and orientation off the stack and sets its position and orientation to that when it pushed onto the stack
    public void Pop()
    {
        var poppedTransform = stack.Pop();
        Position = poppedTransform.Position;
        Orientation = poppedTransform.Orientation;
    }
}
