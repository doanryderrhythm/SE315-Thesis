using Unity.Netcode;
using UnityEngine;

public class NetworkedCollectible : NetworkBehaviour
{
    [SerializeField] private WeaponType weaponType;

    private WeaponSpawner spawner;

    public void Initialize(WeaponSpawner ownerSpawner)
    {
        spawner = ownerSpawner;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!IsServer) return;

        Player player = other.GetComponent<Player>();
        if (player == null) return;

        ApplyWeapon(player);

        if (GetComponent<NetworkObject>() != null)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }

    private void ApplyWeapon(Player player)
    {
        // Call player.SetWeapon directly from the server as requested
        player.SetWeapon(weaponType);
        player.SetWeaponClientRpc(weaponType);
    }

    public override void OnNetworkDespawn()
    {
        if (IsServer && spawner != null && GetComponent<NetworkObject>() != null)
        {
            spawner.OnPickupDespawned(GetComponent<NetworkObject>());
        }
        base.OnNetworkDespawn();
    }
}
