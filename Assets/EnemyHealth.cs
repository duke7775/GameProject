using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 3;
    public bool useDeathAnimation = false;

    private int currentHealth;
    private Animator animator;
    private EnemyDrop enemyDrop;

    void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        enemyDrop = GetComponent<EnemyDrop>();
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0)
        {
            return;
        }

        currentHealth -= damage;

        Debug.Log(
            gameObject.name +
            " HP: " +
            currentHealth
        );

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
    }

    void Die()
    {
        Debug.Log(
            gameObject.name +
            " Died!"
        );

        if (enemyDrop != null)
        {
            enemyDrop.Drop();
        }

        if (useDeathAnimation &&
            animator != null)
        {
            animator.SetTrigger("Death");

            Destroy(
                gameObject,
                1f
            );
        }
        else
        {
            Destroy(gameObject);
        }
    }
}