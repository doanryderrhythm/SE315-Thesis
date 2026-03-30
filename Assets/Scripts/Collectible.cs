using UnityEngine;

public class Collectible : MonoBehaviour
{
    public WeaponType weaponType;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Player player = other.GetComponent<Player>();

        if (player != null)
        {
            if (weaponType == WeaponType.Shield)
            {
                player.ActivateShield();
            }
            else
            {
                player.SetWeapon(weaponType);
            }

            Destroy(gameObject);
        }
    }
}