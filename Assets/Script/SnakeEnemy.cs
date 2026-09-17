using UnityEngine;

public class SnakeEnemy : MonoBehaviour
{
    public float speed = 1.5f;
    public int direction = 1;
    public int attackDamage = 1;

    private Rigidbody2D rb;
    private Collider2D snakeCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        snakeCollider = GetComponent<Collider2D>();
    }

    void FixedUpdate()
    {
        rb.linearVelocity = new Vector2(
            direction * speed,
            rb.linearVelocity.y
        );

        CheckEdge();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            if (Mathf.Abs(collision.contacts[0].normal.x) > 0.5f)
            {
                direction *= -1;
                Flip();
            }
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }
        }
    }

    void CheckEdge()
    {
        float x = snakeCollider.bounds.center.x +
                  direction * snakeCollider.bounds.extents.x * 0.7f;

        float y = snakeCollider.bounds.min.y + 0.1f;

        Vector2 checkPosition = new Vector2(x, y);

        RaycastHit2D hit = Physics2D.Raycast(
            checkPosition,
            Vector2.down,
            0.5f
        );

        if (hit.collider == null || !hit.collider.CompareTag("Ground"))
        {
            direction *= -1;
            Flip();
        }
    }

    void Flip()
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}