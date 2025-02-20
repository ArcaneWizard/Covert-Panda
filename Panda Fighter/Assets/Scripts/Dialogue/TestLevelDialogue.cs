using UnityEngine;
using UnityEngine.UI;

public class TestLevelDialogue : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text dialogue;

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
        if (timer < 0f) {
            DialogueLine line = dM.GetLine();

            if (line == null) {
                name.text = "";
                dialogue.text = "";
            } else {
                timer = line.LingerDuration;
                name.text = line.Name.ToString().ToUpper() + ":";
                dialogue.text = line.Dialogue.ToString();
            }
        } else {
            timer -= Time.deltaTime;
        }
    }
}
