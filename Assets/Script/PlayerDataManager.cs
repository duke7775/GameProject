using UnityEngine;

public class PlayerDataManager : MonoBehaviour
{
    public static PlayerDataManager instance;

    public bool hasData = false;

    public int attackLevel = 1;
    public float attackSpeed = 0.5f;
    public float attackRange = 1.2f;

    public int jumpLevel = 1;
    public float jumpForce = 9f;

    public int currentHealth = 5;
    public int maxHealth = 10;

    void Awake()
    {
        if (instance != null &&
            instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    public void SavePlayer(
        int attackLevel,
        float attackSpeed,
        float attackRange,
        int jumpLevel,
        float jumpForce,
        int currentHealth,
        int maxHealth
    )
    {
        this.attackLevel = attackLevel;
        this.attackSpeed = attackSpeed;
        this.attackRange = attackRange;

        this.jumpLevel = jumpLevel;
        this.jumpForce = jumpForce;

        this.currentHealth = currentHealth;
        this.maxHealth = maxHealth;

        hasData = true;
    }
}