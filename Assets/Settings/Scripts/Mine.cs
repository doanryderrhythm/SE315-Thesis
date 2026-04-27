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
    [SerializeField] private GameObject rangeIndicator;

    private SpriteRenderer[] allRenderers;

    private bool isArmed = false;
    private Player owner;

    public void SetOwner(Player p)
    {
        owner = p;
    }

    void Start()
    {
        allRenderers = GetComponentsInChildren<SpriteRenderer>();

        if (rangeIndicator != null)
            rangeIndicator.SetActive(true);

        StartCoroutine(MineLifecycle());
    }

    IEnumerator MineLifecycle()
    {
        yield return new WaitForSeconds(armDelay);
        isArmed = true;

        yield return new WaitForSeconds(visibleTimeAfterArm);

        StartCoroutine(FadeRange());

        yield return StartCoroutine(FadeOut());
    }

    IEnumerator FadeOut()
    {
        float time = 0f;

        Color[] startColors = new Color[allRenderers.Length];
        for (int i = 0; i < allRenderers.Length; i++)
        {
            startColors[i] = allRenderers[i].color;
        }

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            float alpha = Mathf.Lerp(1f, 0f, time / fadeDuration);

            for (int i = 0; i < allRenderers.Length; i++)
            {
                var c = startColors[i];
                allRenderers[i].color = new Color(c.r, c.g, c.b, alpha);
            }

            yield return null;
        }

        foreach (var r in allRenderers)
        {
            var c = r.color;
            r.color = new Color(c.r, c.g, c.b, 0f);
        }
    }

    IEnumerator FadeRange()
    {
        if (rangeIndicator == null) yield break;

        SpriteRenderer sr = rangeIndicator.GetComponent<SpriteRenderer>();
        if (sr == null) yield break;

        Color startColor = sr.color;
        float t = 0f;

        while (t < fadeDuration)
        {
            t += Time.deltaTime;
            float alpha = Mathf.Lerp(startColor.a, 0f, t / fadeDuration);

            sr.color = new Color(startColor.r, startColor.g, startColor.b, alpha);
            yield return null;
        }

        rangeIndicator.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isArmed) return;

        if (other.TryGetComponent<Player>(out var p))
        {
            Explode();
            return;
        }

        if (other.TryGetComponent<Dummy>(out var d))
        {
            Explode();
            return;
        }
    }

    void Explode()
    {
        if (explosionEffect != null)
        {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, explosionRadius);

        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Player>(out var p))
            {
                p.Die();
            }

            if (hit.TryGetComponent<Dummy>(out var d))
            {
                d.Die();
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