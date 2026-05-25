using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class Dummy : NetworkBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

    [Header("Movement")]
    [SerializeField] private bool canMove = true;
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float rotateSpeed = 200f;

    [SerializeField] private Transform pointA;
    [SerializeField] private Transform pointB;

    private Transform currentTarget;

    private Vector3 spawnPosition;
    private Quaternion spawnRotation;

    private Renderer[] renderers;
    private Collider2D[] colliders;
    private Rigidbody2D rb;

    private bool isDead = false;

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        renderers = GetComponentsInChildren<Renderer>();
        colliders = GetComponentsInChildren<Collider2D>();
        rb = GetComponent<Rigidbody2D>();

        if (canMove && pointA != null && pointB != null)
        {
            currentTarget = pointB;
        }
    }

    void Update()
    {
        if (!canMove || isDead) return;

        HandleMovement();
    }

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

            Destroy(effect, 2f);
        }
    }

    public void Die()
    {
        if (!IsServer) return;

        if (isDead) return;

        isDead = true;

        if (explosionEffect != null)
        {
            PlayExplosionClientRpc(transform.position);
        }

        foreach (var r in renderers)
        {
            r.enabled = false;
        }

        foreach (var c in colliders)
        {
            c.enabled = false;
        }

        if (rb != null)
        {
            rb.simulated = false;
        }

        StartCoroutine(RespawnRoutine());
    }

    void HandleMovement()
    {
        Vector2 direction = (currentTarget.position - transform.position).normalized;

        float targetAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        float angle = Mathf.MoveTowardsAngle(transform.eulerAngles.z, targetAngle, rotateSpeed * Time.deltaTime);
        transform.rotation = Quaternion.Euler(0, 0, angle);

        float angleDiff = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.z, targetAngle));

        if (angleDiff < 5f)
        {
            transform.position += transform.right * moveSpeed * Time.deltaTime;
        }

        float distance = Vector2.Distance(transform.position, currentTarget.position);

        if (distance < 0.2f)
        {
            currentTarget = (currentTarget == pointA) ? pointB : pointA;
        }
    }

    [Rpc(SendTo.ClientsAndHost)]
    void RespawnClientRpc()
    {
        foreach (var r in renderers)
        {
            r.enabled = false;
        }

        foreach (var c in colliders)
        {
            c.enabled = false;
        }


        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        RespawnClientRpc();

        isDead = false;
    }
}