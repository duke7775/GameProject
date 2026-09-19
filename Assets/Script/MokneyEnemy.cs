using UnityEngine;

public class MonkeyEnemy : MonoBehaviour
{
    public float speed = 1.5f;
    public float minSpeed = 1.1f;
    public float maxSpeed = 1.8f;

    public float chaseRange = 5f;
    public float attackRange = 1.5f;
    public float attackYRange = 1.5f;
    public float turnDeadZone = 0.5f;

    public float minPatrolTime = 2f;
    public float maxPatrolTime = 5f;

    public float minPauseTime = 0.5f;
    public float maxPauseTime = 1.5f;

    public int attackDamage = 1;
    public float attackDuration = 0.8f;
    public float attackHitDelay = 0.3f;
    public float attackCooldown = 1.5f;

    public float minStartDelay = 0f;
    public float maxStartDelay = 2f;

    private Rigidbody2D rb;
    private Animator animator;
    private Collider2D monkeyCollider;
    private Transform player;

    private int direction = 1;

    private bool isDead = false;
    private bool isAttacking = false;
    private bool hasHitPlayer = false;
    private bool patrolling = false;
    private bool patrolStarted = false;
    private bool avoidingEdge = false;

    private float patrolTimer = 0f;
    private float pauseTimer = 0f;
    private float attackTimer = 0f;
    private float attackAnimationTimer = 0f;
    private float attackHitTimer = 0f;
    private float edgeAvoidTimer = 0f;
    private float patrolSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        monkeyCollider = GetComponent<Collider2D>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        direction =
            Random.value > 0.5f ? 1 : -1;

        patrolSpeed = Random.Range(
            minSpeed,
            maxSpeed
        );

        Invoke(
            nameof(StartInitialPatrol),
            Random.Range(
                minStartDelay,
                maxStartDelay
            )
        );

        IgnoreOtherMonkeys();
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        UpdateTimers();

        if (isAttacking)
        {
            UpdateAttack();
            return;
        }

        if (!patrolStarted)
        {
            StopMovement();
            SetAnimationSpeed(0);
            return;
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        Collider2D playerCollider =
            player.GetComponent<Collider2D>();

        if (playerCollider != null &&
            monkeyCollider != null)
        {
            float xDistance = Mathf.Abs(
                playerCollider.bounds.center.x -
                monkeyCollider.bounds.center.x
            );

            float yDistance = Mathf.Abs(
                playerCollider.bounds.center.y -
                monkeyCollider.bounds.center.y
            );

            if (xDistance <= attackRange &&
                yDistance <= attackYRange)
            {
                StopMovement();
                FacePlayer();

                if (attackTimer <= 0)
                {
                    StartAttack();
                }

                return;
            }

            if (xDistance <= chaseRange)
            {
                ChasePlayer();
                return;
            }
        }

        Patrol();
    }

