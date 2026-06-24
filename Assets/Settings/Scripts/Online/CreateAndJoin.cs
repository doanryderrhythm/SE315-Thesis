using TMPro;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class CreateAndJoin : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField roomNameInputField;

    [SerializeField] UnityEvent onGameStart;

    public void CreateRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.CreateRoom(roomNameInputField.text, new RoomOptions()
        {
            MaxPlayers = 4,
            IsVisible = true,
            IsOpen = true
        }, TypedLobby.Default, null);
    }

    public void JoinRoom(string roomId)
    {
        PhotonNetwork.JoinRoom(roomId);
    }

    public override void OnJoinedRoom()
    {
        if (SceneManager.GetActiveScene().name == "Selection Screen")
            PhotonNetwork.LoadLevel("GameScene");
        else
            onGameStart.Invoke();
    }
}
