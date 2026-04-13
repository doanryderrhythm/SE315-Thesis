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

    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    void Start()
    {
        spawnPosition = transform.position;
        spawnRotation = transform.rotation;

        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public void Die()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        spriteRenderer.enabled = false;
        col.enabled = false;

        StartCoroutine(RespawnRoutine());
    }

    IEnumerator RespawnRoutine()
    {
        yield return new WaitForSeconds(respawnDelay);

        transform.position = spawnPosition;
        transform.rotation = spawnRotation;

        spriteRenderer.enabled = true;
        col.enabled = true;
    }
}