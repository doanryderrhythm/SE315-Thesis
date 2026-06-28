using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class ConnectToServer : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject panel;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SetState(panel, true);

        if (!PhotonNetwork.IsConnected)
            PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("Lobby Joined.");
        SetState(panel, false);
    }

    public override void OnLeftLobby()
    {
        Debug.Log("Lobby Left.");
        SetState(panel, true);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log($"Disconnected from server: {cause}");
        SetState(panel, true);

        PhotonNetwork.ConnectUsingSettings();
    }

    private void SetState(GameObject obj, bool state)
    {
        if (obj)
        {
            obj.SetActive(state);
        }
    }
}
