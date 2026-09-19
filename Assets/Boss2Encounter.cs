using UnityEngine;

public class Boss2Encounter : MonoBehaviour
{
    public GameObject boss;

    public AudioSource normalBGM;
    public AudioSource bossBGM;

    public BossHealthUI bossHealthUI;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (boss != null)
            {
                boss.SetActive(true);
            }

            if (normalBGM != null)
            {
                normalBGM.Stop();
            }

            if (bossBGM != null)
            {
                bossBGM.loop = true;
                bossBGM.Play();
            }

            if (bossHealthUI != null &&
                boss != null)
            {
                Boss2 boss2 =
                    boss.GetComponent<Boss2>();

                if (boss2 != null)
                {
                    bossHealthUI.SetBoss2(boss2);
                }
            }

            Destroy(gameObject);
        }
    }
}