using Photon.Pun;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class ArenaManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerSpawner playerSpawner;

    private CancellationTokenSource _cts;

    public static UnityEvent OnPlayerDead = new UnityEvent();

    public override void OnEnable()
    {
        OnPlayerDead.AddListener(CheckPlayers);
    }

    public override void OnDisable()
    {
        OnPlayerDead.RemoveListener(CheckPlayers);
    }

    async void Start()
    {
        if (!NetworkManager.Singleton.IsListening)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                NetworkManager.Singleton.StartHost();
            }
            else
            {
                NetworkManager.Singleton.StartClient();
            }
        }

        if (!PhotonNetwork.IsMasterClient) return;

        _cts = new CancellationTokenSource();
        await WaitForPlayersAsync(_cts.Token);
    }

    private void OnDestroy()
    {
        _cts?.Cancel();
        _cts?.Dispose();
    }

    private async Task WaitForPlayersAsync(CancellationToken token)
    {
        int expected = PhotonNetwork.PlayerList.Length;

        while (NetworkManager.Singleton.ConnectedClientsList.Count < expected)
        {
            if (token.IsCancellationRequested) return;
            await Task.Delay(500, token);
        }

        playerSpawner.SpawnPlayersForNewRound();
    }

    public void CheckPlayers()
    {
        var clients = NetworkManager.Singleton.ConnectedClientsList;

        int numberOfSurvivedClients = 0;
        foreach (var client in clients)
        {
            if (client.PlayerObject == null)
                continue;

            if (client.PlayerObject.GetComponent<Player>().isDead.Value == false)
                numberOfSurvivedClients += 1;
        }

        if (numberOfSurvivedClients <= 1)
            GameManager.Instance.EndCurrentRound();
    }
}