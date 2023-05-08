using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    // Creates a plant using L-Systems
    // Made with help from: https://www.youtube.com/watch?v=PECzSINrc60
    public class SimplePlant : MonoBehaviour
    {

        private string axiom =  "F";
        public GameObject branchPrefab;

        // Defines the sentense that the L-Systems scripts follows to produce the plant
        private Dictionary<string, string> ruleset = new Dictionary<string, string>
        {
            {"F", "FF+[+F-F-F]-F-[-F+F+F]"}
        };

        // Defines all the commands that the Turtle which draws the plant follows
        // F = Move forward
        // + = Turn Clockwise
        // - = Turn Anti-Clockwise
        // [ = Push onto the stack. Practically a "save this position"
        // ] = Pop off the stack. Basically go back to the position where we called the push function
        private Dictionary<string, Action<Turtle>> commands = new Dictionary<string, Action<Turtle>>
        {
            {"F", turtle => turtle.Translate(new Vector3(0, 5f, 0))}, 
            {"+", turtle => turtle.Rotate(new Vector3(20f, 0f, 0))},
            {"-", turtle => turtle.Rotate(new Vector3(-20f, 0f, 0))},
            {"[", turtle => turtle.Push()},
            {"]", turtle => turtle.Pop()}
        };

        private void Start()
        {
            var lSystem = new LSystem(axiom, ruleset, commands, transform.position, branchPrefab);
            Debug.Log(lSystem.GenerateSentence());
            Debug.Log(lSystem.GenerateSentence());
            lSystem.DrawSystem();
        }
    }   
}