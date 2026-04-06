using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
    [Header("Mine Settings")]
    [SerializeField] private float armDelay = 1f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;

    private bool isArmed = false;
    private Player owner;

    public void SetOwner(Player p)
    {
        owner = p;
    }

    void Start()
    {
        StartCoroutine(ArmRoutine());
    }

    IEnumerator ArmRoutine()
    {
        yield return new WaitForSeconds(armDelay);
        isArmed = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed) return;

        if (other.CompareTag("Player"))
        {
            Explode(other.gameObject);
        }
    }

    void Explode(GameObject target)
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Destroy(target);
        Destroy(gameObject);
    }
}