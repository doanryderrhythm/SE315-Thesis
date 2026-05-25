using UnityEngine;

[System.Serializable]
public class PlayerMatchData
{
    public bool isLocalPlayer;
    public string playerName;

    // TOTAL MATCH STATS
    public int previousRank;
    public int totalScore;
    public int totalKills;
    public int totalDeaths;

    // ROUND GAIN
    public int gainedScore;
    public int gainedKills;
    public int gainedDeaths;

    public int rank;
}