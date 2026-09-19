using UnityEngine;

public class Boss2FinalTrigger : MonoBehaviour
{
    public GameObject finalBoss;

    public BossHealthUI bossHealthUI;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
        {
            return;
        }

        if (!other.CompareTag("Player"))
        {
            return;
        }

        triggered = true;

        if (finalBoss != null)
        {
            finalBoss.SetActive(true);

            Boss2Final boss2Final =
                finalBoss.GetComponent<Boss2Final>();

            if (bossHealthUI != null &&
                boss2Final != null)
            {
                bossHealthUI.SetBoss2Final(
                    boss2Final
                );
            }
        }

        Debug.Log("Final Boss Activated!");
    }
}