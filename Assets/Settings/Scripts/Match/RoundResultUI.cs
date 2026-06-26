using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundResultUI : MonoBehaviour
{
    [Header("Top UI")]
    public TMP_Text mapInfoText;
    public TMP_Text countdownText;

    [Header("Personal Summary")]
    public TMP_Text rankOldText;
    public TMP_Text rankNewText;

    public TMP_Text scoreValueText;
    public TMP_Text scoreGainText;

    public TMP_Text killsValueText;
    public TMP_Text killsGainText;

    public TMP_Text deathsValueText;
    public TMP_Text deathsGainText;

    [Header("Leaderboard")]
    public Transform rowsContainer;
    public GameObject leaderboardRowPrefab;

    float timer = 6f;

    GameManager gameManager;

    void Start()
    {
        gameManager = GameManager.Instance;
        if (gameManager == null)
            return;

        LoadUI();
        GenerateLeaderboard();
        StartCoroutine(CountdownRoutine());
    }

    void LoadUI()
    {
        int nextRound = gameManager.currentMapIndex + 2;

        if (nextRound <= gameManager.selectedMaps.Count)
        {
            MapData nextMap =
                gameManager.selectedMaps[nextRound - 1];

            mapInfoText.text =
                nextMap.mapName.ToUpper()
                + " MAP INCOMING "
                + "("
                + (gameManager.currentMapIndex + 1)
                + "/"
                + gameManager.selectedMaps.Count
                + ")";
        }
        else
        {
            mapInfoText.text =
                "FINAL ROUND COMPLETE "
                + "("
                + gameManager.selectedMaps.Count
                + "/"
                + gameManager.selectedMaps.Count
                + ")";
        }

        /*
        PlayerMatchData localPlayer =
            gameManager.players[0];

        scoreValueText.text =
            localPlayer.totalScore.ToString();

        scoreGainText.text =
            " + " + localPlayer.gainedScore;

        killsValueText.text =
            localPlayer.totalKills.ToString();

        killsGainText.text =
            " + " + localPlayer.gainedKills;

        deathsValueText.text =
            localPlayer.totalDeaths.ToString();

        deathsGainText.text =
            " + " + localPlayer.gainedDeaths;

        rankOldText.text =
            localPlayer.previousRank.ToString();

        rankNewText.text =
            " ->" + localPlayer.rank.ToString();

        bool changed =
            localPlayer.previousRank != localPlayer.rank;

        rankNewText.gameObject.SetActive(changed);

        if (!changed)
        {
            rankOldText.gameObject.SetActive(false);
        }
        */
    }

    void GenerateLeaderboard()
    {
        foreach (Transform child in rowsContainer)
        {
            Destroy(child.gameObject);
        }

        Photon.Realtime.Player[] players = PhotonNetwork.PlayerList;
        if (players == null || players.Length == 0)
            return;

        Array.Sort(players, (a, b) => b.GetScore().CompareTo(a.GetScore()));

        for (int i = 0; i < players.Length; i++)
        {
            PlayerMatchData playerData = new PlayerMatchData();
            playerData.playerName = players[i].NickName;
            playerData.rank = i + 1;
            playerData.totalScore = players[i].GetScore();
            if (players[i].CustomProperties.ContainsKey("Kills"))
                playerData.totalKills = (int)players[i].CustomProperties["Kills"];
            if (players[i].CustomProperties.ContainsKey("Deaths"))
                playerData.totalDeaths = (int)players[i].CustomProperties["Deaths"];

            GameObject row =
                Instantiate(
                    leaderboardRowPrefab,
                    rowsContainer
                );

            bool isLocalPlayer = players[i].IsLocal;

            row.GetComponent<LeaderboardRowUI>()
                .Setup(playerData, isLocalPlayer);
        }
    }

    IEnumerator CountdownRoutine()
    {
        while (timer > 0)
        {
            countdownText.text =
                "next match starts in " +
                Mathf.Ceil(timer) +
                " sec";

            timer -= Time.deltaTime;

            yield return null;
        }

        StartNextRound();
    }

    void StartNextRound()
    {
        gameManager.LoadNextRound();
    }
}