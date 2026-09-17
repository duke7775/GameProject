using UnityEngine;

public class Boss0 : MonoBehaviour
{
    public int maxHealth = 50;

    public float walkSpeed = 1.5f;

    public float detectRange = 8f;
    public float attackRange = 2.5f;
    public float attackHeight = 1.5f;
    public float attackCooldown = 1.2f;

    public int attackDamage = 2;

    public float chargeSpeed = 5f;
    public float chargeTime = 1.5f;
    public float restTime = 1.5f;
    public int chargeDamage = 4;

    private int currentHealth;
    private int phase = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Collider2D bossCollider;
    private Collider2D playerCollider;

    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isResting = false;
    private bool isDead = false;
    private bool hasHitPlayer = false;

    private float attackTimer = 0f;
    private float chargeTimer = 0f;
    private float restTimer = 0f;

    private int direction = 1;

    void Start()
    {
        currentHealth = maxHealth;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        bossCollider = GetComponent<Collider2D>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
            playerCollider = playerObject.GetComponent<Collider2D>();
        }

        direction = transform.localScale.x >= 0 ? 1 : -1;
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (currentHealth <= 20 && phase == 1)
        {
            EnterPhase2();
            return;
        }

        if (phase == 2)
        {
            Phase2();
            return;
        }

        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            return;
        }

        if (player == null || playerCollider == null)
        {
            StopMoving();
            return;
        }

        float xDistance = Mathf.Abs(
            playerCollider.bounds.center.x -
            bossCollider.bounds.center.x
        );

        float yDistance = Mathf.Abs(
            playerCollider.bounds.center.y -
            bossCollider.bounds.center.y
        );

        if (xDistance <= detectRange)
        {
            direction =
                playerCollider.bounds.center.x >
                bossCollider.bounds.center.x ? 1 : -1;

            FaceDirection();

            if (xDistance <= attackRange &&
                yDistance <= attackHeight)
            {
                StopMoving();

                if (attackTimer <= 0)
                {
                    StartAttack();
                }

                return;
            }

            rb.linearVelocity = new Vector2(
                direction * walkSpeed,
                rb.linearVelocity.y
            );

            animator.SetFloat("Speed", walkSpeed);

            return;
        }

        StopMoving();
    }

    void StartAttack()
    {
        isAttacking = true;
        hasHitPlayer = false;
        attackTimer = attackCooldown;

        rb.linearVelocity = Vector2.zero;

        animator.SetFloat("Speed", 0);

        animator.Play("boss0_attack0", 0, 0f);

        Debug.Log("========== BOSS START ATTACK ==========");
    }

    public void AttackPlayer()
    {
        if (!isAttacking ||
            isDead ||
            player == null ||
            playerCollider == null ||
            hasHitPlayer)
        {
            return;
        }

        float xDistance = Mathf.Abs(
            playerCollider.bounds.center.x -
            bossCollider.bounds.center.x
        );

        float yDistance = Mathf.Abs(
            playerCollider.bounds.center.y -
            bossCollider.bounds.center.y
        );

        if (xDistance <= attackRange &&
            yDistance <= attackHeight)
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

    public void FinishAttack()
    {
        isAttacking = false;

        if (!isDead && phase == 1)
        {
            animator.Play("boss0_walk", 0, 0f);
        }
    }

    void EnterPhase2()
    {
        phase = 2;

        isAttacking = false;
        isCharging = false;
        isResting = false;

        attackTimer = 0;
        chargeTimer = 0;
        restTimer = 0;

        rb.linearVelocity = Vector2.zero;

        animator.SetFloat("Speed", 0);

        Debug.Log("========== BOSS PHASE 2 START ==========");

        StartCharge();
    }

    void Phase2()
    {
        if (isResting)
        {
            restTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat("Speed", 0);

            if (restTimer <= 0)
            {
                StartCharge();
            }

            return;
        }

        if (isCharging)
        {
            chargeTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                direction * chargeSpeed,
                rb.linearVelocity.y
            );

            animator.SetFloat("Speed", chargeSpeed);

            if (chargeTimer <= 0)
            {
                StopCharge();
            }

            return;
        }

        StartCharge();
    }

    void StartCharge()
    {
        if (player == null)
        {
            return;
        }

        isCharging = true;
        isResting = false;
        hasHitPlayer = false;

        chargeTimer = chargeTime;

        direction =
            player.position.x > transform.position.x ? 1 : -1;

        FaceDirection();

        animator.Play("boss0_walk", 0, 0f);

        animator.SetFloat("Speed", chargeSpeed);

        Debug.Log("========== BOSS CHARGE ==========");
    }

    void StopCharge()
    {
        isCharging = false;
        isResting = true;

        restTimer = restTime;

        rb.linearVelocity = new Vector2(
            0,
            rb.linearVelocity.y
        );

        animator.SetFloat("Speed", 0);

        Debug.Log("========== BOSS REST ==========");
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!isCharging || isDead)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null && !hasHitPlayer)
            {
                playerHealth.TakeDamage(chargeDamage);

                hasHitPlayer = true;

                Debug.Log("========== BOSS CHARGE HIT ==========");
            }
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Boss0 HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        isAttacking = false;
        isCharging = false;
        isResting = false;

        rb.linearVelocity = Vector2.zero;

        animator.SetFloat("Speed", 0);

        animator.Play("boss0_death", 0, 0f);

        Debug.Log("========== BOSS DEFEATED ==========");
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector2(
            0,
            rb.linearVelocity.y
        );

        animator.SetFloat("Speed", 0);
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