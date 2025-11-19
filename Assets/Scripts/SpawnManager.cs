using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    public string previousSpawnID = "";

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        // make sure there is only one instance of spawn manager in the scene
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetPreviousDoor(string doorID)
    {
        previousSpawnID = doorID;
        Debug.Log("Set last door: " + doorID);
    }

    public string GetPreviousDoor()
    {
        return previousSpawnID;
    }
}
