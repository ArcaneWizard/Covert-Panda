using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    private static Dictionary<Transform, KDA> statsManager = new Dictionary<Transform, KDA>();
    public Transform Player;
    public Transform AIs;

    public Text Scores;

    public static Stats Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
            Instance = this;
    }

    public void ConfirmKillFor(Transform killer)
    {
        KDA kda = statsManager[killer];
        kda.AddKill();
        statsManager[killer] = kda;
    }

    public void ConfirmDeathFor(Transform victim)
    {
        KDA kda = statsManager[victim];
        kda.AddDeath();
        statsManager[victim] = kda;
    }

    public static void ConfirmAssistFor(Transform assister)
    {
        KDA kda = statsManager[assister];
        kda.AddAssist();
        statsManager[assister] = kda;
    }

    void Start()
    {
        statsManager[Player] = new KDA(0, 0, 0);

        for (int index = 0; index < AIs.childCount; index++)
            statsManager[AIs.GetChild(index)] = new KDA(0, 0, 0);
    }

    /*
    void Update()
    {
        statReader.Clear();
        foreach (KeyValuePair<Transform, KDA> stat in statsManager)
            statReader.Append(scores.text + stat.Key.name + ": " + stat.Value.ToString() + " \n");

        scores.text = statReader.ToString();
    }
    */
}
