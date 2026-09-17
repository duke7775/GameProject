using UnityEngine;

public class BoarBoss : MonoBehaviour
{
    public int maxHealth = 100;

    public float walkSpeed = 1.5f;
    public float runSpeed = 4f;

    public float attackRange = 2f;
    public float attackCooldown = 1.2f;

    public float chargeTime = 1.5f;
    public float restTime = 1.5f;

    public int attackDamage = 2;
    public int chargeDamage = 3;

    private int currentHealth;
    private int phase = 1;

    private Animator animator;
    private Transform player;

    private bool introFinished = false;
    private bool isAttacking = false;
    private bool isCharging = false;
    private bool isResting = false;

    private float attackTimer = 0f;
    private float chargeTimer = 0f;
    private float restTimer = 0f;

    private int chargeDirection = 1;

    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();

        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

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

        if (!introFinished)
        {
            return;
        }

        if (player == null)
        {
            return;
        }

        if (phase == 2)
        {
            Phase2Behavior();
        }
        else
        {
            Phase1Behavior();
        }
    }

    public void FinishIntro()
    {
        introFinished = true;
    }

    void Phase1Behavior()
    {
        if (isAttacking)
        {
            animator.SetFloat("Speed", 0);
            return;
        }

        float distance = Mathf.Abs(
            player.position.x - transform.position.x
        );

        if (distance <= attackRange)
        {
            animator.SetFloat("Speed", 0);

            FacePlayer();

            if (attackTimer <= 0)
            {
                StartAttack();
            }

            return;
        }

        int direction =
            player.position.x > transform.position.x ? 1 : -1;

        transform.position += Vector3.right *
                             direction *
                             walkSpeed *
                             Time.deltaTime;

        animator.SetFloat("Speed", walkSpeed);

        FaceDirection(direction);
    }

    void StartAttack()
    {
        isAttacking = true;
        attackTimer = attackCooldown;

        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Attack");
    }

    public void AttackPlayer()
    {
        if (!isAttacking || player == null)
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
            }
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

    void Phase2Behavior()
    {
        if (isResting)
        {
            restTimer -= Time.deltaTime;

            animator.SetFloat("Speed", 0);

            if (restTimer <= 0)
            {
                isResting = false;
                StartCharge();
            }

            return;
        }

        if (isCharging)
        {
            chargeTimer -= Time.deltaTime;

            transform.position += Vector3.right *
                                 chargeDirection *
                                 runSpeed *
                                 Time.deltaTime;

            animator.SetFloat("Speed", runSpeed);

            if (chargeTimer <= 0)
            {
                EndCharge();
            }

            return;
        }

        StartCharge();
    }

    void StartCharge()
    {
        isCharging = true;
        chargeTimer = chargeTime;

        chargeDirection =
            player.position.x > transform.position.x ? 1 : -1;

        FaceDirection(chargeDirection);

        animator.Play("boss_run");
        animator.SetFloat("Speed", runSpeed);
    }

    void EndCharge()
    {
        isCharging = false;
        isResting = true;
        restTimer = restTime;

        animator.SetFloat("Speed", 0);
        animator.Play("boss_reset");
    }

    void FacePlayer()
    {
        int direction =
            player.position.x > transform.position.x ? 1 : -1;

        FaceDirection(direction);
    }

    void FaceDirection(int direction)
    {
        transform.localScale = new Vector3(
            direction * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        PlayerHealth playerHealth =
            collision.gameObject.GetComponent<PlayerHealth>();

        if (playerHealth != null && phase == 2 && isCharging)
        {
            playerHealth.TakeDamage(chargeDamage);
        }

        if (isCharging)
        {
            EndCharge();
        }
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
            currentHealth = 0;
            Die();
            return;
        }

        if (currentHealth <= maxHealth / 2 && phase == 1)
        {
            phase = 2;

            isAttacking = false;
            isCharging = false;
            isResting = false;

            attackTimer = 0;

            animator.SetFloat("Speed", 0);

            Debug.Log("Boss Phase 2!");
        }
    }

    void Die()
    {
        animator.SetFloat("Speed", 0);
        animator.SetTrigger("Dead");

        enabled = false;
    }
}