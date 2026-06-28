using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_Text numberOfPlayersText;

    [SerializeField] UnityEvent onGameStart;

    private void Awake()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public void CreateRoom()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {
            PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions()
            {
                MaxPlayers = 4,
                IsVisible = true,
                IsOpen = true
            }, TypedLobby.Default, null);

            return;
        }

        if (!PhotonNetwork.IsConnected || PhotonNetwork.NetworkClientState == ClientState.ConnectedToNameServer)
        {
            Debug.LogWarning($"Photon was on the {PhotonNetwork.NetworkClientState}. Connecting to Master server...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public void JoinRoom(string roomId)
    {
        PhotonNetwork.JoinRoom(roomId);
    }

    public override void OnJoinedRoom()
    {
        if (SceneManager.GetActiveScene().name == "Selection Screen")
            PhotonNetwork.LoadLevel("MapSelectionScene");
        else
            onGameStart.Invoke();
    }

    private void Start()
    {
        UpdateNumberOfPlayers(SceneManager.GetActiveScene().name == "MapSelectionScene");
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        UpdateNumberOfPlayers(SceneManager.GetActiveScene().name == "MapSelectionScene");
    }

    void UpdateNumberOfPlayers(bool isWaiting)
    {
        if (numberOfPlayersText)
            numberOfPlayersText.text = "NUMBER OF PLAYERS: " + PhotonNetwork.PlayerList.Length;
    }
}
