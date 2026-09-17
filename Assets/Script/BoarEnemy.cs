using UnityEngine;

public class BoarEnemy : MonoBehaviour
{
    public float walkSpeed = 1.2f;
    public float runSpeed = 3.5f;
    public float detectRange = 5f;
    public float chargeTime = 1.5f;

    public int attackDamage = 2;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private int direction = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        direction = transform.localScale.x >= 0 ? 1 : -1;
    }

    void FixedUpdate()
    {
        if (isCharging)
        {
            chargeTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                direction * runSpeed,
                rb.linearVelocity.y
            );

            FaceDirection();
            animator.SetFloat("Speed", runSpeed);

            if (chargeTimer <= 0)
            {
                isCharging = false;

                rb.linearVelocity = new Vector2(
                    0,
                    rb.linearVelocity.y
                );

                animator.SetFloat("Speed", 0);
            }

            return;
        }

        if (player != null)
        {
            float distance = Mathf.Abs(
                player.position.x - transform.position.x
            );

            if (distance <= detectRange)
            {
                direction =
                    player.position.x > transform.position.x ? 1 : -1;

                isCharging = true;
                chargeTimer = chargeTime;

                FaceDirection();

                rb.linearVelocity = new Vector2(
                    direction * runSpeed,
                    rb.linearVelocity.y
                );

                animator.SetFloat("Speed", runSpeed);

                return;
            }
        }

        Patrol();
    }

    void Patrol()
    {
        rb.linearVelocity = new Vector2(
            direction * walkSpeed,
            rb.linearVelocity.y
        );

        FaceDirection();
        animator.SetFloat("Speed", walkSpeed);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(attackDamage);
            }

            isCharging = false;

            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            animator.SetFloat("Speed", 0);

            return;
        }

        if (collision.gameObject.CompareTag("BoarWall"))
        {
            direction *= -1;

            isCharging = false;
            chargeTimer = 0;

            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            FaceDirection();

            animator.SetFloat("Speed", 0);

            return;
        }

        if (collision.gameObject.GetComponent<BoarEnemy>() != null)
        {
            direction *= -1;

            isCharging = false;
            chargeTimer = 0;

            rb.linearVelocity = new Vector2(
                0,
                rb.linearVelocity.y
            );

            FaceDirection();

            animator.SetFloat("Speed", 0);
        }
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