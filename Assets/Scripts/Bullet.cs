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
        
    }

    void DestroyBullet()
    {
        if (owner != null)
        {
            owner.ReturnAmmo();
        }

        Destroy(gameObject);
    }

    void Update()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}