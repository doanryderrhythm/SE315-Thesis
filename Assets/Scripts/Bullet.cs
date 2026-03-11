using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]

    [SerializeField, Tooltip("Time before the bullet is destroyed")]
    private float lifeTime = 3f;


    [Header("Owner")]

    [Tooltip("Reference to the player that fired this bullet")]
    public Player owner;


    void Start()
    {
        Invoke(nameof(DestroyBullet), lifeTime);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        DestroyBullet();
    }


    void DestroyBullet()
    {
        if (owner != null)
        {
            owner.ReturnAmmo();
        }

        Destroy(gameObject);
    }
}