using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Boss2Final : MonoBehaviour
{
    public float moveSpeed = 2f;

    public float flyHeight = 4f;
    public float flySpeed = 5f;

    public GameObject bombWarningPrefab;

    public int minBombCount = 3;
    public int maxBombCount = 4;

    public float bombMinX = 121f;
    public float bombMaxX = 144f;
    public float bombY = 30f;

    public float warningTime = 1.5f;
    public float explosionTime = 0.4f;

    public float attackInterval = 2f;

    public Transform player;

    public int maxHealth = 30;

    private int currentHealth;
    private Animator animator;
    private float groundY;
    private bool attacking = false;
    private bool dead = false;

    void Start()
    {
        Debug.Log("[Boss2Final] Start");

        animator = GetComponent<Animator>();

        if (animator == null)
        {
            Debug.LogError("[Boss2Final] Animator is missing!");
        }
        else
        {
            Debug.Log("[Boss2Final] Animator found");
        }

        currentHealth = maxHealth;

        Debug.Log(
            "[Boss2Final] HP = "
            + currentHealth
            + "/"
            + maxHealth
        );

        groundY = transform.position.y;

        Debug.Log("[Boss2Final] Ground Y = " + groundY);

        if (player == null)
        {
            Debug.LogError("[Boss2Final] Player is missing!");
        }
        else
        {
            Debug.Log(
                "[Boss2Final] Player found: "
                + player.name
            );
        }

        if (bombWarningPrefab == null)
        {
            Debug.LogError(
                "[Boss2Final] Bomb Warning Prefab is missing!"
            );
        }
        else
        {
            Debug.Log(
                "[Boss2Final] Bomb Warning Prefab found: "
                + bombWarningPrefab.name
            );
        }

        StartCoroutine(BossAttackLoop());
    }

    void Update()
    {
        if (player == null)
            return;

        if (attacking || dead)
            return;

        float direction = Mathf.Sign(
            player.position.x - transform.position.x
        );

        transform.position += new Vector3(
            direction * moveSpeed * Time.deltaTime,
            0,
            0
        );

        FacePlayer();
    }

    void FacePlayer()
    {
        if (player.position.x > transform.position.x)
        {
            transform.localScale = new Vector3(
                7f,
                7f,
                1f
            );
        }
        else if (player.position.x < transform.position.x)
        {
            transform.localScale = new Vector3(
                -7f,
                7f,
                1f
            );
        }
    }

    public void TakeDamage(int damage)
    {
        if (dead)
            return;

        currentHealth -= damage;

        Debug.Log(
            "[Boss2Final] Took "
            + damage
            + " damage. HP = "
            + currentHealth
            + "/"
            + maxHealth
        );

        if (currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else
        {
            if (animator != null)
            {
                animator.SetTrigger("Hurt");
            }
        }
    }

    void Die()
    {
        dead = true;
        attacking = true;

        Debug.Log("[Boss2Final] Boss defeated!");

        if (animator != null)
        {
            animator.SetTrigger("Death");
        }

        StopAllCoroutines();
    }

    IEnumerator BossAttackLoop()
    {
        Debug.Log("[Boss2Final] Attack Loop Started");

        yield return new WaitForSeconds(2f);

        Debug.Log("[Boss2Final] First attack starting");

        while (true)
        {
            if (!attacking && !dead)
            {
                Debug.Log(
                    "[Boss2Final] Calling BombAttack"
                );

                yield return StartCoroutine(
                    BombAttack()
                );

                Debug.Log(
                    "[Boss2Final] BombAttack finished"
                );
            }

            if (dead)
                yield break;

            Debug.Log(
                "[Boss2Final] Waiting "
                + attackInterval
                + " seconds"
            );

            yield return new WaitForSeconds(
                attackInterval
            );
        }
    }

    IEnumerator BombAttack()
    {
        Debug.Log("[Boss2Final] BombAttack START");

        attacking = true;

        if (animator != null)
        {
            animator.SetTrigger("BombAttack");

            Debug.Log(
                "[Boss2Final] BombAttack trigger sent"
            );
        }

        float targetY = groundY + flyHeight;

        Debug.Log(
            "[Boss2Final] Flying from Y "
            + transform.position.y
            + " to Y "
            + targetY
        );

        while (transform.position.y < targetY)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(
                    transform.position.x,
                    targetY,
                    transform.position.z
                ),
                flySpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            targetY,
            transform.position.z
        );

        Debug.Log(
            "[Boss2Final] Reached flying height: "
            + transform.position.y
        );

        SpawnBombWarnings();

        Debug.Log(
            "[Boss2Final] Waiting for warning/explosion: "
            + (warningTime + explosionTime)
            + " seconds"
        );

        yield return new WaitForSeconds(
            warningTime + explosionTime
        );

        Debug.Log("[Boss2Final] Starting to fall");

        while (transform.position.y > groundY)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                new Vector3(
                    transform.position.x,
                    groundY,
                    transform.position.z
                ),
                flySpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.position = new Vector3(
            transform.position.x,
            groundY,
            transform.position.z
        );

        Debug.Log(
            "[Boss2Final] Reached ground Y: "
            + groundY
        );

        attacking = false;

        Debug.Log("[Boss2Final] BombAttack END");
    }

    void SpawnBombWarnings()
    {
        Debug.Log(
            "[Boss2Final] SpawnBombWarnings START"
        );

        if (bombWarningPrefab == null)
        {
            Debug.LogError(
                "[Boss2Final] Bomb Warning Prefab is missing!"
            );

            return;
        }

        int count = Random.Range(
            minBombCount,
            maxBombCount + 1
        );

        Debug.Log(
            "[Boss2Final] Bomb count = "
            + count
        );

        List<float> usedPositions =
            new List<float>();

        for (int i = 0; i < count; i++)
        {
            float randomX;
            int attempts = 0;

            do
            {
                randomX = Random.Range(
                    bombMinX,
                    bombMaxX
                );

                attempts++;

            } while (
                IsTooClose(
                    randomX,
                    usedPositions
                )
                && attempts < 20
            );

            usedPositions.Add(randomX);

            Vector3 spawnPosition =
                new Vector3(
                    randomX,
                    bombY,
                    0
                );

            Debug.Log(
                "[Boss2Final] Spawn warning "
                + (i + 1)
                + " at X="
                + randomX
                + " Y="
                + bombY
            );

            Instantiate(
                bombWarningPrefab,
                spawnPosition,
                Quaternion.identity
            );
        }

        Debug.Log(
            "[Boss2Final] SpawnBombWarnings END"
        );
    }

    bool IsTooClose(
        float position,
        List<float> positions
    )
    {
        foreach (float usedPosition in positions)
        {
            if (Mathf.Abs(
                position - usedPosition
            ) < 1.5f)
            {
                return true;
            }
        }

        return false;
    }

    public void StartBombAttack()
    {
        Debug.Log(
            "[Boss2Final] StartBombAttack called"
        );

        if (attacking || dead)
        {
            Debug.Log(
                "[Boss2Final] Already attacking or dead"
            );

            return;
        }

        StartCoroutine(BombAttack());
    }
}