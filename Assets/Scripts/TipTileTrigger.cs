using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTipTrigger : MonoBehaviour
{
    public Tilemap tipTilemap;
    public GameObject tipUI; // Now referencing the GameObject directly
    public float fadeDuration = 0.5f;

    private CanvasGroup tipUICanvasGroup;
    private bool isShowingTip = false;
    private Coroutine currentFadeCoroutine;

    void Start()
    {
        // Get or add CanvasGroup for fading
        tipUICanvasGroup = tipUI.GetComponent<CanvasGroup>();
        if (tipUICanvasGroup == null)
        {
            tipUICanvasGroup = tipUI.AddComponent<CanvasGroup>();
        }

        // Start with UI hidden
        tipUICanvasGroup.alpha = 0f;
        tipUI.SetActive(false);
    }

    void Update()
    {
        Vector3 playerPos = transform.position;
        Vector3Int cellPos = tipTilemap.WorldToCell(playerPos);

        TileBase tile = tipTilemap.GetTile(cellPos);
        if (tile is TipTile tipTile)
        {
            if (!isShowingTip)
            {
                ShowTipUI();
            }
        }
        else
        {
            if (isShowingTip)
            {
                HideTipUI();
            }
        }
    }

    private void ShowTipUI()
    {
        isShowingTip = true;
        tipUI.SetActive(true);

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeUI(0f, 1f));
    }

    private void HideTipUI()
    {
        isShowingTip = false;

        if (currentFadeCoroutine != null)
        {
            StopCoroutine(currentFadeCoroutine);
        }
        currentFadeCoroutine = StartCoroutine(FadeUI(1f, 0f, () => tipUI.SetActive(false)));
    }

    private System.Collections.IEnumerator FadeUI(float startAlpha, float targetAlpha, System.Action onComplete = null)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            tipUICanvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }

        tipUICanvasGroup.alpha = targetAlpha;
        onComplete?.Invoke();
    }
}