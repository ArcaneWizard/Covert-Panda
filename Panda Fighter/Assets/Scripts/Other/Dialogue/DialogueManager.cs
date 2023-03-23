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

    // Specify character speaking
    public void Specify(Character name) => characterSpeaking = name;

    // Add line of dialogue. If linger duration isn't specified, it's auto set to be 
    // proportional to how long the dialogue is
    public void AddLine(string line, float duration = -1.0f)
    {
        dialogue.Enqueue(new DialogueLine(characterSpeaking, line, duration));
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