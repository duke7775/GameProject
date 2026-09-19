using UnityEngine;
using TMPro;

public class PlayerUI : MonoBehaviour
{
    public TMP_Text hpText;
    public TMP_Text attackText;
    public TMP_Text jumpText;
    public TMP_Text coinText;

    private PlayerHealth playerHealth;
    private PlayerMovement playerMovement;

    void Start()
    {
        GameObject player =
            GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            playerHealth =
                player.GetComponent<PlayerHealth>();

            playerMovement =
                player.GetComponent<PlayerMovement>();
        }
    }

    void Update()
    {
        if (playerHealth != null)
        {
            hpText.text =
                playerHealth.currentHealth +
                "/" +
                playerHealth.maxHealth;
        }

        if (playerMovement != null)
        {
            int attackLevel =
                Mathf.RoundToInt(
                    (playerMovement.attackSpeed - 0.5f) / 0.2f
                ) + 1;

            int jumpLevel =
                Mathf.RoundToInt(
                    (playerMovement.jumpForce - 9f) / 0.25f
                ) + 1;

            attackText.text =
                "Lv." +
                attackLevel;

            jumpText.text =
                "Lv." +
                jumpLevel;
        }

        if (CoinManager.instance != null)
        {
            coinText.text =
                CoinManager.instance.coins.ToString();
        }
    }
}