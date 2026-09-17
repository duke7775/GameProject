using UnityEngine;

public class BossCameraTrigger : MonoBehaviour
{
    public Camera bossCamera;
    public Camera mainCamera;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            mainCamera.enabled = false;
            bossCamera.enabled = true;
        }
    }
}