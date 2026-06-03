using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PodiumSlotUI : MonoBehaviour
{
    [SerializeField] private TMP_Text playerNameText;

    public void SetPlayer(PlayerMatchData player)
    {
        if (player == null)
        {
            gameObject.SetActive(false);
            return;
        }

        gameObject.SetActive(true);

        playerNameText.text = player.playerName;
    }
}