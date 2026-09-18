using UnityEngine;
using System.Collections;

public class DroneEnemy : MonoBehaviour
{
    public float detectRange = 5f;

    public float downDistance = 2f;
    public float moveSpeed = 2f;
    public float restTime = 2f;

    public GameObject bulletPrefab;
    public Transform firePoint;

    public float attackCooldown = 2f;
    public float attackDelay = 0.5f;

    private Vector3 highPosition;
    private Vector3 lowPosition;

    private PlayerHealth player;
    private Animator animator;

    private float attackTimer = 0f;
    private bool attacking = false;

    void Start()
    {
        highPosition = transform.position;
        lowPosition = highPosition + Vector3.down * downDistance;

        animator = GetComponent<Animator>();

        attackTimer = Random.Range(0.5f, 1.5f);
    }

    void Update()
    {
        if (!attacking)
        {
            FindPlayer();

            attackTimer -= Time.deltaTime;

            if (player != null && attackTimer <= 0f)
            {
                StartCoroutine(Attack());
            }
        }
    }

    void FindPlayer()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(
            transform.position,
            detectRange
        );

        player = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                PlayerHealth target = hit.GetComponent<PlayerHealth>();

                if (target != null)
                {
                    player = target;
                    return;
                }
            }
        }
    }

    IEnumerator Attack()
    {
        attacking = true;

        Vector2 targetPosition = player.transform.position;

        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(attackDelay);

        Shoot(targetPosition);

        yield return StartCoroutine(MoveDown());

        yield return new WaitForSeconds(restTime);

        yield return StartCoroutine(MoveUp());

        player = null;

        attackTimer = attackCooldown;

        attacking = false;
    }

    void Shoot(Vector2 targetPosition)
    {
        if (bulletPrefab == null)
            return;

        if (firePoint == null)
            return;

        Vector2 direction =
            (targetPosition - (Vector2)firePoint.position).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        DroneBullet bulletScript =
            bullet.GetComponent<DroneBullet>();

        if (bulletScript != null)
        {
            bulletScript.SetDirection(direction);
        }
    }

    IEnumerator MoveDown()
    {
        while (Vector3.Distance(transform.position, lowPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                lowPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    IEnumerator MoveUp()
    {
        while (Vector3.Distance(transform.position, highPosition) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                highPosition,
                moveSpeed * Time.deltaTime
            );

            yield return null;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(
            transform.position,
            detectRange
        );
    }
}