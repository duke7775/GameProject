using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    public float speed = 5f;
    public int damage = 1;
    public float lifeTime = 5f;

    private Vector2 direction = Vector2.right;

    public void SetDirection(Vector2 dir)
    {
        if (dir != Vector2.zero)
        {
            direction = dir.normalized;
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player = other.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}