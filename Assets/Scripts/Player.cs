using UnityEngine;
using UnityEngine.InputSystem;

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
    private PlayerControls controls;
    private Vector2 moveInput;

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
        
    }


    void FixedUpdate()
    {
        HandleMovement();
    }

    void Awake()
    {
        controls = new PlayerControls();
    }

    void OnEnable()
    {
        controls.Enable();

        controls.Player.Move.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        controls.Player.Move.canceled += ctx => moveInput = Vector2.zero;

        controls.Player.Attack.performed += ctx =>
        {
            if (currentAmmo > 0)
                Shoot();
        };
    }

    void OnDisable()
    {
        controls.Disable();
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