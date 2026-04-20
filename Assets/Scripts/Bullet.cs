using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Settings")]

    [SerializeField, Tooltip("Time before the bullet is destroyed")]
    private float lifeTime = 3f;

    [Header("Bomb Settings")]
    public bool isBomb = false;
    [SerializeField] private int bombBulletCount = 6;
    [SerializeField] private float childLifeMultiplier = 0.5f;
    [SerializeField] private GameObject normalBulletPrefab;

    [Header("Owner")]

    [Tooltip("Reference to the player that fired this bullet")]
    public Player owner;

    public bool useLifeTime = true;
    public bool canBounce = false;

    public int maxBounce = 1;
    private int bounceCount = 0;
    private Rigidbody2D rb;

    public void SetNormalBulletPrefab(GameObject prefab)
    {
        normalBulletPrefab = prefab;
    }

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
            Player p = collision.gameObject.GetComponent<Player>();

            if (p != null)
            {
                p.Die();
            }

            DestroyBullet();
        }

        if (collision.gameObject.CompareTag("Shield"))
        {
            DestroyBullet();
            return;
        }
    }

    void DestroyBullet()
    {
        if (isBomb)
        {
            ExplodeBomb();
        }

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

    void ExplodeBomb()
    {
        float angleStep = 360f / bombBulletCount;

        for (int i = 0; i < bombBulletCount; i++)
        {
            float angle = i * angleStep;

            Vector2 dir = Quaternion.Euler(0, 0, angle) * Vector2.right;

            GameObject bullet = Instantiate(normalBulletPrefab, transform.position, Quaternion.identity);

            TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
            if (trail != null)
            {
                trail.enabled = false;
            }

            Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
            rbBullet.linearVelocity = dir.normalized * rb.linearVelocity.magnitude;

            Bullet b = bullet.GetComponent<Bullet>();

            if (b != null)
            {
                b.owner = owner;
                b.useLifeTime = true;

                b.SetLifeTimeMultiplier(childLifeMultiplier);
            }
        }
    }

    public void SetLifeTimeMultiplier(float multiplier)
    {
        CancelInvoke();

        float newLife = lifeTime * multiplier;
        Invoke(nameof(DestroyBullet), newLife);
    }
}