using UnityEngine;

public class Parallax : MonoBehaviour
{
    public Transform cameraTransform; // Reference to the camera's transform
    public float relativeSpeed; // Speed of the parallax effect

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector2(cameraTransform.position.x * relativeSpeed, transform.position.y);
    }
}
