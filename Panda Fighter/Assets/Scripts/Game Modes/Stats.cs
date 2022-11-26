using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

public class Stats : MonoBehaviour
{
    private static Dictionary<Transform, KDA> statsManager = new Dictionary<Transform, KDA>();
    public Transform Player;
    public Transform AIs;

    public Text scores;
    private StringBuilder statReader = new StringBuilder();

    void Start()
    {
        statsManager[Player] = new KDA(0, 0, 0);

        for (int index = 0; index < AIs.childCount; index++)
            statsManager[AIs.GetChild(index)] = new KDA(0, 0, 0);
    }

    void Update()
    {
        scores.text = "";
        foreach (KeyValuePair<Transform, KDA> stat in statsManager)
        {
            statReader.Clear();
            statReader.Append(scores.text + stat.Key.name + ": " + stat.Value.ToString() + " \n");
            scores.text = statReader.ToString();
        }
    }

    public static void confirmKillFor(Transform killer)
    {
        KDA kda = statsManager[killer];
        kda.GotKill();
        statsManager[killer] = kda;
    }

    public static void confirmDeathFor(Transform victim)
    {
        KDA kda = statsManager[victim];
        kda.GotDeath();
        statsManager[victim] = kda;
    }

    public static void confirmAssistfor(Transform assister)
    {
        KDA kda = statsManager[assister];
        kda.GotAssist();
        statsManager[assister] = kda;
    }
}
