using UnityEngine;

public class Collectible : MonoBehaviour
{
    [SerializeField] private WeaponType weaponType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player == null) return;

        ApplyEffect(player);
        Destroy(gameObject);
    }

    void ApplyEffect(Player player)
    {
        switch (weaponType)
        {
            case WeaponType.Shield:
                player.ActivateShield();
                break;

            case WeaponType.Mine:
            case WeaponType.Gatling:
            case WeaponType.Laser:
            case WeaponType.Bomb:   
            case WeaponType.Normal:
                player.SetWeapon(weaponType);
                break;

            default:
                Debug.LogWarning("Unknown weapon type: " + weaponType);
                break;
        }
    }
}