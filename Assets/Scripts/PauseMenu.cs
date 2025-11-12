using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    [Header("Assign your pause menu GameObject here")]
    public GameObject pauseMenuUI;
    public GameObject optionsMenuUI;

    private bool isPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void Options()
    {
        pauseMenuUI.SetActive(false);
        optionsMenuUI.SetActive(true);
    }

    // Optional helper for UI buttons
    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
    }
}
