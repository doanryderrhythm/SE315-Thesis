using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [Header("Spawn Points Configuration")]
    [SerializeField] private Transform[] spawnPoints;

    /// <summary>
    /// Repositions all connected players to unique spawn points and resets their gameplay state.
    /// Only runs on the Server.
    /// </summary>
    public void SpawnPlayersForNewRound()
    {
        if (NetworkManager.Singleton == null || !NetworkManager.Singleton.IsServer)
        {
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[PlayerSpawner] No spawn points assigned in the Inspector!");
            return;
        }

        StartCoroutine(SpawnPlayersCoroutine());
    }

    private IEnumerator SpawnPlayersCoroutine()
    {
        while (true)
        {
            bool ready = true;

            foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
            {
                if (client.PlayerObject == null)
                {
                    ready = false;
                    break;
                }
            }

            if (ready)
                break;

            yield return null;
        }

        yield return new WaitForFixedUpdate();

        var clients = NetworkManager.Singleton.ConnectedClientsList;

        List<Transform> shuffledSpawns = new List<Transform>(spawnPoints);
        for (int i = shuffledSpawns.Count - 1; i > 0; i--)
        {
            int rnd = Random.Range(0, i + 1);
            Transform temp = shuffledSpawns[i];
            shuffledSpawns[i] = shuffledSpawns[rnd];
            shuffledSpawns[rnd] = temp;
        }

        int spawnIndex = 0;
        foreach (var client in clients)
        {
            if (spawnIndex >= shuffledSpawns.Count)
            {
                Debug.LogWarning("[PlayerSpawner] Not enough spawn points for all connected players!");
                break;
            }

            Transform spawnPoint = shuffledSpawns[spawnIndex];
            if (spawnPoint == null)
            {
                spawnIndex++;
                continue;
            }

            NetworkObject playerNetObj = client.PlayerObject;
            if (playerNetObj != null)
            {
                RepositionPlayer(playerNetObj, spawnPoint);
                spawnIndex++;
            }
            else
            {
                Debug.LogWarning($"[PlayerSpawner] PlayerObject is null for client {client.ClientId}. Skipping reposition.");
            }
        }
    }

    private void RepositionPlayer(NetworkObject playerNetObj, Transform spawnPoint)
    {
        var rb = playerNetObj.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }

        playerNetObj.transform.SetPositionAndRotation(
            spawnPoint.position,
            spawnPoint.rotation
        );

        if (rb != null)
        {
            rb.position = spawnPoint.position;
        }

        var player = playerNetObj.GetComponent<Player>();
        if (player != null)
        {
            player.ResetPlayerState();
        }

        Physics2D.SyncTransforms();

        Debug.Log(
            $"Player {playerNetObj.OwnerClientId} moved to {playerNetObj.transform.position}"
        );
    }
}
