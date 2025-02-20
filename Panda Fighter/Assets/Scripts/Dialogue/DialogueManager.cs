using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used to display dialogue with the correct order and timing
public class DialogueManager
{
    private Queue<DialogueLine> dialogue;
    private Character characterSpeaking;

    public DialogueManager()
    {
        dialogue = new Queue<DialogueLine>();
    }

    /// <summary> Specify character speaking </summary>
    public void Specify(Character character) => characterSpeaking = character;

    /// <summary> Adds line of dialogue. If linger duration isn't specified, it's auto set to be 
    /// proportional to how long the dialogue is </summary>
    public void AddLine(string line, float duration = -1.0f)
    {
        dialogue.Enqueue(new DialogueLine(characterSpeaking, line, duration));
    }

    /// <summary> Adds line of dialogue and voice action (ex. shrieking). 
    /// If linger duration isn't specified, it's auto set to be 
    /// proportional to how long the dialogue is </summary>
    public void AddLine(string action, string line, float duration = -1.0f)
    {
        dialogue.Enqueue(new DialogueLine(characterSpeaking, $"[{action}] line", duration));
    }

    // Retrieve line of dialogue.
    public DialogueLine GetLine()
    {
        if (dialogue.Count <= 0)
            return null;

        return dialogue.Dequeue();
    }
}

// Structure representing a line of spoken dialogue
public class DialogueLine 
{
    public Character Name;
    public string Dialogue;
    public float LingerDuration; // in seconds

    private const float charactersPerSecond = 7;

    public DialogueLine(Character name, string dialogue, float duration)
    {
        Name = name;
        Dialogue = dialogue;

        if (duration <= 0f)
            LingerDuration = dialogue.Length / charactersPerSecond;
        else
            LingerDuration = duration;
    } 
}