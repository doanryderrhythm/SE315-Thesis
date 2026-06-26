using UnityEngine;

public class PlayerSpawnerTester : MonoBehaviour
{
    public void TestSpawnPlayers()
    {
        PlayerSpawner spawner = FindAnyObjectByType<PlayerSpawner>();

        if (spawner == null)
        {
            Debug.LogError("PlayerSpawner not found!");
            return;
        }

        Debug.Log("=== TEST PLAYER SPAWNER ===");

        spawner.SpawnPlayersForNewRound();
    }
}