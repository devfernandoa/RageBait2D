using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene"); // Replace with your game scene name
    }

    public void OpenOptions()
    {
        // Hide main menu, show options menu
    }

    public void QuitGame()
    {
        Application.Quit();

        // For testing in editor:
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}