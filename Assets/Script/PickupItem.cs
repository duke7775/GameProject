using UnityEngine;

public class PickupItem : MonoBehaviour
{
    public enum PickupType
    {
        Coin,
        AttackSpeed,
        HP,
        Jump
    }

    public PickupType pickupType;

    public int coinAmount = 1;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }

        if (pickupType == PickupType.Coin)
        {
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoin(coinAmount);
            }

            Debug.Log(
                "Picked up Coin x" +
                coinAmount
            );
        }

        if (pickupType == PickupType.AttackSpeed)
        {
            PlayerMovement playerMovement =
                other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.UpgradeAttack();
            }
        }

        if (pickupType == PickupType.HP)
        {
            PlayerHealth playerHealth =
                other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.Heal(1);

                Debug.Log(
                    "Picked up HP +1"
                );
            }
        }

        if (pickupType == PickupType.Jump)
        {
            PlayerMovement playerMovement =
                other.GetComponent<PlayerMovement>();

            if (playerMovement != null)
            {
                playerMovement.UpgradeJump();
            }
        }

        Destroy(gameObject);
    }
}