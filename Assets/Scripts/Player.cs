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

    [SerializeField] private float gatlingBulletSpeed = 10f;

    #endregion


    #region Ammo Settings

    [Header("Ammo Settings")]

    [SerializeField, Tooltip("Maximum bullets that player can shoot")]
    private int maxAmmo = 6;

    private int currentAmmo;

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
            if (currentWeapon == WeaponType.Gatling)
            {
                isFiring = true;
                StartCoroutine(GatlingFire());
            }
            else
            {
                Shoot();
            }
        }

        if (context.canceled)
        {
            isFiring = false;

            if (currentWeapon == WeaponType.Gatling)
            {
                currentWeapon = WeaponType.Normal;
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

    void SpawnBullet(Vector2 direction, float scale = 1f, float speedOverride = -1f)
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);

        bullet.transform.localScale *= scale;

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        float speed = (speedOverride > 0) ? speedOverride : bulletSpeed;

        rbBullet.linearVelocity = direction.normalized * speed;

        Bullet b = bullet.GetComponent<Bullet>();
        if (b != null)
        {
            b.owner = this;
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

    #endregion
}