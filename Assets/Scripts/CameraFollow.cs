using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player; // Reference to the player
    public float smoothSpeed = 0.125f; // How smoothly the camera follows
    public Vector3 offset; // Offset from the player

    void LateUpdate()
    {
        Vector3 desiredPosition = player.position + offset;
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
    }
}