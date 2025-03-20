using UnityEngine;

public class TracerFade : MonoBehaviour
{
    public float fadeDuration = 0.5f; // Time in seconds for the tracer to fade out
    private float fadeTimer;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        fadeTimer = fadeDuration;

        // Set the sorting order to ensure tracers render in front of the background
        spriteRenderer.sortingLayerName = "Foreground"; // Create a "Foreground" sorting layer if it doesn't exist
        spriteRenderer.sortingOrder = 1; // Adjust this value as needed
    }

    void Update()
    {
        // Reduce the fade timer
        fadeTimer -= Time.deltaTime;

        // Calculate the alpha value based on the fade timer
        float alpha = Mathf.Clamp01(fadeTimer / fadeDuration);

        // Update the sprite's color with the new alpha value
        Color color = spriteRenderer.color;
        color.a = alpha;
        spriteRenderer.color = color;

        // Destroy the tracer when it's fully faded out
        if (fadeTimer <= 0)
        {
            Destroy(gameObject);
        }
    }
}