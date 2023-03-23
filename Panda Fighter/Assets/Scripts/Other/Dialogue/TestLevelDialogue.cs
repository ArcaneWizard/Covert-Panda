using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TestLevelDialogue : MonoBehaviour
{
    [SerializeField] private Text Name;
    [SerializeField] private Text Dialogue;

    private DialogueManager dM;
    private float timer;

    private void Awake()
    {
        dM = new DialogueManager();
    }

    // write dialogue
    void Start()
    {
        dM.Specify(Character.Akabe);
        dM.AddLine("These people don't deserve to watch this land burn...");

        dM.Specify(Character.Gargan);
        dM.AddLine("I sacrificed my throne for those poor people, AND WHAT DID I GET IN RETURN?");
        dM.AddLine("Nothing. But that's the life we were born with. That IS the life of the ruler.");

        dM.Specify(Character.You);
        dM.AddLine("Don't lie to me. I know yo took the casket!");
    }

    // execute dialogue with correct timing and order
    void Update()
    {
        if (timer < 0f)
        {
            DialogueLine line = dM.GetLine();

            if (line == null)
            {
                Name.text = "";
                Dialogue.text = "";
            }
            else
            {
                timer = line.LingerDuration;
                Name.text = line.Name.ToString().ToUpper() + ":";
                Dialogue.text = line.Dialogue.ToString();
            }
        }
        else
            timer -= Time.deltaTime;
    }
}
