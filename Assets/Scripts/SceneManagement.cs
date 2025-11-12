using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagement : MonoBehaviour
{
    [Header("Scene Settings")]
    private AudioSource audioSource;

    private void Awake()
    {
        // audioSource = GetComponent<AudioSource>();
        // if (!audioSource)
        //     Debug.LogError("no audio source attacjed to SceneManagement");
    }


    /// <summary>
    /// Call this when the player fails the scene.
    /// </summary>
    public void OnSceneFail()
    {
        Debug.Log("You lost");
    }

    /// <summary>
    /// Loads a scene by its name
    /// </summary>
    public void LoadSceneByName(string name)
    {
        SceneManager.LoadScene(name);
    }

    /// <summary>
    /// Loads a scene by its name
    /// </summary>
    public void LoadSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    /// <summary>
    /// Loads the next scene.
    /// </summary>
    public void LoadNextScene()
    {
        Debug.LogWarning("No next scene specified.");
    }

    /// <summary>
    /// Reloads the current scene.
    /// </summary>
    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    /// <summary>
    /// Restarts the game from the first scene.
    /// </summary>
    public void RestartGame()
    {
        Debug.LogError("No next scene specified.");
    }

    private void PlaySound(AudioClip clip)
    {
        if (clip != null)
            audioSource.PlayOneShot(clip);
    }
}
