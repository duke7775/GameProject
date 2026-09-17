using UnityEngine;

public class MonkeyEnemy : MonoBehaviour
{
    public float speed = 1.5f;
    public float chaseRange = 5f;
    public float attackRange = 2f;

    public int attackDamage = 2;

    private int direction = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Collider2D monkeyCollider;

    private bool isAttacking = false;
    private bool hasHitPlayer = false;

    private float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    private float hitCooldown = 0f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        monkeyCollider = GetComponent<Collider2D>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        if (hitCooldown > 0)
        {
            hitCooldown -= Time.fixedDeltaTime;
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        float distance = Mathf.Abs(
            player.position.x - transform.position.x
        );

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            FacePlayer();

            if (!isAttacking && attackTimer <= 0 && hitCooldown <= 0)
            {
                StartAttack();
            }

            return;
        }

        if (distance <= chaseRange)
        {
            ChasePlayer();
            return;
        }

        Patrol();
    }

    void StartAttack()
    {
        isAttacking = true;
        hasHitPlayer = false;
        attackTimer = attackCooldown;

        animator.Play("monkey2", 0, 0f);

        Invoke(nameof(FinishAttack), 1f);
    }

    public void AttackPlayer()
    {
        if (!isAttacking || hasHitPlayer)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        float distance = Mathf.Abs(
            player.position.x - transform.position.x
        );

        if (distance <= attackRange)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
                hasHitPlayer = true;
            }
        }
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(
            direction * speed,
            rb.linearVelocity.y
        );

        animator.SetFloat("Speed", speed);

        FaceDirection();

        CheckEdge();
    }

    void ChasePlayer()
    {
        float playerDirection = Mathf.Sign(
            player.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(
            playerDirection * speed,
            rb.linearVelocity.y
        );

        animator.SetFloat(
            "Speed",
            Mathf.Abs(rb.linearVelocity.x)
        );

        FacePlayer();
    }

    void CheckEdge()
    {
        float x = monkeyCollider.bounds.center.x +
                  direction * monkeyCollider.bounds.extents.x;

        float y = monkeyCollider.bounds.min.y + 0.05f;

        Vector2 checkPosition = new Vector2(x, y);

        RaycastHit2D hit = Physics2D.Raycast(
            checkPosition,
            Vector2.down,
            0.3f
        );

        if (hit.collider == null || !hit.collider.CompareTag("Ground"))
        {
            direction *= -1;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<MonkeyEnemy>() != null)
        {
            direction *= -1;
        }
    }

    void FinishAttack()
    {
        isAttacking = false;
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            direction = 1;
        }
        else
        {
            direction = -1;
        }

        FaceDirection();
    }

    void FaceDirection()
    {
        transform.localScale = new Vector3(
            direction * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}