using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

    [Header("Ad Reward")]
    public GameObject fakeAdPanel;      // assign FakeAdPanel here
    public HatCollectable adRewardHat;  // assign your AdRewardHat GameObject's HatCollectable here

    public GameObject pauseMenuCanvas;
    private bool isPaused = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            PauseGame();
        }
        else
        {
            ResumeGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f; // Freeze game time
        pauseMenuCanvas.SetActive(true);

        // Pause audio (optional)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.primarySource.Pause();
            AudioManager.Instance.transitionSource1.Pause();
            AudioManager.Instance.transitionSource2.Pause();
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f; // Resume game time
        pauseMenuCanvas.SetActive(false);

        // Resume audio (optional)
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.primarySource.UnPause();
            AudioManager.Instance.transitionSource1.UnPause();
            AudioManager.Instance.transitionSource2.UnPause();
        }

        isPaused = false;
    }

    public void QuitToMainMenu()
    {
        Time.timeScale = 1f; // Important: reset time scale

        // Stop all audio properly
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.primarySource.Stop();
            AudioManager.Instance.transitionSource1.Stop();
            AudioManager.Instance.transitionSource2.Stop();
        }

        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void OnWatchAdButton()
    {
        if (fakeAdPanel != null)
        {
            fakeAdPanel.SetActive(true);
        }
        else
        {
            Debug.LogWarning("FakeAdPanel not assigned in PauseManager!");
        }
    }

    public void OnCloseFakeAd()
    {
        // 1) Hide the fake ad UI
        if (fakeAdPanel != null)
            fakeAdPanel.SetActive(false);

        // 2) Award the hat by calling our new HatCollectable method
        if (adRewardHat != null)
        {
            adRewardHat.CollectFromAd();
        }
        else
        {
            Debug.LogWarning("adRewardHat not assigned in PauseManager!");
        }

        // 3) Resume normal gameplay
        ResumeGame();
    }
}