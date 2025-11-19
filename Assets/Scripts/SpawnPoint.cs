using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    public string previousSpawnID;

    void Awake()
    {
        if (previousSpawnID == "") 
            Debug.LogWarning("door ID missing from this object");
    }

}
