using UnityEngine;
using System;

[System.Serializable]
public struct KDA
{
    public int kills { get; private set; }
    public int deaths { get; private set; }
    public int assists { get; private set; }

    public KDA(int kills, int deaths, int assists)
    {
        this.kills = kills;
        this.deaths = deaths;
        this.assists = assists;
    }

    public void GotKill() => ++kills;
    public void GotDeath() => ++deaths;
    public void GotAssist() => ++assists;

    public String ToString() => $"(K: {kills}, D: {deaths}, A: {assists})";
}