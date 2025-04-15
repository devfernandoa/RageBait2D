using UnityEngine;
using UnityEngine.SceneManagement;

public class EndGoal : MonoBehaviour
{
    public GameObject endMenuCanvas; // Assign in Inspector

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // Pause game and show menu
            Time.timeScale = 0f;
            endMenuCanvas.SetActive(true);

            // Stop and display final time
            GameTimer timer = FindObjectOfType<GameTimer>();
            if (timer != null)
            {
                timer.StopTimerAndShowFinalTime();
            }
        }
    }

    // Call this from a UI button to restart the level
    public void RestartLevel()
    {
        Time.timeScale = 1f; // Unpause
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Call this from a UI button to return to the main menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu"); // Replace with your main menu scene name
    }
}