using UnityEngine;
// using UnityEngine.AdaptivePerformance;

public class SpawnScene : MonoBehaviour
{
    public string nextSceneName;
    public string currentSpawnID;
    private SceneManagement sceneManagement;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    // void Awake()
    // {
    //     sceneManagement = FindAnyObjectByType<SceneManagement>();

    //     if (!sceneManagement)
    //         Debug.LogError("SceneManagement not found in scene.");
    // }

    void OnTriggerEnter(Collider other)
    {
        if(other.transform.CompareTag("Player"))
        {
            Debug.Log("loading next scene: " + nextSceneName);
            EnterScene();
            sceneManagement.LoadSceneByName(nextSceneName);
        }
        else 
        {
            Debug.Log("error loading next scene");
        }
    }

    public void EnterScene()
    {
        if (currentSpawnID == "" || currentSpawnID == null)
        {
            Debug.LogWarning("there is no door id attached to this scene spawned");
        }
        SpawnManager.Instance.SetPreviousDoor(currentSpawnID);
    }
}
