using UnityEngine;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 9f;
    public float jumpCut = 0.5f;

    public int attackDamage = 1;
    public float attackRange = 1.2f;
    public float attackSpeed = 0.5f;

    public int attackLevel = 1;
    public int jumpLevel = 1;

    public AudioSource audioSource;
    public AudioClip attackSound;
    public AudioClip hitSound;

    private float attackTimer = 0f;
    private bool isAttacking = false;
    private bool isGrounded = false;
    private int groundContacts = 0;

    private Rigidbody2D rb;
    private Animator animator;

    private Collider2D currentDropPlatform;
    private Collider2D playerCollider;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();

        if (PlayerDataManager.instance != null &&
            PlayerDataManager.instance.hasData)
        {
            attackLevel =
                PlayerDataManager.instance.attackLevel;

            attackSpeed =
                PlayerDataManager.instance.attackSpeed;

            attackRange =
                PlayerDataManager.instance.attackRange;

            jumpLevel =
                PlayerDataManager.instance.jumpLevel;

            jumpForce =
                PlayerDataManager.instance.jumpForce;
        }
    }

    void Update()
    {
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (isAttacking &&
            !animator.GetCurrentAnimatorStateInfo(0).IsName("attack"))
        {
            isAttacking = false;
        }

        if (Input.GetKeyDown(KeyCode.J) &&
            attackTimer <= 0 &&
            isGrounded)
        {
            isAttacking = true;

            animator.SetTrigger("Attack");

            if (audioSource != null &&
                attackSound != null)
            {
                audioSource.PlayOneShot(attackSound);
            }

            attackTimer = 0.5f / attackSpeed;
        }

        float move = Input.GetAxis("Horizontal");

        rb.linearVelocity = new Vector2(
            move * speed,
            rb.linearVelocity.y
        );

        if (!isAttacking)
        {
            if (move > 0)
            {
                transform.localScale =
                    new Vector3(1, 1, 1);
            }
            else if (move < 0)
            {
                transform.localScale =
                    new Vector3(-1, 1, 1);
            }
        }

        animator.SetFloat(
            "Speed",
            Mathf.Abs(move)
        );

        if (Input.GetKeyDown(KeyCode.Space) &&
            isGrounded)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                jumpForce
            );

            isGrounded = false;
        }
        else if (Input.GetKeyUp(KeyCode.Space) &&
                 rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * jumpCut
            );
        }

        if (Input.GetKeyDown(KeyCode.S) &&
            currentDropPlatform != null)
        {
            StartCoroutine(
                DropThroughPlatform()
            );
        }
    }

    public void Attack()
    {
        Vector2 attackPosition =
            transform.position;

        attackPosition.x +=
            transform.localScale.x *
            attackRange;

        Collider2D[] hits =
            Physics2D.OverlapCircleAll(
                attackPosition,
                0.8f
            );

        bool hitEnemy = false;

        foreach (Collider2D hit in hits)
        {
            Boss0 boss =
                hit.GetComponent<Boss0>();

            if (boss != null)
            {
                boss.TakeDamage(attackDamage);
                hitEnemy = true;
                continue;
            }

            Boss2 boss2 =
                hit.GetComponent<Boss2>();

            if (boss2 != null)
            {
                boss2.TakeDamage(attackDamage);
                hitEnemy = true;
                continue;
            }

            Boss2Final boss2Final =
                hit.GetComponent<Boss2Final>();

            if (boss2Final != null)
            {
                boss2Final.TakeDamage(
                    attackDamage
                );
                hitEnemy = true;
                continue;
            }

            EnemyHealth enemy =
                hit.GetComponent<EnemyHealth>();

            if (enemy != null)
            {
                enemy.TakeDamage(
                    attackDamage
                );

                hitEnemy = true;
            }
        }

        if (hitEnemy &&
            audioSource != null &&
            hitSound != null)
        {
            audioSource.PlayOneShot(hitSound);
        }
    }

    public void FinishAttack()
    {
        isAttacking = false;
    }

    public void UpgradeAttack()
    {
        if (attackSpeed < 2.0f)
        {
            attackSpeed = Mathf.Min(
                attackSpeed + 0.2f,
                2.0f
            );

            attackRange = Mathf.Min(
                attackRange + 0.1f,
                1.6f
            );

            attackLevel++;

            SavePlayerData();
        }

        Debug.Log(
            "Attack upgraded! Level = " +
            attackLevel +
            " Speed = " +
            attackSpeed +
            " Range = " +
            attackRange
        );
    }

    public void UpgradeJump()
    {
        if (jumpForce < 11f)
        {
            jumpForce = Mathf.Min(
                jumpForce + 0.2f,
                11f
            );

            jumpLevel++;

            SavePlayerData();
        }

        Debug.Log(
            "Jump upgraded! Level = " +
            jumpLevel +
            " Jump Force = " +
            jumpForce
        );
    }

    void SavePlayerData()
    {
        if (PlayerDataManager.instance == null)
        {
            return;
        }

        PlayerHealth playerHealth =
            GetComponent<PlayerHealth>();

        int currentHealth = 5;
        int maxHealth = 10;

        if (playerHealth != null)
        {
            currentHealth =
                playerHealth.currentHealth;

            maxHealth =
                playerHealth.maxHealth;
        }

        PlayerDataManager.instance.SavePlayer(
            attackLevel,
            attackSpeed,
            attackRange,
            jumpLevel,
            jumpForce,
            currentHealth,
            maxHealth
        );
    }

    void OnCollisionEnter2D(
        Collision2D collision
    )
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts++;

            isGrounded = true;
        }

        if (collision.gameObject.layer ==
            LayerMask.NameToLayer(
                "DropPlatform"
            ))
        {
            currentDropPlatform =
                collision.collider;
        }
    }

    void OnCollisionExit2D(
        Collision2D collision
    )
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            groundContacts--;

            if (groundContacts < 0)
            {
                groundContacts = 0;
            }

            if (groundContacts == 0)
            {
                isGrounded = false;
            }
        }

        if (collision.collider ==
            currentDropPlatform)
        {
            currentDropPlatform = null;
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector2 attackPosition =
            transform.position;

        attackPosition.x +=
            transform.localScale.x *
            attackRange;

        Gizmos.DrawWireSphere(
            attackPosition,
            0.8f
        );
    }

    IEnumerator DropThroughPlatform()
    {
        groundContacts = 0;
        isGrounded = false;

        Physics2D.IgnoreCollision(
            playerCollider,
            currentDropPlatform,
            true
        );

        Collider2D platform =
            currentDropPlatform;

        while (
            playerCollider.bounds.max.y >
            platform.bounds.min.y
        )
        {
            yield return null;
        }

        Physics2D.IgnoreCollision(
            playerCollider,
            platform,
            false
        );

        currentDropPlatform = null;
    }
}