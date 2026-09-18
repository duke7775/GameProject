using UnityEngine;

public class Boss2Bullet : MonoBehaviour
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

            // Make the bullet face the direction it is moving
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            // If the bullet image points LEFT by default, use +180
            transform.rotation = Quaternion.Euler(0f, 0f, angle + 180f);
        }
    }

    void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    void Update()
    {
        transform.position +=
            (Vector3)(direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth player =
                other.GetComponent<PlayerHealth>();

            if (player != null)
            {
                player.TakeDamage(damage);
            }

            Destroy(gameObject);
        }
    }
}