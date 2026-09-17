using UnityEngine;

public class XenoEnemy : MonoBehaviour
{
    public Transform player;
    public Transform firePoint;
    public GameObject bulletPrefab;

    public float attackRange = 6f;
    public float attackCooldown = 2f;

    private float lastAttackTime = -999f;

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (player == null)
            return;

        float distance = Vector2.Distance(transform.position, player.position);

        if (distance <= attackRange)
        {
            if (Time.time >= lastAttackTime + attackCooldown)
            {
                Attack();
            }
        }
    }

    void Attack()
{
    lastAttackTime = Time.time;

    animator.SetTrigger("Attack");

    GameObject bullet = Instantiate(
        bulletPrefab,
        firePoint.position,
        Quaternion.identity
    );

    XenoBullet bulletScript = bullet.GetComponent<XenoBullet>();

    if (bulletScript != null)
    {
        float direction = Mathf.Sign(transform.localScale.x);
        bulletScript.SetDirection(direction);
    }
}
}