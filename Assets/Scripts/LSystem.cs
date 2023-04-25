using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LSystem
{
    private string sentence;
    private Dictionary<string, string> ruleset;

    private Dictionary<string, Action<Turtle>> turtleCommands;
    private Turtle turtle;

    public LSystem(string axiom, Dictionary<string, string> ruleset, Dictionary<string, Action<Turtle>> turtleCommands, Vector3 initialPos)
    {
        this.sentence = axiom;
        this.ruleset = ruleset;
        this.turtleCommands = turtleCommands;

        turtle = new Turtle(initialPos);
    }

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

    public string GenerateSentence()
    {
        sentence = IterateSentence(sentence);
        return sentence;
    }

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
