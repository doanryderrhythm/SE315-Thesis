using TMPro;
using UnityEngine;

public class LeaderboardRowUI : MonoBehaviour
{
    public TMP_Text rankText;
    public TMP_Text playerNameText;
    public TMP_Text scoreText;
    public TMP_Text killsText;
    public TMP_Text deathsText;

    public GameObject highlightBorder;

    public void Setup(PlayerMatchData data, bool isLocalPlayer)
    {
        rankText.text = data.rank.ToString();

        playerNameText.text = data.playerName;

        scoreText.text = data.totalScore.ToString();

        killsText.text = data.totalKills.ToString();

        deathsText.text = data.totalDeaths.ToString();

        highlightBorder.SetActive(isLocalPlayer);
    }
}