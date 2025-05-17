using UnityEngine;

public class HatCollectable : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.25f;
    public float frequency = 1f;

    [Header("Attach Offset")]
    public Vector3 attachOffset = new Vector3(0f, 0.5f, 0f);

    private Vector3 startPos;
    private bool isCollected = false;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        if (!isCollected)
        {
            float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = startPos + new Vector3(0f, offsetY, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            AttachToPlayer(other.transform);
        }
    }

    void AttachToPlayer(Transform player)
    {
        isCollected = true;
        transform.SetParent(player);  // Makes it follow the player
        transform.localPosition = attachOffset;  // Position on the head
        GetComponent<Collider2D>().enabled = false;
        // Optional: destroy Rigidbody if present
        if (TryGetComponent<Rigidbody2D>(out var rb))
            Destroy(rb);
    }
}