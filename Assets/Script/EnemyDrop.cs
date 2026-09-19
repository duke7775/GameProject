using UnityEngine;

public class EnemyDrop : MonoBehaviour
{
    public GameObject coinPrefab;
    public GameObject attackSpeedPrefab;
    public GameObject hpPrefab;
    public GameObject jumpPrefab;

    [Range(0f, 100f)]
    public float coinChance = 70f;

    [Range(0f, 100f)]
    public float itemChance = 10f;

    [Range(0f, 100f)]
    public float oneCoinChance = 60f;

    [Range(0f, 100f)]
    public float twoCoinChance = 30f;

    private bool hasDropped = false;

    public void Drop()
    {
        if (hasDropped)
        {
            return;
        }

        hasDropped = true;

        float roll = Random.Range(0f, 100f);

        if (roll < coinChance)
        {
            DropCoins();
            return;
        }

        if (roll < coinChance + itemChance)
        {
            DropItem();
            return;
        }
    }

    void DropCoins()
    {
        if (coinPrefab == null)
        {
            Debug.LogError(
                gameObject.name +
                " Coin Prefab is missing!"
            );

            return;
        }

        int amount;

        float roll = Random.Range(
            0f,
            100f
        );

        if (roll < oneCoinChance)
        {
            amount = 1;
        }
        else if (
            roll <
            oneCoinChance +
            twoCoinChance
        )
        {
            amount = 2;
        }
        else
        {
            amount = 3;
        }

        for (int i = 0; i < amount; i++)
        {
            if (coinPrefab == null)
            {
                return;
            }

            Instantiate(
                coinPrefab,
                transform.position,
                Quaternion.identity
            );
        }

        Debug.Log(
            gameObject.name +
            " dropped Coin x" +
            amount
        );
    }

    void DropItem()
    {
        GameObject itemPrefab = null;

        float roll = Random.Range(
            0f,
            100f
        );

        if (roll < 40f)
        {
            itemPrefab = attackSpeedPrefab;
        }
        else if (roll < 75f)
        {
            itemPrefab = hpPrefab;
        }
        else
        {
            itemPrefab = jumpPrefab;
        }

        if (itemPrefab == null)
        {
            Debug.LogError(
                gameObject.name +
                " Item Prefab is missing!"
            );

            return;
        }

        Instantiate(
            itemPrefab,
            transform.position,
            Quaternion.identity
        );

        Debug.Log(
            gameObject.name +
            " dropped an item"
        );
    }
}