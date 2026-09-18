using UnityEngine;
using System.Collections;

public class BossBombWarning : MonoBehaviour
{
    [Header("Warning")]
    public float warningTime = 3.5f;
    public float blinkStartTime = 1.5f;
    public float blinkInterval = 0.2f;

    [Header("Explosion")]
    public float explosionTime = 0.6f;
    public float explosionRadius = 1.2f;
    public int damage = 1;

    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private bool exploded = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (animator != null)
        {
            animator.enabled = false;
        }

        spriteRenderer.enabled = true;

        StartCoroutine(WarningRoutine());
    }

    IEnumerator WarningRoutine()
    {
        float timer = 0f;

        while (timer < warningTime)
        {
            if (timer >= warningTime - blinkStartTime)
            {
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(blinkInterval);

                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(blinkInterval);

                timer += blinkInterval * 2f;
            }
            else
            {
                yield return null;
                timer += Time.deltaTime;
            }
        }

        spriteRenderer.enabled = true;

        Explode();

        yield return new WaitForSeconds(explosionTime);

        Destroy(gameObject);
    }

    void Explode()
    {
        if (exploded)
            return;

        exploded = true;

        if (animator != null)
        {
            animator.enabled = true;
            animator.SetTrigger("Explosion");
        }

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            explosionRadius
        );

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth playerHealth =
                    hit.GetComponent<PlayerHealth>();

                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                }
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawWireSphere(
            transform.position,
            explosionRadius
        );
    }
}