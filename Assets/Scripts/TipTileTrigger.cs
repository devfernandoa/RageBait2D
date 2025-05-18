using UnityEngine;
using UnityEngine.Tilemaps;
using TMPro;

public class TileTipTrigger : MonoBehaviour
{
    public Tilemap tipTilemap;
    public GameObject tipUI;
    public float fadeDuration = 0.3f; // Adjustable in Inspector

    private TMP_Text[] tipTexts;
    private Coroutine fadeRoutine;

    void Start()
    {
        // Get all TextMeshPro components
        tipTexts = tipUI.GetComponentsInChildren<TMP_Text>();

        // Start hidden
        SetTextAlpha(0f);
        tipUI.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other == tipTilemap.GetComponent<Collider2D>())
        {
            tipUI.SetActive(true);
            FadeText(0f, 1f); // Fade in
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other == tipTilemap.GetComponent<Collider2D>())
        {
            FadeText(1f, 0f, () => tipUI.SetActive(false)); // Fade out then disable
        }
    }

    private void FadeText(float startAlpha, float targetAlpha, System.Action onComplete = null)
    {
        // Stop any existing fade
        if (fadeRoutine != null)
            StopCoroutine(fadeRoutine);

        fadeRoutine = StartCoroutine(FadeRoutine(startAlpha, targetAlpha, onComplete));
    }

    private System.Collections.IEnumerator FadeRoutine(float startAlpha, float targetAlpha, System.Action onComplete)
    {
        SetTextAlpha(startAlpha);

        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration);
            SetTextAlpha(alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        SetTextAlpha(targetAlpha);
        onComplete?.Invoke();
    }

    private void SetTextAlpha(float alpha)
    {
        foreach (TMP_Text text in tipTexts)
        {
            Color color = text.color;
            color.a = alpha;
            text.color = color;
        }
    }
}