using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public static PauseManager Instance { get; private set; }

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
}