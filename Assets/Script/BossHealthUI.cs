using UnityEngine;
using UnityEngine.UI;

public class BossHealthUI : MonoBehaviour
{
    public Slider healthBar;

    private Boss0 boss0;
    private Boss2 boss2;
    private Boss2Final boss2Final;

    void Start()
    {
        healthBar.gameObject.SetActive(false);
    }

    public void SetBoss(Boss0 boss)
    {
        boss0 = boss;
        boss2 = null;
        boss2Final = null;

        if (boss0 == null)
        {
            return;
        }

        healthBar.maxValue = boss0.maxHealth;
        healthBar.value = boss0.GetCurrentHealth();

        healthBar.gameObject.SetActive(true);
    }

    public void SetBoss2(Boss2 boss)
    {
        boss0 = null;
        boss2 = boss;
        boss2Final = null;

        if (boss2 == null)
        {
            return;
        }

        healthBar.maxValue = boss2.maxHealth;
        healthBar.value = boss2.GetCurrentHealth();

        healthBar.gameObject.SetActive(true);
    }

    public void SetBoss2Final(Boss2Final boss)
    {
        boss0 = null;
        boss2 = null;
        boss2Final = boss;

        if (boss2Final == null)
        {
            return;
        }

        healthBar.maxValue = boss2Final.maxHealth;
        healthBar.value = boss2Final.GetCurrentHealth();

        healthBar.gameObject.SetActive(true);
    }

    public void HideHealthBar()
    {
        healthBar.gameObject.SetActive(false);
    }

    void Update()
    {
        if (boss0 != null)
        {
            healthBar.value =
                boss0.GetCurrentHealth();

            if (boss0.GetCurrentHealth() <= 0)
            {
                healthBar.gameObject.SetActive(false);
            }

            return;
        }

        if (boss2 != null)
        {
            healthBar.value =
                boss2.GetCurrentHealth();

            if (boss2.GetCurrentHealth() <= 0)
            {
                healthBar.gameObject.SetActive(false);
            }

            return;
        }

        if (boss2Final != null)
        {
            healthBar.value =
                boss2Final.GetCurrentHealth();

            if (boss2Final.GetCurrentHealth() <= 0)
            {
                healthBar.gameObject.SetActive(false);
            }
        }
    }
}