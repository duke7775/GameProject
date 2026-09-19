using UnityEngine;
using System.Collections;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 10;
    public int currentHealth = 5;

    public float hurtDuration = 1.2f;
    public float blinkInterval = 0.1f;

    private Animator animator;
    private SpriteRenderer[] spriteRenderers;

    private bool invincible = false;
    private bool hurt = false;

    void Start()
    {
        animator = GetComponent<Animator>();

        spriteRenderers =
            GetComponentsInChildren<SpriteRenderer>();

        if (PlayerDataManager.instance != null &&
            PlayerDataManager.instance.hasData)
        {
            currentHealth =
                PlayerDataManager.instance.currentHealth;

            maxHealth =
                PlayerDataManager.instance.maxHealth;
        }

        currentHealth = Mathf.Clamp(
            currentHealth,
            0,
            maxHealth
        );
    }

    public void TakeDamage(int damage)
    {
        if (invincible)
        {
            return;
        }

        currentHealth -= damage;

        if (currentHealth < 0)
        {
            currentHealth = 0;
        }

        SavePlayerData();

        Debug.Log(
            "Player HP: " +
            currentHealth +
            "/" +
            maxHealth
        );

        if (currentHealth <= 0)
        {
            Die();
            return;
        }

        StartCoroutine(HurtRoutine());
    }

    IEnumerator HurtRoutine()
    {
        if (hurt)
        {
            yield break;
        }

        hurt = true;
        invincible = true;

        if (animator != null)
        {
            animator.SetTrigger("Hurt");
        }

        float timer = 0f;

        while (timer < hurtDuration)
        {
            SetPlayerVisible(false);

            yield return new WaitForSeconds(
                blinkInterval
            );

            timer += blinkInterval;

            SetPlayerVisible(true);

            yield return new WaitForSeconds(
                blinkInterval
            );

            timer += blinkInterval;
        }

        SetPlayerVisible(true);

        invincible = false;
        hurt = false;
    }

    void SetPlayerVisible(bool visible)
    {
        if (spriteRenderers == null)
        {
            return;
        }

        foreach (
            SpriteRenderer spriteRenderer
            in spriteRenderers
        )
        {
            spriteRenderer.enabled = visible;
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;

        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }

        SavePlayerData();

        Debug.Log(
            "Player HP: " +
            currentHealth +
            "/" +
            maxHealth
        );
    }

    void SavePlayerData()
    {
        if (PlayerDataManager.instance == null)
        {
            return;
        }

        PlayerMovement playerMovement =
            GetComponent<PlayerMovement>();

        if (playerMovement == null)
        {
            return;
        }

        PlayerDataManager.instance.SavePlayer(
            playerMovement.attackLevel,
            playerMovement.attackSpeed,
            playerMovement.attackRange,
            playerMovement.jumpLevel,
            playerMovement.jumpForce,
            currentHealth,
            maxHealth
        );
    }

    public bool IsInvincible()
    {
        return invincible;
    }

    public bool IsHurt()
    {
        return hurt;
    }

    void Die()
    {
        Debug.Log("Player Died!");
    }
}