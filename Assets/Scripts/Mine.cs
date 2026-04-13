using UnityEngine;
using System.Collections;

public class Mine : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float armDelay = 1f;
    [SerializeField] private float visibleTimeAfterArm = 3f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private float explosionRadius = 2f;

    [Header("Visual")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float fadeDuration = 0.5f;

    private bool isArmed = false;
    private Player owner;

    public void SetOwner(Player p)
    {
        owner = p;
    }

    void Start()
    {
        StartCoroutine(MineLifecycle());
    }

    IEnumerator MineLifecycle()
    {
        yield return new WaitForSeconds(armDelay);
        isArmed = true;

        yield return new WaitForSeconds(visibleTimeAfterArm);

        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0f;
        Color startColor = spriteRenderer.color;

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

            yield return null;
        }

        spriteRenderer.color = new Color(startColor.r, startColor.g, startColor.b, 0f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed) return;

        if (other.CompareTag("Player"))
        {
            Explode();
        }
    }

    void Explode()
    {

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                Player p = hit.GetComponent<Player>();

                Instantiate(explosionEffect, transform.position, Quaternion.identity);

                if (p != null)
                {
                    p.Die();
                }
            }
        }

        Destroy(gameObject);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}