using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    private static Dictionary<Transform, KDA> everyonesStats = new Dictionary<Transform, KDA>();
    public Transform Player;
    public Transform AIs;

    public Text scores;

    void Start()
    {
        everyonesStats[Player] = new KDA(0, 0, 0);

        for (int index = 0; index < AIs.childCount; index++)
            everyonesStats[AIs.GetChild(index)] = new KDA(0, 0, 0);
    }

    void Update()
    {
        scores.text = "";
        foreach (KeyValuePair<Transform, KDA> stat in everyonesStats)
        {
            String s = scores.text;
            s += stat.Key.name + ": " + stat.Value.ToString() + " \n";
            scores.text = s;
        }
    }

    public static void confirmKillFor(Transform killer)
    {
        KDA kda = everyonesStats[killer];
        kda.GotKill();
        everyonesStats[killer] = kda;
    }

    public static void confirmDeathFor(Transform victim)
    {
        KDA kda = everyonesStats[victim];
        kda.GotDeath();
        everyonesStats[victim] = kda;
    }

    public static void confirmAssistfor(Transform assister)
    {
        KDA kda = everyonesStats[assister];
        kda.GotAssist();
        everyonesStats[assister] = kda;
    }
}
