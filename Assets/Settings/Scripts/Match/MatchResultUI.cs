using NUnit.Framework;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchResultUI : MonoBehaviour
{
    [Header("Podium")]
    public PodiumSlotUI firstPlace;
    public PodiumSlotUI secondPlace;
    public PodiumSlotUI thirdPlace;

    [Header("Leaderboard")]
    public Transform rowsContainer;
    public GameObject leaderboardRowPrefab;

    GameManager gameManager;

    Photon.Realtime.Player[] players;
    List<PlayerMatchData> playerDatas;

    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            return;
        }

        GenerateLeaderboard();
        SetupPodium();
    }

    void GenerateLeaderboard()
    {
        foreach (Transform child in rowsContainer)
        {
            Destroy(child.gameObject);
        }

        players = PhotonNetwork.PlayerList;
        if (players == null || players.Length == 0)
            return;

        Array.Sort(players, (a, b) => b.GetScore().CompareTo(a.GetScore()));

        playerDatas = new List<PlayerMatchData>();

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
            playerData.isLocalPlayer = players[i].IsLocal;

            playerDatas.Add(playerData);

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

    void SetupPodium()
    {
        if (playerDatas.Count > 0)
        {
            firstPlace.SetPlayer(playerDatas[0]);
        }
        else
        {
            firstPlace.gameObject.SetActive(false);
        }

        if (playerDatas.Count > 1)
        {
            secondPlace.SetPlayer(playerDatas[1]);
        }
        else
        {
            secondPlace.gameObject.SetActive(false);
        }

        if (playerDatas.Count > 2)
        {
            thirdPlace.SetPlayer(playerDatas[2]);
        }
        else
        {
            thirdPlace.gameObject.SetActive(false);
        }
    }

    public void OnReturnToRoom()
    {
        SceneManager.LoadScene("MapSelectionScene");
    }

    public void OnLeaveMatch()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.LeaveRoom();
        }

        if (NetworkManager.Singleton != null &&
            NetworkManager.Singleton.IsListening)
        {
            NetworkManager.Singleton.Shutdown();
        }

        if (GameManager.Instance)
            Destroy(GameManager.Instance.gameObject);

        GameSettings.selectedMaps.Clear();
        GameSettings.selectedMapsThemeTrack = null;

        SceneManager.LoadScene("Main Menu");
    }
}