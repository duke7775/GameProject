using UnityEngine;

public class RobotEnemy : MonoBehaviour
{
    public float speed = 2f;
    public float moveDistance = 3f;
    public float restTime = 2f;

    public int attackDamage = 1;
    public float attackInterval = 0.8f;

    private Vector3 startPosition;
    private int direction = 1;

    private float restTimer = 0f;
    private float attackTimer = 0f;
    private int attackCount = 0;

    private Animator animator;
    private PlayerHealth player;

    void Start()
    {
        startPosition = transform.position;
        animator = GetComponent<Animator>();

        attackTimer = Random.Range(0.2f, 1.5f);
    }

    void Update()
    {
        if (restTimer > 0)
        {
            restTimer -= Time.deltaTime;
            animator.SetBool("Walking", false);
            return;
        }

        animator.SetBool("Walking", true);

        transform.Translate(
            Vector2.right * direction * speed * Time.deltaTime
        );

        attackTimer -= Time.deltaTime;

        if (player != null && attackTimer <= 0f)
        {
            AttackPlayer();
        }

        if (Mathf.Abs(transform.position.x - startPosition.x) >= moveDistance)
        {
            direction *= -1;

            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;

            startPosition = transform.position;

            restTimer = restTime;
            attackCount = 0;
            attackTimer = Random.Range(0.5f, 1.5f);
        }
    }

    void AttackPlayer()
    {
        if (player == null)
            return;

        player.TakeDamage(attackDamage);

        attackCount++;

        if (attackCount >= 2)
        {
            restTimer = restTime;
            attackCount = 0;
            attackTimer = Random.Range(0.5f, 1.5f);
        }
        else
        {
            attackTimer = attackInterval;
        }
    }

    public void SetPlayer(PlayerHealth target)
    {
        if (player == null)
        {
            player = target;
            attackTimer = Random.Range(0.2f, 1.5f);
        }
    }

    public void ClearPlayer(PlayerHealth target)
    {
        if (player == target)
        {
            player = null;
            attackCount = 0;
            attackTimer = Random.Range(0.5f, 1.5f);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealth playerHealth =
                collision.gameObject.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(1);
            }
        }
    }
}