using UnityEngine;
using System;

[System.Serializable]
public struct KDA
{
    public int Kills { get; private set; }
    public int Deaths { get; private set; }
    public int Assists { get; private set; }

    public KDA(int kills, int deaths, int assists)
    {
        Kills = kills;
        Deaths = deaths;
        Assists = assists;
    }

    public void AddKill() => ++Kills;
    public void AddDeath() => ++Deaths;
    public void AddAssist() => ++Assists;

    public override String ToString() => $"(K: {Kills}, D: {Deaths}, A: {Assists})";
}