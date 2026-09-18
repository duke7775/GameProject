using UnityEngine;
using System.Collections;

public class MonkeyEnemy : MonoBehaviour
{
    public float speed = 1.5f;
    public float minSpeed = 1.1f;
    public float maxSpeed = 1.8f;

    public float chaseRange = 5f;
    public float attackRange = 2f;
    public float attackYRange = 1f;
    public float turnDeadZone = 0.5f;

    public float minPatrolTime = 2f;
    public float maxPatrolTime = 5f;

    public float minPauseTime = 0.5f;
    public float maxPauseTime = 1.5f;

    public int attackDamage = 1;
    public float attackHitDelay = 0.4f;

    public float minStartDelay = 0f;
    public float maxStartDelay = 2f;

    private int direction = 1;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;
    private Collider2D monkeyCollider;

    private bool isAttacking = false;
    private bool hasHitPlayer = false;
    private bool avoidingEdge = false;
    private bool patrolling = false;
    private bool patrolStarted = false;

    private float attackCooldown = 1.5f;
    private float attackTimer = 0f;
    private float edgeAvoidTimer = 0f;
    private float patrolTimer = 0f;
    private float pauseTimer = 0f;
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

        StartCoroutine(
            DelayedPatrol()
        );

        MonkeyEnemy[] monkeys =
            FindObjectsByType<MonkeyEnemy>(
                FindObjectsSortMode.None
            );

        foreach (MonkeyEnemy monkey in monkeys)
        {
            if (monkey != this)
            {
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
    }

    IEnumerator DelayedPatrol()
    {
        float delay = Random.Range(
            minStartDelay,
            maxStartDelay
        );

        yield return new WaitForSeconds(delay);

        patrolStarted = true;

        StartPatrol();
    }

    void FixedUpdate()
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

        if (isAttacking)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            return;
        }

        if (!patrolStarted)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat(
                "Speed",
                0
            );

            return;
        }

        if (player == null)
        {
            Patrol();
            return;
        }

        float xDistance = Mathf.Abs(
            player.position.x -
            transform.position.x
        );

        float yDistance = Mathf.Abs(
            player.position.y -
            transform.position.y
        );

        if (xDistance <= attackRange &&
            yDistance <= attackYRange)
        {
            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

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

        Patrol();
    }

    void StartAttack()
    {
        if (isAttacking)
        {
            return;
        }

        isAttacking = true;
        hasHitPlayer = false;
        attackTimer = attackCooldown;

        rb.linearVelocity = new Vector2(
            0,
            rb.linearVelocity.y
        );

        animator.Play(
            "monkey2",
            0,
            0f
        );

        StartCoroutine(
            AttackRoutine()
        );
    }

    IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(
            attackHitDelay
        );

        AttackPlayer();

        while (true)
        {
            AnimatorStateInfo state =
                animator.GetCurrentAnimatorStateInfo(0);

            if (!state.IsName("monkey2"))
            {
                break;
            }

            if (state.normalizedTime >= 1f)
            {
                break;
            }

            yield return null;
        }

        isAttacking = false;
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

        float xDistance = Mathf.Abs(
            player.position.x -
            transform.position.x
        );

        float yDistance = Mathf.Abs(
            player.position.y -
            transform.position.y
        );

        if (xDistance <= attackRange &&
            yDistance <= attackYRange)
        {
            PlayerHealth playerHealth =
                player.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(
                    attackDamage
                );

                hasHitPlayer = true;

                Debug.Log(
                    "Monkey attacked Player. Damage = "
                    + attackDamage
                );
            }
        }
    }

    void Patrol()
    {
        if (avoidingEdge)
        {
            MovePatrol();
            return;
        }

        if (pauseTimer > 0)
        {
            pauseTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat(
                "Speed",
                0
            );

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
        rb.linearVelocity = new Vector2(
            direction * patrolSpeed,
            rb.linearVelocity.y
        );

        animator.SetFloat(
            "Speed",
            Mathf.Abs(rb.linearVelocity.x)
        );

        FaceDirection();
    }

    void ChasePlayer()
    {
        if (avoidingEdge)
        {
            rb.linearVelocity = new Vector2(
                direction * speed,
                rb.linearVelocity.y
            );

            animator.SetFloat(
                "Speed",
                speed
            );

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

        if (Mathf.Abs(difference) > turnDeadZone)
        {
            if (difference > 0)
            {
                direction = 1;
            }
            else
            {
                direction = -1;
            }
        }

        rb.linearVelocity = new Vector2(
            direction * speed,
            rb.linearVelocity.y
        );

        animator.SetFloat(
            "Speed",
            Mathf.Abs(rb.linearVelocity.x)
        );

        FaceDirection();
    }

    bool IsEdgeAhead()
    {
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
        float difference =
            player.position.x -
            transform.position.x;

        if (Mathf.Abs(difference) <= turnDeadZone)
        {
            return;
        }

        if (difference > 0)
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
            direction *
            Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}