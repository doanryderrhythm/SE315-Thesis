using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class WeaponSpawner : NetworkBehaviour
{
    [Header("Spawner Configuration")]
    [SerializeField] private float spawnInterval = 10f;
    [SerializeField] private int maxActivePickups = 5;
    [SerializeField] private float minimumDistanceFromPlayers = 5f;
    [SerializeField] private float pickupLifetime = 30f;
    [SerializeField] private LayerMask obstacleLayerMask;

    [Header("Spawn Points & Prefabs")]
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private GameObject[] weaponPickupPrefabs; 

    private List<NetworkObject> activePickups = new List<NetworkObject>();

    public override void OnNetworkSpawn()
    {
        if (!IsServer) return;

        StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitForSeconds(spawnInterval);

        while (true)
        {
            TrySpawn();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void TrySpawn()
    {
        if (activePickups.Count >= maxActivePickups)
        {
            Debug.Log("[WeaponSpawner] Max active pickups reached, skipping spawn attempt.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            Debug.LogWarning("[WeaponSpawner] No spawn points assigned!");
            return;
        }

        if (weaponPickupPrefabs == null || weaponPickupPrefabs.Length == 0)
        {
            Debug.LogWarning("[WeaponSpawner] No weapon pickup prefabs assigned!");
            return;
        }

        // Build a list of valid spawn points first
        List<Transform> validSpawnPoints = new List<Transform>();
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;

            if (IsValidSpawnPosition(point.position))
            {
                validSpawnPoints.Add(point);
            }
        }

        if (validSpawnPoints.Count == 0)
        {
            Debug.Log("[WeaponSpawner] No valid spawn positions available (all blocked or too close to players).");
            return;
        }

        //Random weapon
        Transform selectedPoint = validSpawnPoints[Random.Range(0, validSpawnPoints.Count)];

        GameObject randomPrefab = weaponPickupPrefabs[Random.Range(0, weaponPickupPrefabs.Length)];

        GameObject spawnedObj = Instantiate(randomPrefab, selectedPoint.position, Quaternion.identity);
        Debug.Log($"Spawning pickup: {spawnedObj.name}");

        NetworkObject netObj = spawnedObj.GetComponent<NetworkObject>();
        Debug.Log($"Has NetworkObject: {netObj != null}");

        if (netObj != null)
        {
            netObj.Spawn();
            Debug.Log($"IsSpawned: {netObj.IsSpawned}");
            activePickups.Add(netObj);

            NetworkedCollectible collectible = spawnedObj.GetComponent<NetworkedCollectible>();
            if (collectible != null)
            {
                collectible.Initialize(this);
            }

            StartCoroutine(DestroyAfterLifetimeRoutine(netObj, pickupLifetime));
        }
        else
        {
            Destroy(spawnedObj);
            Debug.LogError("[WeaponSpawner] Spawned prefab does not have a NetworkObject component!");
        }
    }

    private bool IsValidSpawnPosition(Vector2 position)
    {
        foreach (NetworkObject pickup in activePickups)
        {
            if (pickup == null) continue;

            if (Vector2.Distance(position,
                pickup.transform.position) < 0.5f)
            {
                return false;
            }
        }

        Collider2D obstacleCollider = Physics2D.OverlapCircle(position, 0.4f, obstacleLayerMask);
        if (obstacleCollider != null)
        {
            return false;
        }

        Player[] players = FindObjectsByType<Player>(FindObjectsSortMode.None);
        foreach (Player player in players)
        {
            if (player == null) continue;

            float distance = Vector2.Distance(position, player.transform.position);
            if (distance < minimumDistanceFromPlayers)
            {
                return false;
            }
        }

        return true;
    }

    private IEnumerator DestroyAfterLifetimeRoutine(NetworkObject netObj, float lifetime)
    {
        yield return new WaitForSeconds(lifetime);

        if (netObj != null && netObj.IsSpawned && IsServer)
        {
            netObj.Despawn();
        }
    }

    public void OnPickupDespawned(NetworkObject netObj)
    {
        if (activePickups.Contains(netObj))
        {
            activePickups.Remove(netObj);
        }
    }
}
