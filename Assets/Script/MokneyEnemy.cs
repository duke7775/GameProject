using UnityEngine;

public class MonkeyEnemy : MonoBehaviour
{
    public float speed = 1.5f;
    public float chaseRange = 5f;
    public float attackRange = 1.2f;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform player;

    private bool isAttacking = false;

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

        float distance = Vector2.Distance(transform.position, player.position);

        // 玩家太远：原地
        if (distance > chaseRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0);
            return;
        }

        // 玩家进入攻击范围
        if (distance <= attackRange)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            animator.SetFloat("Speed", 0);

            if (!isAttacking)
            {
                isAttacking = true;
                animator.SetTrigger("attack");
                Invoke("FinishAttack", 0.5f);
            }

            return;
        }

        rb.linearVelocity = new Vector2(
            Mathf.Sign(player.position.x - transform.position.x) * speed,
            rb.linearVelocity.y
        );

        animator.SetFloat("Speed", Mathf.Abs(rb.linearVelocity.x));

        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
        else
        {
            transform.localScale = new Vector3(
                -Mathf.Abs(transform.localScale.x),
                transform.localScale.y,
                transform.localScale.z
            );
        }
    }

    void FinishAttack()
    {
        isAttacking = false;
    }
}