using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class RoundResultUI : MonoBehaviour
{
    [Header("Top UI")]
    public TMP_Text mapInfoText;
    public TMP_Text countdownText;

    [Header("Personal Summary")]
    public TMP_Text scoreText;
    public TMP_Text killsText;
    public TMP_Text deathsText;
    public TMP_Text rankText;

    [Header("Leaderboard")]
    public Transform rowsContainer;
    public GameObject leaderboardRowPrefab;

    float timer = 6f;

    GameManager gameManager;

    void Start()
    {
        gameManager = FindFirstObjectByType<GameManager>();

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

        PlayerMatchData localPlayer =
            gameManager.players[0];

        scoreText.text =
            localPlayer.totalScore +
            " + " +
            localPlayer.gainedScore;

        killsText.text =
            localPlayer.totalKills +
            " + " +
            localPlayer.gainedKills;

        deathsText.text =
            localPlayer.totalDeaths +
            " + " +
            localPlayer.gainedDeaths;

        rankText.text =
            localPlayer.rank.ToString();
    }

    void GenerateLeaderboard()
    {
        foreach (Transform child in rowsContainer)
        {
            Destroy(child.gameObject);
        }

        gameManager.players.Sort((a, b) =>
            b.totalScore.CompareTo(a.totalScore));

        for (int i = 0; i < gameManager.players.Count; i++)
        {
            gameManager.players[i].rank = i + 1;
        }

        for (int i = 0; i < gameManager.players.Count; i++)
        {
            PlayerMatchData player =
                gameManager.players[i];

            GameObject row =
                Instantiate(
                    leaderboardRowPrefab,
                    rowsContainer
                );

            bool isLocalPlayer = player.isLocalPlayer;

            row.GetComponent<LeaderboardRowUI>()
                .Setup(player, isLocalPlayer);
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