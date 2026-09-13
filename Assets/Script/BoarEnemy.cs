using UnityEngine;

public class BoarEnemy : MonoBehaviour
{
    public float walkSpeed = 1.2f;
    public float runSpeed = 3.5f;
    public float detectRange = 5f;
    public float chargeTime = 1.5f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private bool isCharging = false;
    private float chargeTimer = 0f;
    private int chargeDirection = 1;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }

    void FixedUpdate()
    {
        if (player == null)
        {
            return;
        }

        if (isCharging)
        {
            chargeTimer -= Time.fixedDeltaTime;

            rb.linearVelocity = new Vector2(
                chargeDirection * runSpeed,
                rb.linearVelocity.y
            );

            animator.SetFloat("Speed", runSpeed);

    
            if (chargeTimer <= 0)
            {
                isCharging = false;
                rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
                animator.SetFloat("speed", 0);
            }

            return;
        }

        float distance = Vector2.Distance(transform.position, player.position);
        if (distance <= detectRange)
        {
            isCharging = true;
            chargeTimer = chargeTime;

            chargeDirection =
                player.position.x > transform.position.x ? 1 : -1;

            FaceDirection(chargeDirection);

            rb.linearVelocity = new Vector2(
                chargeDirection * runSpeed,
                rb.linearVelocity.y
            );

            animator.SetFloat("speed", runSpeed);

            return;
        }

        rb.linearVelocity = new Vector2(
            walkSpeed * Mathf.Sign(transform.localScale.x),
            rb.linearVelocity.y
        );

        animator.SetFloat("Speed", walkSpeed);
    }

    void FaceDirection(int direction)
    {
        transform.localScale = new Vector3(
            direction * Mathf.Abs(transform.localScale.x),
            transform.localScale.y,
            transform.localScale.z
        );
    }
}