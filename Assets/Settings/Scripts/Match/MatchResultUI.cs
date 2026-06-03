using TMPro;
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

    void Start()
    {
        gameManager = GameManager.Instance;

        if (gameManager == null)
        {
            Debug.LogError("GameManager not found!");
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

            bool isLocalPlayer =
                player.isLocalPlayer;

            row.GetComponent<LeaderboardRowUI>()
                .Setup(player, isLocalPlayer);
        }
    }

    void SetupPodium()
    {
        if (gameManager.players.Count > 0)
        {
            firstPlace.SetPlayer(gameManager.players[0]);
        }
        else
        {
            firstPlace.gameObject.SetActive(false);
        }

        if (gameManager.players.Count > 1)
        {
            secondPlace.SetPlayer(gameManager.players[1]);
        }
        else
        {
            secondPlace.gameObject.SetActive(false);
        }

        if (gameManager.players.Count > 2)
        {
            thirdPlace.SetPlayer(gameManager.players[2]);
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
        SceneManager.LoadScene("MainMenu");
    }
}