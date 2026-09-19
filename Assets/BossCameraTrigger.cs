using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    public Camera bossCamera;
    public Camera mainCamera;

    public GameObject boss0;

    public AudioSource normalBGM;
    public AudioSource bossBGM;

    public BossHealthUI bossHealthUI;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (triggered)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            triggered = true;

            mainCamera.enabled = false;
            bossCamera.enabled = true;

            if (boss0 != null)
            {
                boss0.SetActive(true);
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
                boss0 != null)
            {
                Boss0 boss =
                    boss0.GetComponent<Boss0>();

                bossHealthUI.SetBoss(boss);
            }
        }
    }
}