using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class HatCollectable : MonoBehaviour
{
    [Header("Bounce Settings")]
    public float amplitude = 0.25f;
    public float frequency = 1f;

    [Header("Attach Offset")]
    public Vector3 attachOffset = new Vector3(0f, 0.5f, 0f);

    [Header("Save Settings")]
    public string saveKey = "HatCollected";

    [Header("Debug")]
    [Tooltip("Check to remove hat in editor")]
    public bool removeHatInEditor = false;

    private Vector3 startPos;
    private bool isCollected = false;
    private Transform playerTransform;

    void Start()
    {
        startPos = transform.position;
        isCollected = PlayerPrefs.GetInt(saveKey, 0) == 1;

        if (isCollected)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                AttachToPlayer(player.transform);
            }
            else
            {
                Debug.LogWarning("Player not found - hat won't be attached");
                isCollected = false;
            }
        }
    }

    void Update()
    {
        if (!isCollected)
        {
            float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
            transform.position = startPos + new Vector3(0f, offsetY, 0f);
        }

        // Handle debug removal
#if UNITY_EDITOR
        if (removeHatInEditor)
        {
            RemoveHat();
            removeHatInEditor = false; // Reset the flag
        }
#endif
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isCollected)
        {
            playerTransform = other.transform;
            AttachToPlayer(playerTransform);
            SaveCollectionState();
        }
    }

    void AttachToPlayer(Transform player)
    {
        isCollected = true;
        transform.SetParent(player);
        transform.localPosition = attachOffset;
        GetComponent<Collider2D>().enabled = false;

        if (TryGetComponent<Rigidbody2D>(out var rb))
            Destroy(rb);
    }

    void SaveCollectionState()
    {
        PlayerPrefs.SetInt(saveKey, 1);
        PlayerPrefs.Save();
    }

    public void RemoveHat()
    {
        // Reset parent and position
        transform.SetParent(null);
        transform.position = startPos;

        // Reset components
        GetComponent<Collider2D>().enabled = true;

        // Reset state
        isCollected = false;
        PlayerPrefs.SetInt(saveKey, 0);
        PlayerPrefs.Save();
    }

    public static void ResetHatState(string key)
    {
        PlayerPrefs.DeleteKey(key);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(HatCollectable))]
public class HatCollectableEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        HatCollectable hat = (HatCollectable)target;
        if (GUILayout.Button("Debug: Remove Hat"))
        {
            hat.RemoveHat();
        }
    }
}
#endif