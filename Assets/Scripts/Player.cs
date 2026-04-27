using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    #region Player Settings

    [Header("Player Settings")]

    [SerializeField, Tooltip("Speed of the tank movement")]
    private float moveSpeed = 5f;

    [SerializeField, Tooltip("Speed of the tank rotation")]
    private float rotateSpeed = 200f;

    #endregion


    #region Shooting Settings

    [Header("Shooting Settings")]

    [SerializeField, Tooltip("Bullet prefab that will be spawned")]
    private GameObject bulletPrefab;

    [SerializeField, Tooltip("Spawn position of the bullet")]
    private Transform firePoint;

    [SerializeField, Tooltip("Speed of the bullet")]
    private float bulletSpeed = 10f;

    [SerializeField, Tooltip("Bomb lifetime")]
    private float bombLifeTime = 5f;

    [SerializeField] private float gatlingBulletSpeed = 10f;

    private Coroutine cookingBombRoutine;
    private float currentCookTime = 0f;

    [Header("Bomb Audio")]
    [SerializeField] private AudioSource cookTickAudio;

    private float nextCookTickTime = 0f;

    #endregion


    #region Ammo Settings

    [Header("Ammo Settings")]

    [SerializeField, Tooltip("Maximum bullets that player can shoot")]
    private int maxAmmo = 6;

    private int currentAmmo;

    private float bombHoldStartTime;
    private bool isCookingBomb = false;

    #endregion

    #region Weapon Settings

    [SerializeField] private GameObject normalBulletPrefab;

    [SerializeField] private GameObject shieldObject;
    [SerializeField] private float shieldDuration = 15f;

    private Coroutine shieldCoroutine;

    [SerializeField] private GameObject minePrefab;
    
    [SerializeField] private GameObject bombBulletPrefab;

    #endregion

    #region Effects Setting

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

    #endregion


    #region Private References

    private Rigidbody2D rb;
    private float rotateInput;
    private Vector2 moveInput;
    private WeaponType currentWeapon = WeaponType.Normal;
    private bool isFiring = false;

    #endregion


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randomAngle);

        currentAmmo = maxAmmo;
    }

    void FixedUpdate()
    {
        HandleMovement();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (currentWeapon == WeaponType.Bomb)
            {
                isCookingBomb = true;
                currentCookTime = 0f;
                nextCookTickTime = 0f;

                if (cookingBombRoutine != null)
                    StopCoroutine(cookingBombRoutine);

                cookingBombRoutine = StartCoroutine(CookBombRoutine());
                return;
            }

            if (currentWeapon == WeaponType.Gatling)
            {
                isFiring = true;
                StartCoroutine(GatlingFire());
                return;
            }

            Shoot();
            return;
        }

        if (context.canceled)
        {
            if (currentWeapon == WeaponType.Bomb && isCookingBomb)
            {
                isCookingBomb = false;

                if (cookingBombRoutine != null)
                    StopCoroutine(cookingBombRoutine);

                ShootBomb(currentCookTime);

                currentWeapon = WeaponType.Normal;
                return;
            }

            if (currentWeapon == WeaponType.Gatling)
            {
                isFiring = false;
                currentWeapon = WeaponType.Normal;
                return;
            }
        }
    }

    #region Movement

    void HandleMovement()
    {
        float forward = moveInput.y;
        float rotate = -moveInput.x;

        rb.MoveRotation(rb.rotation + rotate * rotateSpeed * Time.fixedDeltaTime);

        Vector2 movement = transform.right * forward * moveSpeed;
        rb.linearVelocity = movement;
    }

    #endregion

    #region Effects
    public void Die()
    {
        Instantiate(explosionEffect, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }

    #endregion


    #region Shooting

    void Shoot()
    {
        if (currentAmmo <= 0) return;

        currentAmmo--;

        switch (currentWeapon)
        {
            case WeaponType.Normal:
                ShootNormal();
                break;

            case WeaponType.Laser:
                ShootLaser();
                currentWeapon = WeaponType.Normal;
                break;

            case WeaponType.Mine:
                PlaceMine();
                currentWeapon = WeaponType.Normal;
                break;

            case WeaponType.Bomb:
                break;
        }
    }

    IEnumerator GatlingFire()
    {
        int bulletCount = 0;

        while (isFiring && bulletCount < 15)
        {
            bulletCount++;

            float spread = Random.Range(-15f, 15f);
            Vector2 dir = Quaternion.Euler(0, 0, spread) * firePoint.right;

            SpawnBullet(dir, 0.6f, gatlingBulletSpeed);

            yield return new WaitForSeconds(0.05f);
        }

        currentWeapon = WeaponType.Normal;
    }

    void ShootNormal()
    {
        SpawnBullet(firePoint.right);
    }

    void ShootLaser()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = true;
        }

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        rbBullet.linearVelocity = firePoint.right * bulletSpeed * 5f;

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.owner = this;
            b.maxBounce = 10;
            b.useLifeTime = false;
            b.canBounce = true;
        }
    }

    void ShootBomb(float cookedTime)
    {
        float remainingTime = Mathf.Max(0f, bombLifeTime - cookedTime);

        GameObject bullet = Instantiate(
            bombBulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();
        if (rbBullet != null)
        {
            rbBullet.linearVelocity = firePoint.right * bulletSpeed;
        }

        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = false;
        }

        Bullet bomb = bullet.GetComponent<Bullet>();
        if (bomb != null)
        {
            bomb.owner = this;
            bomb.isBomb = true;
            bomb.SetNormalBulletPrefab(normalBulletPrefab);

            bomb.StartBombTimer(remainingTime);
        }
    }

    void SpawnBullet(Vector2 direction, float scale = 1f, float speedOverride = -1f)
    {
        GameObject bullet = Instantiate(normalBulletPrefab, firePoint.position, Quaternion.identity);

        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        if (trail != null)
        {
            trail.enabled = false;
        }

        bullet.transform.localScale *= scale;

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        float speed = (speedOverride > 0) ? speedOverride : bulletSpeed;

        rbBullet.linearVelocity = direction.normalized * speed;

        Collider2D bulletCol = bullet.GetComponent<Collider2D>();
        Collider2D playerCol = GetComponent<Collider2D>();

        if (bulletCol != null && playerCol != null)
        {
            StartCoroutine(EnableCollisionAfterDelay(bulletCol, playerCol, 0.1f));
        }

        if (shieldObject != null && shieldObject.activeSelf)
        {
            Collider2D shieldCol = shieldObject.GetComponent<Collider2D>();

            if (bulletCol != null && shieldCol != null)
            {
                StartCoroutine(EnableCollisionAfterDelay(bulletCol, playerCol, 0.1f));
            }
        }

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.owner = this;
        }
    }

    IEnumerator EnableCollisionAfterDelay(Collider2D bullet, Collider2D player, float delay)
    {
        Physics2D.IgnoreCollision(bullet, player, true);

        yield return new WaitForSeconds(delay);

        if (bullet != null && player != null)
        {
            Physics2D.IgnoreCollision(bullet, player, false);
        }
    }

    void PlaceMine()
    {
        GameObject mine = Instantiate(minePrefab, transform.position, Quaternion.identity);

        Mine m = mine.GetComponent<Mine>();
        if (m != null)
        {
            m.SetOwner(this);
        }
    }

    #endregion


    #region Ammo System

    public void ReturnAmmo()
    {
        currentAmmo = Mathf.Min(currentAmmo + 1, maxAmmo);
    }

    #endregion

    #region Weapon System

    public void SetWeapon(WeaponType newWeapon)
    {
        currentWeapon = newWeapon;
    }

    public void ActivateShield()
    {
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }

        shieldCoroutine = StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        shieldObject.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        shieldObject.SetActive(false);
    }

    #endregion

    IEnumerator CookBombRoutine()
    {
        currentCookTime = 0f;

        while (isCookingBomb)
        {
            currentCookTime += Time.deltaTime;

            float remainingTime = bombLifeTime - currentCookTime;

            if (remainingTime <= 0f)
            {
                GameObject tempBomb = Instantiate(
                    bombBulletPrefab,
                    transform.position,
                    Quaternion.identity
                );

                Bullet b = tempBomb.GetComponent<Bullet>();
                if (b != null)
                {
                    b.owner = this;
                    b.isBomb = true;
                    b.SetNormalBulletPrefab(normalBulletPrefab);

                    b.ForceExplode();
                }

                Die();
                currentWeapon = WeaponType.Normal;
                isCookingBomb = false;
                yield break;
            }

            if (remainingTime <= 3f)
            {
                float tickInterval = Mathf.Lerp(
                    0.35f,   
                    0.06f,   
                    1f - (remainingTime / 3f)
                );

                if (Time.time >= nextCookTickTime)
                {
                    if (cookTickAudio != null)
                    {
                        cookTickAudio.PlayOneShot(cookTickAudio.clip);
                    }

                    nextCookTickTime = Time.time + tickInterval;
                }
            }

            yield return null;
        }
    }
}