using UnityEngine;
using System.Collections;

public class Boss2 : MonoBehaviour
{
    [Header("Health")]
    public int maxHealth = 50;

    private int currentHealth;

    [Header("Movement")]
    public float moveSpeed = 2f;
    public float stopDistance = 6f;

    [Header("Shooting")]
    public GameObject bulletPrefab;
    public Transform firePoint;

    public float shootDelay = 0.5f;
    public float bulletInterval = 0.25f;

    [Header("Rest")]
    public float restTime = 2.5f;

    [Header("Escape")]
    public float escapeSpeed = 3f;
    public float escapeDistance = 8f;

    [Header("References")]
    public Transform player;

    private Animator animator;

    private bool isAttacking = false;
    private bool phase2Triggered = false;


    void Start()
    {
        currentHealth = maxHealth;

        animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (player == null)
            return;

        if (phase2Triggered)
            return;

        if (isAttacking)
            return;

        float distance = Mathf.Abs(
            player.position.x - transform.position.x
        );

        if (distance > stopDistance)
        {
            MoveTowardsPlayer();
        }
        else
        {
            StopMoving();

            StartCoroutine(ShootAttack());
        }
    }


    void MoveTowardsPlayer()
    {
        Vector3 targetPosition = new Vector3(
            player.position.x,
            transform.position.y,
            transform.position.z
        );

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime
        );

        animator.SetBool("IsRunning", true);

        FacePlayer();
    }


    void StopMoving()
    {
        animator.SetBool("IsRunning", false);

        FacePlayer();
    }


    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(7f, 7f, 1f);
        }
        else if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(-7f, 7f, 1f);
        }
    }


    IEnumerator ShootAttack()
    {
        isAttacking = true;

        animator.SetBool("IsRunning", false);

        FacePlayer();

        yield return new WaitForSeconds(shootDelay);

        // First bullet
        Shoot();

        yield return new WaitForSeconds(bulletInterval);

        // Second bullet
        Shoot();

        // Give player time to attack
        yield return new WaitForSeconds(restTime);

        isAttacking = false;
    }


    void Shoot()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Boss2 Bullet Prefab is missing!");
            return;
        }

        if (firePoint == null)
        {
            Debug.LogWarning("Boss2 Fire Point is missing!");
            return;
        }

        if (player == null)
            return;

        Vector2 direction = new Vector2(
            player.position.x - firePoint.position.x,
            (player.position.y - firePoint.position.y) * 1.5f
        ).normalized;

        GameObject bullet = Instantiate(
            bulletPrefab,
            firePoint.position,
            Quaternion.identity
        );

        Boss2Bullet bulletScript =
            bullet.GetComponent<Boss2Bullet>();

        if (bulletScript != null)
        {
            bulletScript.damage = 1;
            bulletScript.SetDirection(direction);
        }
    }


    public void TakeDamage(int damage)
    {
        if (phase2Triggered)
            return;

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        Debug.Log("Boss2 HP: " + currentHealth);

        animator.SetTrigger("Hurt");

        if (currentHealth <= maxHealth / 2)
        {
            StartPhase2();
        }
    }


    void StartPhase2()
    {
        phase2Triggered = true;

        StopAllCoroutines();

        isAttacking = false;

        animator.SetBool("IsRunning", false);

        Debug.Log("Boss2 Phase 2 Triggered!");

        StartCoroutine(Escape());
    }


    IEnumerator Escape()
    {
        // Decide which direction to escape
        float escapeDirection;

        if (transform.position.x < player.position.x)
        {
            // Boss is on the left of player -> escape left
            escapeDirection = -1f;
        }
        else
        {
            // Boss is on the right of player -> escape right
            escapeDirection = 1f;
        }

        // Face the escape direction
        transform.localScale = new Vector3(
            escapeDirection * 7f,
            7f,
            1f
        );

        // Play running animation
        animator.SetBool("IsRunning", true);

        float startX = transform.position.x;

        while (Mathf.Abs(transform.position.x - startX) < escapeDistance)
        {
            transform.position += new Vector3(
                escapeDirection * escapeSpeed * Time.deltaTime,
                0f,
                0f
            );

            yield return null;
        }

        animator.SetBool("IsRunning", false);

        // Boss leaves the scene
        gameObject.SetActive(false);

        Debug.Log("Boss2 escaped!");
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;

        Gizmos.DrawWireSphere(
            transform.position,
            stopDistance
        );
    }
}