    void UpdateTimers()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.fixedDeltaTime;
        }

        if (edgeAvoidTimer > 0)
        {
            edgeAvoidTimer -= Time.fixedDeltaTime;
        }
        else
        {
            avoidingEdge = false;
        }
    }

    void StartInitialPatrol()
    {
        if (isDead)
        {
            return;
        }

        patrolStarted = true;

        StartPatrol();
    }

    void StartAttack()
    {
        if (isDead ||
            isAttacking)
        {
            return;
        }

        isAttacking = true;
        hasHitPlayer = false;

        attackTimer = attackCooldown;
        attackAnimationTimer = attackDuration;
        attackHitTimer = attackHitDelay;

        StopMovement();

        if (animator != null)
        {
            animator.Play(
                "monkey2",
                0,
                0f
            );
        }

        Debug.Log("Monkey started attack");
    }

    void UpdateAttack()
    {
        if (isDead)
        {
            return;
        }

        StopMovement();

        attackAnimationTimer -=
            Time.fixedDeltaTime;

        attackHitTimer -=
            Time.fixedDeltaTime;

        if (!hasHitPlayer &&
            attackHitTimer <= 0)
        {
            AttackPlayer();
        }

        if (attackAnimationTimer <= 0)
        {
            isAttacking = false;
        }
    }

    void AttackPlayer()
    {
        if (isDead ||
            hasHitPlayer ||
            player == null)
        {
            return;
        }

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        if (playerHealth != null)
        {
            playerHealth.TakeDamage(
                attackDamage
            );

            hasHitPlayer = true;

            Debug.Log(
                "Monkey attacked Player! Damage = " +
                attackDamage
            );
        }
    }

    void Patrol()
    {
        if (isDead)
        {
            return;
        }

        if (avoidingEdge)
        {
            MovePatrol();
            return;
        }

        if (pauseTimer > 0)
        {
            pauseTimer -= Time.fixedDeltaTime;

            StopMovement();
            SetAnimationSpeed(0);

            return;
        }

        if (IsEdgeAhead())
        {
            TurnAroundFromEdge();
            return;
        }

        patrolTimer -= Time.fixedDeltaTime;

        if (patrolTimer <= 0)
        {
            if (patrolling)
            {
                patrolling = false;

                pauseTimer = Random.Range(
                    minPauseTime,
                    maxPauseTime
                );
            }
            else
            {
                StartPatrol();
            }

            return;
        }

        MovePatrol();
    }

    void StartPatrol()
    {
        if (isDead)
        {
            return;
        }

        patrolling = true;

        patrolTimer = Random.Range(
            minPatrolTime,
            maxPatrolTime
        );

        patrolSpeed = Random.Range(
            minSpeed,
            maxSpeed
        );

        if (Random.value > 0.5f)
        {
            direction *= -1;
        }
    }

    void MovePatrol()
    {
        if (isDead)
        {
            return;
        }

        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );

        SetAnimationSpeed(
            Mathf.Abs(rb.linearVelocity.x)
        );

        FaceDirection();
    }

    void ChasePlayer()
    {
        if (isDead ||
            player == null)
        {
            return;
        }

        if (avoidingEdge)
        {
            rb.linearVelocity = new Vector2(
                direction * speed,
                rb.linearVelocity.y
            );

            SetAnimationSpeed(speed);
            FaceDirection();

            return;
        }

        if (IsEdgeAhead())
        {
            TurnAroundFromEdge();
            return;
        }

        float difference =
            player.position.x -
            transform.position.x;

        if (Mathf.Abs(difference) >
            turnDeadZone)
        {
            direction =
                difference > 0 ? 1 : -1;
        }

        rb.linearVelocity = new Vector2(
            direction * speed,
            rb.linearVelocity.y
        );

        SetAnimationSpeed(
            Mathf.Abs(rb.linearVelocity.x)
        );

        FaceDirection();
    }

    bool IsEdgeAhead()
    {
        if (monkeyCollider == null)
        {
            return false;
        }

        float checkX =
            monkeyCollider.bounds.center.x +
            direction *
            monkeyCollider.bounds.extents.x *
            0.7f;

        float checkY =
            monkeyCollider.bounds.min.y +
            0.1f;

        Vector2 checkPosition =
            new Vector2(
                checkX,
                checkY
            );

        RaycastHit2D hit =
            Physics2D.Raycast(
                checkPosition,
                Vector2.down,
                0.5f
            );

        if (hit.collider == null)
        {
            return true;
        }

        if (!hit.collider.CompareTag("Ground"))
        {
            return true;
        }

        return false;
    }

    void TurnAroundFromEdge()
    {
        if (isDead)
        {
            return;
        }

        direction *= -1;

        avoidingEdge = true;
        edgeAvoidTimer = 0.5f;

        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );

        FaceDirection();
    }

    void FacePlayer()
    {
        if (player == null)
        {
            return;
        }

        float difference =
            player.position.x -
            transform.position.x;

        if (Mathf.Abs(difference) <=
            turnDeadZone)
        {
            return;
        }

        direction =
            difference > 0 ? 1 : -1;

        FaceDirection();
    }

    void FaceDirection()
    {
        if (isDead)
        {
            return;
        }

        transform.localScale =
            new Vector3(
                direction *
                Mathf.Abs(
                    transform.localScale.x
                ),
                transform.localScale.y,
                transform.localScale.z
            );
    }

    void StopMovement()
    {
        if (rb == null)
        {
            return;
        }

        rb.linearVelocity =
            new Vector2(
                0,
                rb.linearVelocity.y
            );
    }

    void SetAnimationSpeed(float value)
    {
        if (animator == null)
        {
            return;
        }

        animator.SetFloat(
            "Speed",
            value
        );
    }

    void IgnoreOtherMonkeys()
    {
        MonkeyEnemy[] monkeys =
            FindObjectsByType<MonkeyEnemy>(
                FindObjectsSortMode.None
            );

        foreach (MonkeyEnemy monkey in monkeys)
        {
            if (monkey == this)
            {
                continue;
            }

            Collider2D otherCollider =
                monkey.GetComponent<Collider2D>();

            if (otherCollider != null &&
                monkeyCollider != null)
            {
                Physics2D.IgnoreCollision(
                    monkeyCollider,
                    otherCollider,
                    true
                );
            }
        }
    }

    public void StopEnemy()
    {
        if (isDead)
        {
            return;
        }

        isDead = true;
        isAttacking = false;
        hasHitPlayer = true;

        CancelInvoke();

        StopMovement();

        if (monkeyCollider != null)
        {
            monkeyCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.simulated = false;
        }

        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}