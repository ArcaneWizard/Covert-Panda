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
        dM.AddLine(Voice.Shrieking, "VORTEVO, THE BRIDGE... it's collapsing");

        dM.Specify(Character.Vortevo);
        dM.AddLine(Voice.Shouting, "I JUST NEED A FEW SECONDS.");
        dM.AddLine("ALMOST THERE");
        dM.AddLine("ON MY MARK... ... FIRE");


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
        {
            timer -= Time.deltaTime;
        }
    }
}
