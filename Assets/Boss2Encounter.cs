using UnityEngine;

public class Boss2Encounter : MonoBehaviour
{
    public GameObject boss;

    private bool triggered = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered)
            return;

        if (other.CompareTag("Player"))
        {
            triggered = true;

            if (boss != null)
            {
                boss.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}