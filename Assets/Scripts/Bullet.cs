using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]

    [SerializeField, Tooltip("Time before the bullet is destroyed")]
    private float lifeTime = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

    [Header("Owner")]

    [Tooltip("Reference to the player that fired this bullet")]
    public Player owner;

    public bool useLifeTime = true;
    public bool canBounce = false;

    public int maxBounce = 1;
    private int bounceCount = 0;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        if (useLifeTime)
        {
            Invoke(nameof(DestroyBullet), lifeTime);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            if (!canBounce)
            {
                DestroyBullet();
                return;
            }

            bounceCount++;

            if (bounceCount > maxBounce)
            {
                DestroyBullet();
                return;
            }

            Vector2 normal = collision.contacts[0].normal;
            Vector2 reflect = Vector2.Reflect(rb.linearVelocity, normal);

            rb.linearVelocity = reflect.normalized * rb.linearVelocity.magnitude;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            Explode(collision.transform.position);

            Destroy(collision.gameObject);
            DestroyBullet();
        }
    }

    void Explode(Vector3 pos)
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, pos, Quaternion.identity);
            Destroy(gameObject, 1f);
        }
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
        if (rb.linearVelocity.sqrMagnitude > 0.01f)
        {
            float angle = Mathf.Atan2(rb.linearVelocity.y, rb.linearVelocity.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}