using UnityEngine;

public class Player : MonoBehaviour
{
    #region Movement Settings

    [Header("Movement Settings")]

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

    #endregion


    #region Ammo Settings

    [Header("Ammo Settings")]

    [SerializeField, Tooltip("Maximum bullets that player can shoot")]
    private int maxAmmo = 6;

    private int currentAmmo;

    #endregion


    #region Private References

    private Rigidbody2D rb;
    private float moveInput;
    private float rotateInput;

    #endregion


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Random starting direction
        float randomAngle = Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randomAngle);

        currentAmmo = maxAmmo;
    }


    void Update()
    {
        HandleInput();
        HandleShooting();
    }


    void FixedUpdate()
    {
        HandleMovement();
    }


    #region Input Handling

    void HandleInput()
    {
        moveInput = 0f;

        if (Input.GetKey(KeyCode.W))
            moveInput = 1f;
        else if (Input.GetKey(KeyCode.S))
            moveInput = -1f;


        rotateInput = 0f;

        if (Input.GetKey(KeyCode.A))
            rotateInput = 1f;
        else if (Input.GetKey(KeyCode.D))
            rotateInput = -1f;
    }

    #endregion


    #region Movement

    void HandleMovement()
    {
        rb.MoveRotation(rb.rotation + rotateInput * rotateSpeed * Time.fixedDeltaTime);

        Vector2 movement = transform.right * moveInput * moveSpeed;
        rb.linearVelocity = movement;
    }

    #endregion


    #region Shooting

    void HandleShooting()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentAmmo > 0)
        {
            Shoot();
        }
    }

    void Shoot()
    {
        currentAmmo--;

        GameObject bullet = Instantiate(bulletPrefab, firePoint.position, firePoint.rotation);

        Rigidbody2D rbBullet = bullet.GetComponent<Rigidbody2D>();

        if (rbBullet != null)
        {
            rbBullet.linearVelocity = firePoint.right * bulletSpeed;
        }

        Bullet bulletScript = bullet.GetComponent<Bullet>();
        if (bulletScript != null)
        {
            bulletScript.owner = this;
        }
    }

    #endregion


    #region Ammo System

    public void ReturnAmmo()
    {
        currentAmmo = Mathf.Min(currentAmmo + 1, maxAmmo);
    }

    #endregion
}