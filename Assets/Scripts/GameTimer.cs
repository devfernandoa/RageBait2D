using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro; // Ensure you have TextMeshPro installed if using TMP

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI gameplayTimerText;  // Assign in Inspector (in-game timer)
    public TextMeshProUGUI finalTimeText;     // Assign in Inspector (end menu timer)

    private float elapsedTime = 0f;
    private bool isTimerRunning = true;

    void Update()
    {
        if (isTimerRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    void UpdateTimerDisplay()
    {
        if (gameplayTimerText != null)
        {
            gameplayTimerText.text = FormatTime(elapsedTime);
        }
    }

    public void StopTimerAndShowFinalTime()
    {
        isTimerRunning = false;
        if (finalTimeText != null)
        {
            finalTimeText.text = "Time: " + FormatTime(elapsedTime);
        }
    }

    private string FormatTime(float timeInSeconds)
    {
        TimeSpan timeSpan = TimeSpan.FromSeconds(timeInSeconds);
        return string.Format("{0:D2}:{1:D2}.{2:D3}",
            timeSpan.Minutes,
            timeSpan.Seconds,
            timeSpan.Milliseconds);
    }

    // Call this to reset the timer (e.g., when restarting)
    public void ResetTimer()
    {
        elapsedTime = 0f;
        isTimerRunning = true;
        UpdateTimerDisplay();
    }
}