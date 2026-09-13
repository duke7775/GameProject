using UnityEngine;

public class BoarBoss : MonoBehaviour
{
    public int maxHealth = 100;
    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;
    public float detectRange = 6f;
    public float attackRange = 2f;
    public float attackCooldown = 1.2f;
    public float chargeTime = 1.5f;

    private int currentHealth;
    private int phase = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private bool isAttacking = false;
    private bool isCharging = false;

    private float attackTimer = 0f;
    private float chargeTimer = 0f;

    private int chargeDirection = 1;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("speed", 0);
            return;
        }

        if (phase == 2)
        {
            Phase2Behavior();
            return;
        }

        Phase1Behavior();
    }

    void Phase1Behavior()
    {
        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance > detectRange)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat("speed", 0);
            return;
        }

        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat("speed", 0);

            FacePlayer();

            if (!isAttacking && attackTimer <= 0)
            {
                isAttacking = true;
                attackTimer = attackCooldown;

                animator.SetTrigger("attack");

                Invoke(nameof(FinishAttack), 0.8f);
            }

            return;
        }

        float direction = Mathf.Sign(
            player.position.x - transform.position.x
        );

        rb.linearVelocity = new Vector2(
            direction * walkSpeed,
            rb.linearVelocity.y
        );

        animator.SetFloat("speed", walkSpeed);

        FacePlayer();
    }

    void Phase2Behavior()
    {
        if (isCharging)
        {
            chargeTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                chargeDirection * runSpeed,
                rb.linearVelocity.y
            );

            animator.SetFloat("speed", runSpeed);

            if (chargeTimer <= 0)
            {
                isCharging = false;

                rb.linearVelocity = new Vector2(
                    0,
                    rb.linearVelocity.y
                );

                animator.SetFloat("speed", 0);
            }

            return;
        }

        float distance = Vector2.Distance(
            transform.position,
            player.position
        );

        if (distance <= detectRange)
        {
            StartCharge();
        }
        else
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat("speed", 0);
        }
    }

    void StartCharge()
    {
        isCharging = true;
        chargeTimer = chargeTime;

        if (player.position.x > transform.position.x)
        {
            chargeDirection = 1;
        }
        else
        {
            chargeDirection = -1;
        }

        FaceDirection(chargeDirection);

        animator.SetFloat("speed", runSpeed);
    }

    void FinishAttack()
    {
        isAttacking = false;
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            FaceDirection(1);
        }
        else
        {
            FaceDirection(-1);
        }
    }

    void FaceDirection(int direction)
    {
        transform.localScale = new Vector3(
            direction * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log("Boss HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        if (currentHealth <= maxHealth / 2 && phase == 1)
        {
            phase = 2;

            Debug.Log("Boss Phase 2!");

            isAttacking = false;
            isCharging = false;
        }
    }

    void Die()
    {
        rb.linearVelocity = Vector2.zero;

        animator.SetFloat("speed", 0);
        animator.SetTrigger("dead");

        enabled = false;
    }
}