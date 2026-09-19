using UnityEngine;

public class Shop : MonoBehaviour
{
    public int cost = 20;

    private GameObject player;
    private bool playerInRange = false;

    void Update()
    {
        if (!playerInRange)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            DrawReward();
        }
    }

    void DrawReward()
    {
        if (CoinManager.instance == null)
        {
            return;
        }

        if (!CoinManager.instance.SpendCoins(cost))
        {
            Debug.Log("Not enough coins!");
            return;
        }

        PlayerMovement playerMovement =
            player.GetComponent<PlayerMovement>();

        PlayerHealth playerHealth =
            player.GetComponent<PlayerHealth>();

        int reward = Random.Range(0, 3);

        if (reward == 0)
        {
            if (playerMovement != null)
            {
                playerMovement.UpgradeAttack();

                Debug.Log(
                    "Shop Reward: Attack Up!"
                );
            }
        }
        else if (reward == 1)
        {
            if (playerHealth != null)
            {
                playerHealth.Heal(1);

                Debug.Log(
                    "Shop Reward: HP +1!"
                );
            }
        }
        else
        {
            if (playerMovement != null)
            {
                playerMovement.UpgradeJump();

                Debug.Log(
                    "Shop Reward: Jump Up!"
                );
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = other.gameObject;
            playerInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            player = null;
            playerInRange = false;
        }
    }
}