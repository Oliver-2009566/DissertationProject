using System;
using System.Collections.Generic;
using UnityEngine;

namespace DefaultNamespace
{
    public class SimplePlant : MonoBehaviour
    {
        /*
        axiom = F
        f -> FF+[+F-F-F]-[-F+F+F]
        angle = 22.5    
        */

        private string axiom =  "F";

        private Dictionary<string, string> ruleset = new Dictionary<string, string>
        {
            {"F", "FF+[+F-F-F]-[-F+F+F]"}
        };

        private Dictionary<string, Action<Turtle>> commands = new Dictionary<string, Action<Turtle>>
        {
            {"F", turtle => turtle.Translate(new Vector3(0, 0.1f, 0))}, 
            {"+", turtle => turtle.Rotate(new Vector3(25f, 0, 0))},
            {"-", turtle => turtle.Rotate(new Vector3(-25f, 0, 0))},
            {"[", turtle => turtle.Push()},
            {"]", turtle => turtle.Pop()}
        };

        private void Start()
        {
            var lSystem = new LSystem(axiom, ruleset, commands, transform.position);
            Debug.Log(lSystem.GenerateSentence());
            Debug.Log(lSystem.GenerateSentence());
            lSystem.DrawSystem();
        }
    }   
}