using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls how the L-Systems plants are generated
// With help from: https://www.youtube.com/watch?v=PECzSINrc60
public class LSystem
{
    private string sentence;
    private Dictionary<string, string> ruleset;

    private Dictionary<string, Action<Turtle>> turtleCommands;
    private Turtle turtle;

    // The constructor used by the L-System to define how the plant should be structured
    public LSystem(string axiom, Dictionary<string, string> ruleset, Dictionary<string, Action<Turtle>> turtleCommands, Vector3 initialPos, GameObject branchPrefab)
    {
        this.sentence = axiom;
        this.ruleset = ruleset;
        this.turtleCommands = turtleCommands;

        turtle = new Turtle(initialPos, branchPrefab);
    }

    // Function that tells the turtle to run a command based on what character is read out from the instruction sentence
    public void DrawSystem()
    {
        foreach(var instruction in sentence)
        {
            if(turtleCommands.TryGetValue(instruction.ToString(), out var command))
            {
                command(turtle);
            }
        }
    }

    // Generates a sentence by running it through the IterateSentence function
    public string GenerateSentence()
    {
        sentence = IterateSentence(sentence);
        return sentence;
    }

    // Checks through each and every letter in the sentence to see if it matches the ruleset
    private string IterateSentence(string oldSentence)
    {
        var newSentence = "";

        foreach(var c in oldSentence)
        {
            if(ruleset.TryGetValue(c.ToString(), out var replacement))
            {
                newSentence += replacement;
            }
            else
            {
                newSentence += c;
            }
        }

        return newSentence;
    }
}
