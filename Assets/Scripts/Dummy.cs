using UnityEngine;
using System.Collections;

public class Dummy : MonoBehaviour
{
    [Header("Respawn Settings")]
    [SerializeField] private float respawnDelay = 3f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

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
    }

    public void Die()
    {
        if (isDead) return;

        isDead = true;

        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        foreach (var r in renderers)
            r.enabled = false;

        foreach (var c in colliders)
            c.enabled = false;

        if (rb != null)
            rb.simulated = false;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        foreach (var r in renderers)
            r.enabled = true;

        foreach (var c in colliders)
            c.enabled = true;

        if (rb != null)
        {
            rb.simulated = true;
            rb.linearVelocity = Vector2.zero;
        }

        isDead = false;
    }
}