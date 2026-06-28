using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Pun.UtilityScripts;
using Photon.Realtime;
using System;
using System.Collections;
using System.Globalization;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : NetworkBehaviour
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

    [SerializeField] private GameObject laserBulletPrefab;

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
    private bool serverIsFiring = false;
    private PlayerInput playerInput;
    private Vector2 networkMoveInput;

    #endregion

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(
        false,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );


    public override void OnNetworkSpawn()
    {
        rb = GetComponent<Rigidbody2D>();
        playerInput = GetComponent<PlayerInput>();

        if (IsOwner)
        {
            if (PauseMenu.Instance != null)
            {
                PauseMenu.Instance.RegisterPlayerInput(playerInput);
            }
            else
            {
                Debug.LogWarning("PauseMenu.Instance is NULL");
            }
        }

        if (!IsOwner)
        {
            playerInput.enabled = false;
        }

        float randomAngle = UnityEngine.Random.Range(0f, 360f);
        transform.rotation = Quaternion.Euler(0, 0, randomAngle);

        currentAmmo = maxAmmo;

        isDead.OnValueChanged += OnDeathStateChanged;
        OnDeathStateChanged(false, isDead.Value);
    }

    public override void OnNetworkDespawn()
    {
        isDead.OnValueChanged -= OnDeathStateChanged;
    }

    private void OnDeathStateChanged(bool oldVal, bool newVal)
    {
        // newVal is true if dead, false if alive

        // Enable/disable visuals
        var renderers = GetComponentsInChildren<Renderer>(true);
        foreach (var r in renderers)
        {
            if (shieldObject != null && r.gameObject == shieldObject) continue;
            r.enabled = !newVal;
        }

        // Enable/disable collisions
        var colliders = GetComponentsInChildren<Collider2D>(true);
        foreach (var c in colliders)
        {
            c.enabled = !newVal;
        }

        // Enable/disable player input and physics simulation
        if (playerInput != null)
        {
            playerInput.enabled = IsOwner && !newVal;
        }

        if (rb != null)
        {
            rb.simulated = !newVal;
        }
    }

    void FixedUpdate()
    {
        if (IsOwner)
        {
            MoveServerRpc(moveInput);
        }

        if (IsServer)
        {
            HandleMovement(networkMoveInput);
        }
    }

    [Rpc(SendTo.Server)]
    void MoveServerRpc(Vector2 input)
    {
        networkMoveInput = input;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!IsOwner) return;

        moveInput = context.ReadValue<Vector2>();

        if (context.started)
        {
            AudioManager.Instance.Sound.PlayLoop("tank_move");
        }
        
        if (context.canceled)
        {
            AudioManager.Instance.Sound.StopLoop();
        }
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
                serverIsFiring = true;
                StartGatlingServerRpc();
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


                ShootBombServerRpc(currentCookTime);

                currentWeapon = WeaponType.Normal;
                return;
            }

            if (currentWeapon == WeaponType.Gatling)
            {
                StopGatlingServerRpc();

                currentWeapon = WeaponType.Normal;

                return;
            }
        }
    }

    #region Movement

    void HandleMovement(Vector2 input)
    {
        float forward = input.y;
        float rotate = -input.x;

        rb.MoveRotation(
            rb.rotation + rotate * rotateSpeed * Time.fixedDeltaTime
        );

        Vector2 movement = transform.right * forward * moveSpeed;

        rb.linearVelocity = movement;
    }

    #endregion

    #region Effects

    [Rpc(SendTo.ClientsAndHost)]
    void PlayExplosionClientRpc(Vector3 pos)
    {
        if (explosionEffect != null)
        {
            GameObject effect = Instantiate(
                explosionEffect,
                pos,
                Quaternion.identity
            );

            PlaySoundClientRpc("explosion");

            Destroy(effect, 2f);
        }
    }

    public void Die()
    {
        if (!IsServer) return;

        PlayExplosionClientRpc(transform.position);

        isDead.Value = true;

        AddDeathClientRpc();
        ArenaManager.OnPlayerDead.Invoke();
    }

    #endregion


    #region Shooting

    void Shoot()
    {
        if (currentAmmo <= 0) return;

        switch (currentWeapon)
        {
            case WeaponType.Normal:
                ShootServerRpc();
                break;

            case WeaponType.Laser:
                ShootLaserServerRpc();
                SetWeapon(WeaponType.Normal);
                break;

            case WeaponType.Mine:
                PlaceMineServerRpc();
                SetWeapon(WeaponType.Normal);
                break;

            case WeaponType.Bomb:
                break;
        }
    }

    IEnumerator GatlingFire()
    {
        if (!IsServer) yield break;

        int bulletCount = 0;

        while (serverIsFiring && bulletCount < 15)
        {
            bulletCount++;

            float spread = UnityEngine.Random.Range(-15f, 15f);
            Vector2 dir = Quaternion.Euler(0, 0, spread) * firePoint.right;

            SpawnBullet(dir, 0.6f, gatlingBulletSpeed);
            float pitch = UnityEngine.Random.Range(0.9f, 1f);
            PlaySoundClientRpc("tank_shoot", pitch);

            yield return new WaitForSeconds(0.05f);
        }

        currentWeapon = WeaponType.Normal;
    }

    [Rpc(SendTo.Server)]
    void ShootServerRpc()
    {
        if (currentAmmo <= 0)
            return;

        currentAmmo--;

        SpawnBullet(firePoint.right);

        PlaySoundClientRpc("tank_shoot");
    }

    [Rpc(SendTo.Server)]
    void StartGatlingServerRpc()
    {
        serverIsFiring = true;

        StartCoroutine(GatlingFire());
    }

    [Rpc(SendTo.Server)]
    void StopGatlingServerRpc()
    {
        serverIsFiring = false;
    }

    [Rpc(SendTo.Server)]
    void ShootLaserServerRpc()
    {
        ShootLaser();
        PlaySoundClientRpc("tank_shoot", 2.5f);
    }

    [Rpc(SendTo.Owner)]
    public void SetWeaponClientRpc(WeaponType newWeapon)
    {
        Debug.Log($"[Player RPC] SetWeaponClientRpc received on Owner. IsServer={IsServer}, IsOwner={IsOwner}, newWeapon={newWeapon}, Instance={gameObject.name}");
        SetWeapon(newWeapon);
    }

    void ShootLaser()
    {
        GameObject bullet = Instantiate(laserBulletPrefab, firePoint.position, firePoint.rotation);

        bullet.GetComponent<NetworkObject>().Spawn();

        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();

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
        if (!IsServer) return;

        float remainingTime = Mathf.Max(0f, bombLifeTime - cookedTime);

        GameObject bullet = Instantiate(
            bombBulletPrefab,
            firePoint.position,
            firePoint.rotation
        );

        Bullet bomb = bullet.GetComponent<Bullet>();
        if (bomb != null)
        {
            bomb.owner = this;
            bomb.isBomb = true;
            bomb.SetNormalBulletPrefab(normalBulletPrefab);
        }

        NetworkObject netObj = bullet.GetComponent<NetworkObject>();
        if (netObj != null)
        {
            netObj.Spawn();
        }

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

        if (bomb != null)
        {
            bomb.StartBombTimer(remainingTime);
        }
    }

    void SpawnBullet(Vector2 direction, float scale = 1f, float speedOverride = -1f)
    {
        GameObject bullet = Instantiate(normalBulletPrefab, firePoint.position, Quaternion.identity);

        bullet.GetComponent<NetworkObject>().Spawn();

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

    [Rpc(SendTo.Server)]
    void PlaceMineServerRpc()
    {
        PlaceMine();
    }

    [Rpc(SendTo.Server)]
    void ShootBombServerRpc(float cookedTime)
    {
        ShootBomb(cookedTime);

        PlaySoundClientRpc("tank_shoot", 0.5f);
    }

    [Rpc(SendTo.Server)]
    void ExplodeBombInHandServerRpc()
    {
        GameObject tempBomb = Instantiate(
            bombBulletPrefab,
            transform.position,
            Quaternion.identity
        );

        NetworkObject tempBombNet = tempBomb.GetComponent<NetworkObject>();
        if (tempBombNet != null)
        {
            tempBombNet.Spawn();
        }

        Bullet b = tempBomb.GetComponent<Bullet>();
        if (b != null)
        {
            b.owner = this;
            b.isBomb = true;
            b.SetNormalBulletPrefab(normalBulletPrefab);
            b.ForceExplode();
        }

        PlaySoundClientRpc("explosion");
        Die();
        currentWeapon = WeaponType.Normal;
    }

    void PlaceMine()
    {
        GameObject mine = Instantiate(minePrefab, transform.position, Quaternion.identity);

        mine.GetComponent<NetworkObject>().Spawn();

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

        TankSkinLoader tankSkinLoader = GetComponent<TankSkinLoader>();
        if(tankSkinLoader != null)
        {
            tankSkinLoader.SetWeaponTypeServerRpc(newWeapon);
        }
    }

    public void ActivateShield()
    {
        //if (shieldCoroutine != null)
        //{
        //    StopCoroutine(shieldCoroutine);
        //}

        //shieldCoroutine = StartCoroutine(ShieldRoutine());
    }

    IEnumerator ShieldRoutine()
    {
        shieldObject.SetActive(true);

        yield return new WaitForSeconds(shieldDuration);

        shieldObject.SetActive(false);
    }

    public void ResetPlayerState()
    {
        if (!IsServer) return;

        isDead.Value = false;
        currentAmmo = maxAmmo;
        SetWeapon(WeaponType.Normal);
        SetWeaponClientRpc(WeaponType.Normal);
        isCookingBomb = false;
        serverIsFiring = false;
        currentCookTime = 0f;

        if (cookingBombRoutine != null)
        {
            StopCoroutine(cookingBombRoutine);
            cookingBombRoutine = null;
        }

        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        }

        if (shieldObject != null)
        {
            shieldObject.SetActive(false);
        }
    }

    #endregion

    #region ONLINE STATISTICS
    [Rpc(SendTo.Owner)]
    public void AddKillClientRpc()
    {
        Photon.Realtime.Player localP = PhotonNetwork.LocalPlayer;
        if (localP == null) return;

        int kills = localP.CustomProperties.ContainsKey("Kills")
            ? (int)localP.CustomProperties["Kills"] : 0;

        localP.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "Kills", kills + 1 }
        });

        localP.AddScore(30);
    }

    [Rpc(SendTo.Owner)]
    public void AddDeathClientRpc()
    {
        Photon.Realtime.Player localP = PhotonNetwork.LocalPlayer;
        if (localP == null) return;

        int deaths = localP.CustomProperties.ContainsKey("Deaths")
            ? (int)localP.CustomProperties["Deaths"] : 0;

        localP.SetCustomProperties(new ExitGames.Client.Photon.Hashtable
        {
            { "Deaths", deaths + 1 }
        });
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
                ExplodeBombInHandServerRpc();

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
                    PlaySoundClientRpc("bomb_tick");

                    nextCookTickTime = Time.time + tickInterval;
                }
            }

            yield return null;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    private void PlaySoundClientRpc(string soundName, float pitch = 1f)
    {
        AudioManager.Instance.Sound.PlaySound2D(soundName, pitch);
    }
}