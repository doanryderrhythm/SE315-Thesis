using Photon.Pun;
using System.Threading;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;

public class ArenaManager : MonoBehaviourPunCallbacks
{
    [SerializeField] PlayerSpawner playerSpawner;

    private CancellationTokenSource _cts;

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
}