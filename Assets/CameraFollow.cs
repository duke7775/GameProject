using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;

    public float smoothSpeed = 5f;

    public float fixedY;

    public float followUpY = 2f;
    public float followDownY = -5f;

    void Start()
    {
        fixedY = transform.position.y;
    }

    void LateUpdate()
    {
        float targetY = fixedY;

        if (player.position.y > fixedY + followUpY)
        {
            targetY = player.position.y - followUpY;
        }

        else if (player.position.y < fixedY + followDownY)
        {
            targetY = player.position.y;
        }

        Vector3 targetPosition = new Vector3(
            player.position.x,
            targetY,
            transform.position.z
        );

        transform.position = Vector3.Lerp(
            transform.position,
            targetPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}